using AirBears.Web.Models;
using AirBears.Web.Services;
using AirBears.Web.ViewModels;
using AutoMapper;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;
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

        public AccountsController(AppDbContext context, UserManager<User> userManager, SignInManager<User> signInManager, IGeocodeService geocodeService)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _geocodeService = geocodeService;
        }

        [HttpGet("lookup-address", Name = "Lookup Address")]
        [AllowAnonymous]
        public async Task<IActionResult> LookupAddress(string address)
        {
            var result = await _geocodeService.GetCoordsForAddress(address);

            if (result.Status != GeocodeResponseStatus.OK)
            {
                ModelState.AddModelError("", result.Status.ToString());
                return HttpBadRequest(ModelState);
            }

            return Ok(new { Longitude = result.Longitude, Latitude = result.Latitude });
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