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
        private readonly IMapper _mapper;

        public AuthorityUsersController(AppDbContext context, UserManager<User> userManager, IMapper mapper, IMailer mailer)
        {
            _context = context;
            _userManager = userManager;
            _mapper = mapper;
            _mailer = mailer;
        }

        [HttpGet("{id}", Name = "Get Authority User")]
        [Authorize(AuthPolicies.Bearer, Roles = Roles.Admin)]
        public async Task<IActionResult> GetAuthorityUser([FromRoute] string id)
        {
            var user = await _context.Users.Where(u => u.Id == id && u.IsAuthorityAccount).FirstOrDefaultAsync();

            if (user == null)
            {
                return HttpNotFound();
            }

            var resp = _mapper.Map<IdentityViewModel>(user);

            resp.Roles = await _userManager.GetRolesAsync(user);

            return Ok(resp);
        }

        [HttpGet(Name = "Get Authority Users")]
        [Authorize(AuthPolicies.Bearer, Roles = Roles.Admin)]
        public async Task<QueryResult<IdentityViewModel>> GetAuthorityUsers(string name, string sortBy, bool ascending = true, bool onlyUnapproved = false, int page = 1, int? pageSize = 50)
        {
            var query = _context.Users.Include(u => u.Roles).Where(u => u.IsAuthorityAccount);
            var roles = await _context.Roles.ToListAsync();
            var resp = new List<IdentityViewModel>();

            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(u =>
                            u.FirstName.ToLower().Contains(name.ToLower()) ||
                            u.LastName.ToLower().Contains(name.ToLower()) ||
                            u.Email.ToLower().Contains(name.ToLower()));
            }

            if (onlyUnapproved) { query = query.Where(u => u.Roles.Count == 0); } // Unapproved authorities have no roles.

            if (ascending)
                query = query.OrderBy(Static.GetSortExpression(sortBy));
            else
                query = query.OrderByDescending(Static.GetSortExpression(sortBy));            

            // Limit the page size to 200.
            if (!pageSize.HasValue) { pageSize = 200; }

            var users = await query.Skip((page - 1) * pageSize.Value).Take(pageSize.Value).ToListAsync();

            foreach (var u in users)
            {
                var user = _mapper.Map<IdentityViewModel>(u);

                // manually align the roles in memory.
                user.Roles = roles.Where(r => u.Roles.Select(x => x.RoleId).Contains(r.Id)).Select(r => r.Name).ToList();
                resp.Add(user);
            }

            var result = new QueryResult<IdentityViewModel>()
            {
                Items = resp,
                Page = page,
                PageSize = pageSize.Value,
                TotalCount = users.Count()
            };

            return result;
        }

        // PUT: /api/authority-users/5/approve
        [HttpPut("{id}/approve", Name = "Approve Authority")]
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

            var resp = _mapper.Map<IdentityViewModel>(user);
            resp.Roles = await _userManager.GetRolesAsync(user);

            return Ok(resp);
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