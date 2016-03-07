using AirBears.Web.Models;
using AirBears.Web.Services;
using AirBears.Web.ViewModels;
using AutoMapper;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;
using System;
using System.Threading.Tasks;

namespace AirBears.Web.Controllers
{
    [Produces("application/json")]
    [Route("api/accounts")]
    public class AccountsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IGeocodeService _geocodeService;
        private readonly IMailer _mailer;

        public AccountsController(AppDbContext context, UserManager<User> userManager, SignInManager<User> signInManager, IGeocodeService geocodeService, IMailer mailer)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _geocodeService = geocodeService;
            _mailer = mailer;
        }

        // POST: /api/accounts/pilot-registration
        [HttpPost("pilot-registration", Name = "Register Pilot")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterPilot([FromBody]PilotRegistrationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            var state = await _context.States.FirstOrDefaultAsync(s => s.Id == model.StateId.Value);

            if (state == null)
            {
                ModelState.AddModelError("StateId", "A state with that ID does not exist.");
                return HttpBadRequest(ModelState);
            }

            var coords = await _geocodeService.GetCoordsForAddress(model.GetAddress(state.Name));

            if (coords.Status != GeocodeResponseStatus.OK)
            {
                ModelState.AddModelError("", coords.Status.ToString());
                return HttpBadRequest(ModelState);
            }

            var user = Mapper.Map<User>(model);
            user.Longitude = coords.Longitude;
            user.Latitude = coords.Latitude;
            user.GeocodeAddress = coords.GeocodeAddress;
            user.DateRegistered = DateTime.UtcNow;

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                AddErrors(result);
                return HttpBadRequest(ModelState);
            }

            await SendPilotWelcomeEmail(user);

            var responseUser = await _context.Users
                .Include(u => u.TeeShirtSize)
                .Include(u => u.State)
                .SingleAsync(u => u.UserName == model.Email);

            return Ok(Mapper.Map<UserViewModel>(responseUser));
        }

        private async Task SendPilotWelcomeEmail(User user)
        {
            var message = $"Congratulations {user.FirstName}!\n\n"
                + "Thanks you for joining the team. Your exclusive volunteer pilot T-shirt will arrive shortly. The shirt is special. It is not for sale, "
                + "and will eventually grant you access to restricted areas like fire and crime scenes. "
                + "Feel free to wear it as much as you'd like to help spread the word and raise awareness of our cause. "
                + "Note that it must be worn if or when you decide to answer a call for help. "
                + "\n\nWe look forward to making a difference with your help,\nAir Bears";

            await _mailer.SendAsync(user.Email, "Welcome to Air Bears", message);
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
            user.DateRegistered = DateTime.UtcNow;

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                AddErrors(result);
                return HttpBadRequest(ModelState);
            }

            var responseUser = await _context.Users.SingleAsync(u => u.UserName == model.Email);

            return Ok(Mapper.Map<UserViewModel>(responseUser));
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("api/accounts/forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            var user = await _userManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user doen't exist.
                return HttpBadRequest();
            }

            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            await SendForgotPasswordEmail(user, code);

            return Ok();
        }

        private async Task SendForgotPasswordEmail(User user, string code)
        {
            var callbackUrl = Url.Action("ResetPassword", "Accounts", new { code = code }, protocol: HttpContext.Request.Scheme);
            var message = string.Format("Hello {0},\n\nA request was submitted to reset your Air Bears password.", user.FirstName);
            message += string.Format("If this was not your doing, please reply to this message so we can protect your account.");
            message += string.Format("<a href='{0}' target='_blank'>Click here</a> to reset your password or browse to the address below.", callbackUrl);
            message += string.Format("\n\n<a href='{0}' target='_blank'>{1}</a>", callbackUrl, callbackUrl);
            message += string.Format("\n\nThanks,\nAir Bears Team");

            await _mailer.SendAsync(user.Email, "Air Bears Password Recovery", message);
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("api/accounts/reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            var user = await _userManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                return HttpBadRequest();
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.NewPassword);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return HttpBadRequest(ModelState);
            }

            await SendResetPasswordEmail(user);

            return Ok();
        }

        private async Task SendResetPasswordEmail(User user)
        {
            var message = string.Format("Hello {0},\n\nYour Air Bears account password was recently reset. ", user.FirstName);
            message += string.Format("If you did not authorize this reset, please reply to this message so we can protect your account. ");
            message += string.Format("Simply ignore this message if you have authorized the password reset.");
            message += string.Format("\n\nThanks,\nAir Bears Team");

            await _mailer.SendAsync(user.Email, "Air Bears Password Reset", message);
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