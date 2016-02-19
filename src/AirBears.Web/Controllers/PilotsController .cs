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
    //[Authorize(AuthPolicies.Bearer)]
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

        // GET: api/pilots
        [HttpGet]
        [Authorize(AuthPolicies.Bearer, Roles = Roles.Admin)]
        public async Task<IEnumerable<UserViewModel>> GetPilots()
        {
            var users = await _context.Users
                .Where(u => !u.IsAuthorityAccount)
                .Include(u => u.TeeShirtSize)
                .Include(u => u.State)
                .OrderBy(u => u.LastName)
                .ToListAsync();

            return Mapper.Map<IEnumerable<UserViewModel>>(users);
        }

        // GET: api/pilots/search?address=
        [HttpGet("search", Name = "Pilot Search")]
        //[Authorize(AuthPolicies.Bearer, Roles = Roles.Admin_And_Authority)]
        public async Task<IActionResult> Search(string address)
        {
            var coords = await _geocodeService.GetCoordsForAddress(address);

            if (coords.Status != GeocodeResponseStatus.OK)
            {
                ModelState.AddModelError("", coords.Status.ToString());
                return HttpBadRequest(ModelState);
            }


            //var dapperQuery = "select " + userCols + " from [User]  where " + //....
            //                                                                  // other search code... 
            //dapperQuery = dapperQuery + " and " +
            //    "([LocationLong] > @longMin and [LocationLong] < @longMax and [LocationLat] > @latMin and [LocationLat] < @latMax) and " +
            //    "((geography::STPointFromText([LocationPoint], 4326).STDistance(@userPoint)) <= @searchRadius)";

            //SELECT id, (3959 * acos(cos(radians(37)) * cos(radians(lat))
            //* cos(radians(lng) - radians(-122)) + sin(radians(37)) * sin(radians(lat)))) AS distance
            //FROM markers
            //HAVING distance < 25
            //ORDER BY distance

            //Dim radians = 57.295779513082323
            //Dim result = (_
            //    From t1 in Entities.ZipCodeDatas
            //    From t2 in Entities.ZipCodeDatas
            //    Where t2.ZipCode = "10001" _
            //    And 3963.0 * SqlFunctions.Acos(_
            //        SqlFunctions.Sin(t1.Latitude / (radians)) _
            //        * SqlFunctions.Sin(t2.Latitude / (57.295779513082323)) _
            //        + SqlFunctions.Cos(t1.Latitude / (radians)) _
            //        * SqlFunctions.Cos(t2.Latitude / (radians)) _
            //        * SqlFunctions.Cos(t2.Longitude / (radians) - t1.Longitude / (radians)) _
            //    ) < 100
            //    Select New With {
            //                _
            //  ZipCodeDatas = t1, _
            //        DistanceInMiles = 3963.0 * SqlFunctions.Acos(_
            //            SqlFunctions.Sin(t1.Latitude / (@radians)) _
            //            * SqlFunctions.Sin(t2.Latitude / (57.295779513082323)) _
            //            + SqlFunctions.Cos(t1.Latitude / (@radians)) _
            //            * SqlFunctions.Cos(t2.Latitude / (@radians)) _
            //            * SqlFunctions.Cos(t2.Longitude / (@radians) - t1.Longitude / (@radians)) _
            //        )
            //    }
            //).OrderBy(o => o.DistanceInMiles)

            var sqlQuery = "SELECT * FROM dbo.AspNetUsers WHERE (3959 * acos(cos(radians(" + coords.Latitude + ")) * cos(radians(Latitude)) "
            + "* cos(radians(Longitude) - radians(" + coords.Longitude + ")) + sin(radians(" + coords.Latitude + ")) * sin(radians(Latitude)))) < 25";
            var users = _context.Users.FromSql(sqlQuery).ToList();

            //var users = await _context.Users.Select(u => new
            //{
            //    Firstname = u.FirstName,
            //    Distance = Math.Acos(Math.Cos(coords.Latitude) * u.)
            //}).ToListAsync();

            //var users = await _context.Users
            //    .Where(u => !u.IsAuthorityAccount)
            //    .Include(u => u.TeeShirtSize)
            //    .Include(u => u.State)
            //    .OrderBy(u => u.LastName)
            //    .ToListAsync();

            return Ok(Mapper.Map<IEnumerable<UserViewModel>>(users));
        }

        // GET: api/pilots/5
        [HttpGet("{id}", Name = "GetPilot")]
        [Authorize(AuthPolicies.Bearer, Roles = Roles.Admin)]
        public async Task<IActionResult> GetPilot([FromRoute] string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return HttpNotFound();
            }

            return Ok(Mapper.Map<UserViewModel>(user));
        }

        // PUT: api/pilots/5/tee-shirt-mailed
        [HttpPut("{id}/tee-shirt-mailed", Name = "MarkTeeShirtMailed")]
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

            return Ok(Mapper.Map<UserViewModel>(user));
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