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
    [Route("api/users")]
    [Authorize(AuthPolicies.Bearer)]
    public class UsersController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;

        public UsersController(AppDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Route("/api/me")]
        [HttpGet]
        public async Task<IdentityViewModel> GetCurrentUser()
        {
            var user = await _context.Users
                .Include(u => u.TeeShirtSize)
                .Include(u => u.State)
                .FirstOrDefaultAsync(u => u.Id == User.GetUserId());

            var resp = Mapper.Map<IdentityViewModel>(user);

            resp.Roles = User.GetRoles();

            return resp;
        }

        // GET: api/users
        [HttpGet]
        [Authorize(AuthPolicies.Bearer, Roles = Roles.Admin)]
        public async Task<IEnumerable<UserViewModel>> GetUsers()
        {
            var users = await _context.Users
                .Include(u => u.TeeShirtSize)
                .Include(u => u.State)
                .OrderBy(u => u.LastName)
                .ToListAsync();

            return Mapper.Map<IEnumerable<UserViewModel>>(users);
        }

        // GET: api/users/5
        [HttpGet("{id}", Name = "GetUser")]
        [Authorize(AuthPolicies.Bearer, Roles = Roles.Admin)]
        public async Task<IActionResult> GetUser([FromRoute] string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return HttpNotFound();
            }

            return Ok(Mapper.Map<UserViewModel>(user));
        }

        // PUT: /api/users/5/authority-approval
        [HttpPut("{id}/authority-approval", Name = "Approve Authority")]
        [Authorize(AuthPolicies.Bearer, Roles = Roles.Admin)]
        public async Task<IActionResult> ApproveAuthority([FromRoute] string id, [FromBody] bool isAuthorityApproved)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return HttpNotFound();
            }

            if (await _userManager.IsInRoleAsync(user, Roles.Authority))
            {
                return HttpBadRequest($"{user.UserName} already has the {Roles.Authority} role!");
            }

            await _userManager.AddToRoleAsync(user, Roles.Authority);

            return Ok();
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