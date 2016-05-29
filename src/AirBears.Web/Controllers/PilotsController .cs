using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;
using AirBears.Web.Models;
using AirBears.Web.ViewModels;
using AutoMapper;
using Microsoft.AspNet.Authorization;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using System;
using AirBears.Web.Services;
using System.Linq.Expressions;

namespace AirBears.Web.Controllers
{
    [Produces("application/json")]
    [Route("api/pilots")]
    [Authorize(AuthPolicies.Bearer)]
    public class PilotsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IGeocodeService _geocodeService;

        public PilotsController(AppDbContext context, UserManager<User> userManager, IGeocodeService geocodeService)
        {
            _context = context;
            _userManager = userManager;
            _geocodeService = geocodeService;
        }

        // GET: api/pilots/5
        [HttpGet("{id}", Name = "Get Pilot")]
        public async Task<IActionResult> GetPilot([FromRoute] string id)
        {
            var pilot = await _context.Users.AsPilots().Where(p => p.Id == id).FirstOrDefaultAsync();

            if (pilot == null)
            {
                return HttpNotFound();
            }

            return Ok(Mapper.Map<PilotViewModel>(pilot));
        }

        // GET: api/pilots
        [HttpGet]
        [Authorize(AuthPolicies.Bearer, Roles = Roles.Admin)]
        public async Task<QueryResult<PilotViewModel>> GetPilots(string name, string sortBy, bool ascending = true, bool onlyShirtsNotSent = false, int page = 1, int? pageSize = 50)
        {
            var pilots = _context.Users.AsPilots();            

            if(!string.IsNullOrWhiteSpace(name))
            {
                pilots = pilots.Where(p => 
                            p.FirstName.ToLower().Contains(name.ToLower()) || 
                            p.LastName.ToLower().Contains(name.ToLower()) ||
                            p.Email.ToLower().Contains(name.ToLower()));
            }

            if (onlyShirtsNotSent) { pilots = pilots.Where(p => !p.TeeShirtMailedDate.HasValue); }

            if (ascending)
                pilots = pilots.OrderBy(Static.GetSortExpression(sortBy));
            else
                pilots = pilots.OrderByDescending(Static.GetSortExpression(sortBy));

            // Limit the page size to 200.
            if (!pageSize.HasValue) { pageSize = 200; }

            var result = new QueryResult<PilotViewModel>()
            {
                Items = (await pilots.Skip((page - 1) * pageSize.Value).Take(pageSize.Value).ToListAsync()).Select(p => Mapper.Map<PilotViewModel>(p)),
                Page = page,
                PageSize = pageSize.Value,
                TotalCount = pilots.Count()
            };

            return result;
        }

        /// <summary>
        /// Returns a list of pilots that match the search criteria and are within a particular distance (in miles) from the address or coordinates.
        /// </summary>
        /// <returns></returns>
        [HttpPost("search", Name = "Pilot Search")]
        public async Task<IActionResult> Search([FromBody] PilotSearchViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            var currentUser = await _userManager.FindByIdAsync(User.GetUserId());

            // If lat/lng were provided, use them to perform the distance query.
            if (model.Latitude.HasValue && model.Longitude.HasValue)
            {
                return Ok(await FindPilotsWithinRadius(model.Distance, model.Latitude.Value, model.Longitude.Value, !currentUser.IsAuthorityAccount));
            }

            // Otherwise we have to ask the Geocode service to find the lat/lng for the given address.
            var coords = await _geocodeService.GetCoordsForAddress(model.Address);

            if (coords.Status != GeocodeResponseStatus.OK)
            {
                ModelState.AddModelError("", coords.Status.ToString());
                return HttpBadRequest(ModelState);
            }

            return Ok(await FindPilotsWithinRadius(model.Distance, coords.Latitude, coords.Longitude, !currentUser.IsAuthorityAccount));
        }

