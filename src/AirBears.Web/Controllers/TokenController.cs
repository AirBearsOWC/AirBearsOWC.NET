using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using AirBears.Web.Models;
using Microsoft.AspNet.Identity;
using AirBears.Web.ViewModels;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System;

namespace AirBears.Web.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class TokenController : Controller
    {
        private readonly TokenAuthOptions _tokenOptions;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public TokenController(TokenAuthOptions tokenOptions, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _tokenOptions = tokenOptions;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        /// <summary>
        /// Request a new token for a given username/password combiniation.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AuthenticateUser([FromBody] LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            //var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, lockoutOnFailure: false);
            var user = await _userManager.FindByNameAsync(model.Email);

            if (await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var expires = DateTime.UtcNow.AddDays(5);
                var token = await GetToken(model.Email, expires);

                return Ok(new { authenticated = true, token = token, tokenExpires = expires });
            }

            return HttpBadRequest("Invalid login attempt.");
        }

        private async Task<string> GetToken(string username, DateTime? expires)
        {
            var handler = new JwtSecurityTokenHandler();
            var user = await _userManager.FindByNameAsync(username);

            var identity = User.Identity as ClaimsIdentity;
            var roles = await _userManager.GetRolesAsync(user);

            foreach (var r in roles) identity.AddClaim(new Claim(ClaimTypes.Role, r));

            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id));
            identity.AddClaim(new Claim(ClaimTypes.Name, user.UserName));
            identity.AddClaim(new Claim(ClaimTypes.GivenName, user.FirstName));
            identity.AddClaim(new Claim(ClaimTypes.Surname, user.LastName));

            var securityToken = handler.CreateToken(
                issuer: _tokenOptions.Issuer,
                audience: _tokenOptions.Audience,
                signingCredentials: _tokenOptions.SigningCredentials,
                subject: identity,
                expires: expires
                );

            return handler.WriteToken(securityToken);
        }
    }
}