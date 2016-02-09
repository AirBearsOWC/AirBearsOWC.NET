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

            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

            // Obviously, at this point you need to validate the username and password against whatever system you wish.
            if (result.Succeeded)
            {
                DateTime? expires = DateTime.UtcNow.AddDays(5);
                var token = GetToken(expires);

                //return new { authenticated = true, entityId = 1, token = token, tokenExpires = expires };

                return Ok(new { authenticated = true, token = token, tokenExpires = expires });
            }

            return HttpBadRequest("Invalid login attempt.");
        }

        private async Task<string> GetToken(DateTime? expires)
        {
            var handler = new JwtSecurityTokenHandler();
            var user = await _userManager.FindByIdAsync(User.GetUserId());

            var identity = User.Identity as ClaimsIdentity;
            var roles = await _userManager.GetRolesAsync(user);

            foreach (var r in roles) identity.AddClaim(new Claim(ClaimTypes.Role, r));

            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.UserName));
            identity.AddClaim(new Claim(ClaimTypes.GivenName, user.FirstName));
            identity.AddClaim(new Claim(ClaimTypes.Surname, user.LastName));

            // Here, you should create or look up an identity for the user which is being authenticated.
            // For now, just creating a simple generic identity.

            //var identity = new ClaimsIdentity(new GenericIdentity(user, "TokenAuth"), new[] { new Claim("EntityID", "1", ClaimValueTypes.Integer) });

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