        // PUT: api/pilots/5/tee-shirt-mailed
        [HttpPut("{id}/tee-shirt-mailed", Name = "Mark T-Shirt Mailed")]
        [Authorize(AuthPolicies.Bearer, Roles = Roles.Admin)]
        public async Task<IActionResult> MarkTeeShirtMailed([FromRoute] string id, [FromBody] bool teeShirtMailed)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return HttpNotFound();
            }

            user.TeeShirtMailedDate = teeShirtMailed ? DateTime.UtcNow : default(DateTime?);
            _context.Users.Update(user, GraphBehavior.SingleObject);
            await _context.SaveChangesAsync();

            return Ok(Mapper.Map<PilotViewModel>(user));
        }

        // GET: api/pilots/me
        [HttpPut("me", Name = "Update Pilot")]
        [Authorize(AuthPolicies.Bearer)]
        public async Task<IActionResult> UpdatePilot([FromBody] PilotViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            var user = await _context.Users.SingleOrDefaultAsync(u => u.Id == User.GetUserId());
            var addressHasChanged = AddressHasChanged(user, model);

            user = Mapper.Map(model, user);

            if (addressHasChanged)
            {
                //if the pilot's address has changed, get the updated coords from Google.

                var status = await UpdatePilotCoordinates(user, model.State.Name);

                if (status != GeocodeResponseStatus.OK)
                {
                    ModelState.AddModelError(string.Empty, status.ToString());
                    return HttpBadRequest(ModelState);
                }
            }

            await _context.SaveChangesAsync();

            return Ok();
        }

        private bool AddressHasChanged(User user, PilotViewModel pilot)
        {
            return user.StateId != pilot.State.Id
                || user.City != pilot.City
                || user.Street1 != pilot.Street1
                || user.Street2 != pilot.Street2
                || user.Zip != pilot.Zip
                || user.HasInternationalAddress != pilot.HasInternationalAddress
                || user.AddressLine1 != pilot.AddressLine1
                || user.AddressLine2 != pilot.AddressLine2
                || user.AddressLine3 != pilot.AddressLine3
                || user.AddressLine4 != pilot.AddressLine4;
        }

        private async Task<GeocodeResponseStatus> UpdatePilotCoordinates(User pilot, string state)
        {
            var coords = await _geocodeService.GetCoordsForAddress(pilot.GetAddress(state));

            if (coords.Status != GeocodeResponseStatus.OK)
            {
                return coords.Status;
            }

            pilot.Longitude = coords.Longitude;
            pilot.Latitude = coords.Latitude;
            pilot.GeocodeAddress = coords.GeocodeAddress;

            return coords.Status;
        }

        public const int EarthRadius = 3959; // Distance from the Earth's center to its surface, about 3,959 miles (6,371 kilometers).

        private async Task<IEnumerable<PilotSearchResultViewModel>> FindPilotsWithinRadius(int distance, double latitude, double longitude, bool onlyConsenting)
        {
            var consentClause = onlyConsenting ? "AND AllowsPilotSearch = 1 " : string.Empty;
            var query = "SELECT Id, IsAuthorityAccount, AllowsPilotSearch FROM dbo.AspNetUsers "
               + $"WHERE ({ EarthRadius } * acos(cos(radians({ latitude })) * cos(radians(Latitude)) "
               + $"* cos(radians(Longitude) - radians({ longitude })) + sin(radians({ latitude })) * sin(radians(Latitude)))) < { distance } "
               + $"AND IsAuthorityAccount = 0 { consentClause }";

            var pilotIds = _context.Users.FromSql(query).Select(u => u.Id).ToList();
            var users = await _context.Users.AsPilots().Where(u => pilotIds.Contains(u.Id)).ToListAsync();
            var pilots = Mapper.Map<List<PilotSearchResultViewModel>>(users);

            pilots.ForEach(p =>
            {
                p.Distance = (EarthRadius *
                    Math.Acos((Math.Cos(latitude.ToRadians())) * Math.Cos(p.Latitude.Value.ToRadians()) *
                    Math.Cos(p.Longitude.Value.ToRadians() - longitude.ToRadians()) +
                    Math.Sin(latitude.ToRadians()) * Math.Sin(p.Latitude.Value.ToRadians())));
            });

            return pilots.OrderBy(p => p.Distance);
        }

        //private async Task<QueryResult<PilotSearchResultViewModel>> FindPilotsWithinRadius(int distance, double latitude, double longitude, int page, int pageSize, bool onlyConsenting)
        //{
        //    var consentClause = onlyConsenting ? "AND AllowsPilotSearch = 1 " : string.Empty;
        //    var countQuery = "SELECT Id, IsAuthorityAccount, AllowsPilotSearch FROM dbo.AspNetUsers "
        //       + $"WHERE ({ EarthRadius } * acos(cos(radians({ latitude })) * cos(radians(Latitude)) "
        //       + $"* cos(radians(Longitude) - radians({ longitude })) + sin(radians({ latitude })) * sin(radians(Latitude)))) < { distance } "
        //       + $"AND IsAuthorityAccount = 0 { consentClause }";

        //    var pagedSqlQuery = "SELECT Id FROM (SELECT Id, IsAuthorityAccount, AllowsPilotSearch, "
        //           + $"({ EarthRadius } * acos(cos(radians({ latitude })) * cos(radians(Latitude)) "
        //           + $"* cos(radians(Longitude) - radians({ longitude })) + sin(radians({ latitude })) * sin(radians(Latitude)))) as Distance "
        //           + $"FROM dbo.AspNetUsers) as temp "
        //           + $"WHERE Distance <= { distance } AND IsAuthorityAccount = 0 { consentClause }"
        //           + $"ORDER BY Distance "
        //           + $" OFFSET { pageSize } * { page - 1 } ROWS "
        //           + $"FETCH NEXT { pageSize } ROWS ONLY";

        //    var totalCount = _context.Users.FromSql(countQuery).Count();
        //    var pilotIds = _context.Users.FromSql(pagedSqlQuery).Select(u => u.Id).ToList();
        //    var users = await _context.Users.AsPilots().Where(u => pilotIds.Contains(u.Id)).ToListAsync();

        //    var pilots = Mapper.Map<List<PilotSearchResultViewModel>>(users);

        //    pilots.ForEach(p =>
        //    {
        //        p.Distance = (EarthRadius *
        //            Math.Acos((Math.Cos(latitude.ToRadians())) * Math.Cos(p.Latitude.Value.ToRadians()) *
        //            Math.Cos(p.Longitude.Value.ToRadians() - longitude.ToRadians()) +
        //            Math.Sin(latitude.ToRadians()) * Math.Sin(p.Latitude.Value.ToRadians())));
        //    });

        //    var result = new QueryResult<PilotSearchResultViewModel>()
        //    {
        //        Items = pilots.OrderBy(p => p.Distance),
        //        Page = page,
        //        PageSize = pageSize,
        //        TotalCount = totalCount
        //    };

        //    return result;
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}