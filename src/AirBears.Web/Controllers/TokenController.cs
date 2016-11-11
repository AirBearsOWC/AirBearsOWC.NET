using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AirBears.Web.Models;
using Microsoft.AspNetCore.Identity;
using AirBears.Web.ViewModels;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System;
using Braintree;
using Microsoft.IdentityModel.Tokens;

namespace AirBears.Web.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class TokenController : Controller
    {
        private readonly TokenAuthOptions _tokenOptions;
        private readonly UserManager<User> _userManager;
        private readonly AppDbContext _context;
        private readonly IBraintreeGateway _gateway;

        public TokenController(TokenAuthOptions tokenOptions, UserManager<User> userManager, AppDbContext context, IBraintreeGateway gateway)
        {
            _tokenOptions = tokenOptions;
            _userManager = userManager;
            _context = context;
            _gateway = gateway;
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
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByNameAsync(model.Email);

            if (await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var expires = DateTime.UtcNow.AddDays(5);
                var token = await GetToken(model.Email, expires);

                await UpdateLastLoginDate(user);

                return Ok(new { authenticated = true, token = token, tokenExpires = expires });
            }

            return BadRequest("Invalid login attempt.");
        }

        [HttpGet("/api/payment-token", Name = "Get Payment Token")]
        public async Task<IActionResult> GetPaymentToken()
        {
            var clientToken = await Task.Run(() => _gateway.ClientToken.generate());

            return Ok(new { token = clientToken });
        }

        private async Task UpdateLastLoginDate(User user)
        {
            user.LastLoginDate = DateTime.UtcNow;
            _context.Users.Update(user);

           await  _context.SaveChangesAsync();
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

            var securityToken = handler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = _tokenOptions.Issuer,
                Audience = _tokenOptions.Audience,
                SigningCredentials = _tokenOptions.SigningCredentials,
                Subject = identity,
                Expires = expires
            });

            return handler.WriteToken(securityToken);
        }
    }
}