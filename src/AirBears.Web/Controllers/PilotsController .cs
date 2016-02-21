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

        // GET: api/pilots/search?address=xyz&distance=25
        [HttpGet("search", Name = "Pilot Search")]
        //[Authorize(AuthPolicies.Bearer, Roles = Roles.Admin_And_Authority)]
        public async Task<IActionResult> Search(string address, int distance)
        {
            var coords = await _geocodeService.GetCoordsForAddress(address);

            if (coords.Status != GeocodeResponseStatus.OK)
            {
                ModelState.AddModelError("", coords.Status.ToString());
                return HttpBadRequest(ModelState);
            }

            if(distance > 500)
            {
                ModelState.AddModelError("distance", "The distance cannot exceed 1000 miles.");
                return HttpBadRequest(ModelState);
            }

            //SELECT id, (3959 * acos(cos(radians(37)) * cos(radians(lat))
            //* cos(radians(lng) - radians(-122)) + sin(radians(37)) * sin(radians(lat)))) AS distance
            //FROM markers
            //HAVING distance < 25
            //ORDER BY distance

            var sqlQuery = "SELECT * FROM dbo.AspNetUsers "
                + $"WHERE (3959 * acos(cos(radians({coords.Latitude})) * cos(radians(Latitude)) "
                + $"* cos(radians(Longitude) - radians({coords.Longitude})) + sin(radians({coords.Latitude})) * sin(radians(Latitude)))) < {distance}";

            var results = _context.Users.FromSql(sqlQuery).ToList();
            var users = Mapper.Map<List<PilotSearchResultViewModel>>(results);

            users.ForEach(u =>
            {
                u.Distance = (3959 *
                    Math.Acos((Math.Cos(coords.Latitude.ToRadians())) * Math.Cos(u.Latitude.Value.ToRadians()) *
                    Math.Cos(u.Longitude.Value.ToRadians() - coords.Longitude.ToRadians()) +
                    Math.Sin(coords.Latitude.ToRadians()) * Math.Sin(u.Latitude.Value.ToRadians())));
            });

            return Ok(users.OrderBy(u => u.Distance));
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