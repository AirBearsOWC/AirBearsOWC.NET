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
            var pilot = await _context.Users
               .Where(u => !u.IsAuthorityAccount && u.Id == id)
               .Include(u => u.TeeShirtSize)
               .Include(u => u.State)
               .Include(u => u.FlightTime)
               .Include(u => u.Payload)
               .FirstOrDefaultAsync();

            if (pilot == null)
            {
                return HttpNotFound();
            }

            return Ok(Mapper.Map<PilotViewModel>(pilot));
        }

        [Route("me")]
        [HttpGet]
        public async Task<IActionResult> GetCurrentUser()
        {
            var user = await _context.Users
                .Include(u => u.TeeShirtSize)
                .Include(u => u.State)
                .Include(u => u.FlightTime)
                .Include(u => u.Payload)
                .FirstOrDefaultAsync(u => u.Id == User.GetUserId());

            var resp = Mapper.Map<IdentityPilotViewModel>(user);

            resp.Roles = User.GetRoles();

            return Ok(resp);
        }

        // GET: api/pilots
        [HttpGet]
        [Authorize(AuthPolicies.Bearer, Roles = Roles.Admin)]
        public async Task<IEnumerable<PilotViewModel>> GetPilots()
        {
            var pilots = await _context.Users
                .Where(u => !u.IsAuthorityAccount)
                .Include(u => u.TeeShirtSize)
                .Include(u => u.State)
                .Include(u => u.FlightTime)
                .Include(u => u.Payload)
                .OrderBy(u => u.LastName)
                .ToListAsync();

            return Mapper.Map<IEnumerable<PilotViewModel>>(pilots);
        }

        /// <summary>
        /// Returns a list of pilots that match the search criteria and are within a particular distance (in miles) from the address or coordinates.
        /// </summary>
        /// <returns></returns>
        [HttpPost("search", Name = "Pilot Search")]
        public async Task<IActionResult> Search([FromBody]PilotSearchViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            // If lat/lng were provided, use them to perform the distance query.
            if(model.Latitude.HasValue && model.Longitude.HasValue)
            {
                return Ok(await FindPilotsWithinRadius(model.Distance, model.Latitude.Value, model.Longitude.Value, model.Page, model.PageSize));
            }

            // Otherwise we have to ask the Geocode service to find the lat/lng for the given address.
            var coords = await _geocodeService.GetCoordsForAddress(model.Address);

            if (coords.Status != GeocodeResponseStatus.OK)
            {
                ModelState.AddModelError("", coords.Status.ToString());
                return HttpBadRequest(ModelState);
            }

            return Ok(await FindPilotsWithinRadius(model.Distance, coords.Latitude, coords.Longitude, model.Page, model.PageSize));
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
            _context.SaveChanges();

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

            if (AddressHasChanged(user, model))
            {
                //if the pilot's address has changed, get the updated coords from Google.

                var status = await UpdatePilotCoordinates(user, model.State.Name);

                if (status != GeocodeResponseStatus.OK)
                {
                    ModelState.AddModelError(string.Empty, status.ToString());
                    return HttpBadRequest(ModelState);
                }
            }

            user = Mapper.Map(model, user);

            await _context.SaveChangesAsync();

            return Ok();
        }

        private bool AddressHasChanged(User user, PilotViewModel pilot)
        {
            return user.StateId != pilot.State.Id || user.City != pilot.City || user.Street1 != pilot.Street1 || user.Street2 != pilot.Street2;
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

        private async Task<QueryResult<PilotSearchResultViewModel>> FindPilotsWithinRadius(int distance, double latitude, double longitude, int page, int pageSize)
        {
            var sqlQuery = "SELECT Id FROM dbo.AspNetUsers "
               + $"WHERE (3959 * acos(cos(radians({latitude})) * cos(radians(Latitude)) "
               + $"* cos(radians(Longitude) - radians({longitude})) + sin(radians({latitude})) * sin(radians(Latitude)))) < {distance}";

            var pilotIds = _context.Users.FromSql(sqlQuery).Select(u => u.Id).ToList();
            var users = await _context.Users.Include(u => u.TeeShirtSize)
                                        .Include(u => u.State)
                                        .Include(u => u.FlightTime)
                                        .Include(u => u.Payload)
                                        .Where(u => !u.IsAuthorityAccount && pilotIds.Contains(u.Id))
                                        .ToListAsync();

            var pilots = Mapper.Map<List<PilotSearchResultViewModel>>(users);

            pilots.ForEach(p =>
            {
                p.Distance = (3959 *
                    Math.Acos((Math.Cos(latitude.ToRadians())) * Math.Cos(p.Latitude.Value.ToRadians()) *
                    Math.Cos(p.Longitude.Value.ToRadians() - longitude.ToRadians()) +
                    Math.Sin(latitude.ToRadians()) * Math.Sin(p.Latitude.Value.ToRadians())));
            });

            var result = new QueryResult<PilotSearchResultViewModel>()
            {
                Items = pilots.OrderBy(p => p.Distance).Skip((page - 1) * pageSize).Take(pageSize),
                Page = page,
                PageSize = pageSize,
                TotalCount = pilotIds.Count
            };

            return result;
        }

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