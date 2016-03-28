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
using Microsoft.Extensions.WebEncoders;
using Braintree;

namespace AirBears.Web.Controllers
{
    [Produces("application/json")]
    [Route("api/accounts")]
    public class AccountsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IGeocodeService _geocodeService;
        private readonly IMailer _mailer;
        private readonly IBraintreeGateway _gateway;

        public AccountsController(AppDbContext context, UserManager<User> userManager, IGeocodeService geocodeService, IMailer mailer, IBraintreeGateway gateway)
        {
            _context = context;
            _userManager = userManager;
            _geocodeService = geocodeService;
            _mailer = mailer;
            _gateway = gateway;
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
                ModelState.AddModelError(string.Empty, coords.Status.ToString());
                return HttpBadRequest(ModelState);
            }

            var request = new TransactionRequest
            {
                Amount = 25.00M,
                PaymentMethodNonce = model.Nonce,
                Options = new TransactionOptionsRequest
                {
                    SubmitForSettlement = true
                }
            };

            var transactionResult = _gateway.Transaction.Sale(request);

            if (!transactionResult.IsSuccess())
            {
                foreach (var error in transactionResult.Errors.All())
                {
                    ModelState.AddModelError(string.Empty, error.Message);
                    return HttpBadRequest(ModelState);
                }
            }

            var user = Mapper.Map<User>(model);
            user.Longitude = coords.Longitude;
            user.Latitude = coords.Latitude;
            user.GeocodeAddress = coords.GeocodeAddress;
            user.AllowsPilotSearch = true;
            user.SubscribesToUpdates = true;
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

        [AllowAnonymous]
        [HttpPost("forgot-password", Name = "Forgot Password")]
        public async Task<IActionResult> ForgotPassword([FromBody]ForgotPasswordViewModel model)
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

        [AllowAnonymous]
        [HttpPost("reset-password", Name = "Reset Password")]
        public async Task<IActionResult> ResetPassword([FromBody]ResetPasswordViewModel model)
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
                AddErrors(result);
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

        private async Task SendForgotPasswordEmail(User user, string code)
        {
            var callbackUrl = $"https://{Request.Host.Value}/reset-password?code={UrlEncoder.Default.UrlEncode(code)}";
            var message = string.Format("Hello {0},<br /><br />A request was submitted to reset your Air Bears password.", user.FirstName);
            message += string.Format("If this was not your doing, please reply to this message so we can protect your account.");
            message += string.Format("<a href='{0}' target='_blank'>Click here</a> to reset your password or browse to the address below.", callbackUrl);
            message += string.Format("<br /><br /><a href='{0}' target='_blank'>{1}</a>", callbackUrl, callbackUrl);
            message += string.Format("<br /><br />Thanks,\nAir Bears Team");

            await _mailer.SendAsync(user.Email, "Air Bears Password Recovery", message, true);
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