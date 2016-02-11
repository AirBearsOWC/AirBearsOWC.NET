using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using AirBears.Web.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Authorization;
using Microsoft.Data.Entity;
using AirBears.Web.ViewModels;
using AutoMapper;

namespace AirBears.Web.Controllers
{
    [Produces("application/json")]
    [Route("api/accounts")]
    public class AccountsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;


        public AccountsController(AppDbContext context, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // POST: /api/accounts/pilot-registration
        [HttpPost("pilot-registration", Name = "Register Pilot")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterUser([FromBody]PilotRegistrationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            var user = Mapper.Map<User>(model);
            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                AddErrors(result);
                return HttpBadRequest(ModelState);
            }

            await _signInManager.SignInAsync(user, isPersistent: false);

            var responseUser = await _context.Users
                .Include(u => u.TeeShirtSize)
                .Include(u => u.State)
                .SingleAsync(u => u.UserName == model.Email);

            return Ok(Mapper.Map<UserViewModel>(responseUser));
        }

        // POST: /api/accounts/authority-registration
        [HttpPost("authority-registration", Name = "Register Authority")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterAuthority([FromBody]AuthorityRegistrationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            var user = Mapper.Map<User>(model);

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                AddErrors(result);
                return HttpBadRequest(ModelState);
            }

            await _signInManager.SignInAsync(user, isPersistent: false);

            var responseUser = await _context.Users.SingleAsync(u => u.UserName == model.Email);

            return Ok(Mapper.Map<UserViewModel>(responseUser));
        }

        // POST: /api/accounts/authority-approval
        [HttpPost("authority-approval", Name = "Approve Authority")]
        [Authorize(AuthPolicies.Bearer, Roles = Roles.Admin)]
        public async Task<IActionResult> ApproveAuthority([FromBody]string username)
        {
            var user = await _userManager.FindByEmailAsync(username);

            if (user == null)
            {
                return HttpNotFound();
            }

            if(await _userManager.IsInRoleAsync(user, Roles.Authority))
            {
                return HttpBadRequest($"{username} already has the {Roles.Authority} role!");
            }

            await _userManager.AddToRoleAsync(user, Roles.Authority);

            return Ok();
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
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