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
    [Route("api/authority-users")]
    [Authorize(AuthPolicies.Bearer)]
    public class AuthorityUsersController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IMailer _mailer;

        public AuthorityUsersController(AppDbContext context, UserManager<User> userManager, IMailer mailer)
        {
            _context = context;
            _userManager = userManager;
            _mailer = mailer;
        }

        // GET: api/users/5
        [HttpGet("{id}", Name = "Get Authority User")]
        [Authorize(AuthPolicies.Bearer, Roles = Roles.Admin)]
        public async Task<IActionResult> GetAuthorityUser([FromRoute] string id)
        {
            var user = await _context.Users.Where(u => !u.IsAuthorityAccount).FirstOrDefaultAsync();

            if (user == null)
            {
                return HttpNotFound();
            }

            return Ok(Mapper.Map<IdentityViewModel>(user));
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