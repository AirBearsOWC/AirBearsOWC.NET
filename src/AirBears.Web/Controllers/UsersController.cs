using AirBears.Web.Models;
using AirBears.Web.Services;
using AirBears.Web.ViewModels;
using AutoMapper;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;
using System;
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

        [HttpGet("/api/me", Name = "Get Current User")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var user = await _context.Users
                .Include(u => u.TeeShirtSize)
                .Include(u => u.State)
                .Include(u => u.FlightTime)
                .Include(u => u.Payload)
                .FirstOrDefaultAsync(u => u.Id == User.GetUserId());

            if (user.IsAuthorityAccount)
            {
                var authorityUser = Mapper.Map<IdentityViewModel>(user);
                authorityUser.Roles = User.GetRoles();

                // Return an authority user object if that is who is currently authenticated.
                return Ok(authorityUser);
            }

            var pilotUser = Mapper.Map<IdentityPilotViewModel>(user);
            pilotUser.Roles = User.GetRoles();

            return Ok(pilotUser);
        }

        /// <summary>
        /// Changes the current user's password.
        /// </summary>
        /// <returns></returns>
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

            await UpdateLastPasswordChangeDate(user);
            await SendChangePasswordEmail(user);

            return Ok();
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

        private async Task SendChangePasswordEmail(User user)
        {
            var message = string.Format("Hello {0},\n\nYour Air Bears account password was recently changed. ", user.FirstName);
            message += string.Format("If you did not authorize this change, please reply to this message so we can protect your account. ");
            message += string.Format("Simply ignore this message if you have authorized the change.");
            message += string.Format("\n\nThanks,\nAir Bears Team");

            await _mailer.SendAsync(user.Email, "Air Bears Password Changed", message);
        }

        private async Task UpdateLastPasswordChangeDate(User user)
        {
            user.LastPasswordChangeDate = DateTime.UtcNow;
            _context.Users.Update(user, GraphBehavior.SingleObject);

            await _context.SaveChangesAsync();
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