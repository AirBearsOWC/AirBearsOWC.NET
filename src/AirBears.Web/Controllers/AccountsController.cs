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

        // POST: /api/accounts/registration
        [HttpPost("registration", Name = "Register User")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterUser([FromBody]RegistrationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            var user = new User
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                City = model.City,
                StateId = model.StateId,
                Street1 = model.Street1,
                Street2 = model.Street2,
                Zip = model.Zip,
                HasAgreedToTerms = model.HasAgreedToTerms,
                TeeShirtSizeId = model.TeeShirtSizeId
            };

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