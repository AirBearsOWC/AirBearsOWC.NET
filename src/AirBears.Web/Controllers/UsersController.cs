using AirBears.Web.Models;
using AirBears.Web.Services;
using AirBears.Web.ViewModels;
using AutoMapper;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AirBears.Web.Controllers
{
    [Produces("application/json")]
    [Route("api/users")]
    [Authorize(AuthPolicies.Bearer)]
    public class UsersController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IMailer _mailer;

        public UsersController(AppDbContext context, UserManager<User> userManager, IMailer mailer)
        {
            _context = context;
            _userManager = userManager;
            _mailer = mailer;
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

        /// <summary>
        /// Changes the current user's password.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [HttpPost("/api/me/password", Name = "Change Password")]
        public async Task<IActionResult> ChangePassword([FromBody]ChangePasswordViewModel model)
        {
            var user = await _userManager.FindByIdAsync(User.GetUserId());

            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return HttpBadRequest(ModelState);
            }

            await SendChangePasswordEmail(user);

            return Ok();
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

        private async Task SendChangePasswordEmail(User user)
        {
            var message = string.Format("Hello {0},\n\nYour Air Bears account password was recently changed. ", user.FirstName);
            message += string.Format("If you did not authorize this change, please reply to this message so we can protect your account. ");
            message += string.Format("Simply ignore this message if you have authorized the change.");
            message += string.Format("\n\nThanks,\nAir Bears Team");

            await _mailer.SendAsync(user.Email, "Air Bears Password Changed", message);
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