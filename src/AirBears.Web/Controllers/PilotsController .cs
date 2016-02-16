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

namespace AirBears.Web.Controllers
{
    [Produces("application/json")]
    [Route("api/pilots")]
    [Authorize(AuthPolicies.Bearer)]
    public class PilotsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;

        public PilotsController(AppDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
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
        [HttpGet("search?address={address}", Name = "Pilot Search")]
        [Authorize(AuthPolicies.Bearer, Roles = Roles.Admin_And_Authority)]
        public async Task<IEnumerable<UserViewModel>> Search(string address)
        {
            var users = await _context.Users
                .Where(u => !u.IsAuthorityAccount)
                .Include(u => u.TeeShirtSize)
                .Include(u => u.State)
                .OrderBy(u => u.LastName)
                .ToListAsync();

            return Mapper.Map<IEnumerable<UserViewModel>>(users);
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