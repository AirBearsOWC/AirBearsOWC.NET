using AirBears.Web.Models;
using AirBears.Web.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Security.Claims;

namespace AirBears.Web.Controllers
{
    [Produces("application/json")]
    [Route("api/posts")]
    public class PostsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public PostsController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [Authorize(AuthPolicies.Bearer, Roles = Roles.Admin)]
        [HttpGet("{id:guid}", Name = "Get Post By ID")]
        public async Task<IActionResult> GetPostById([FromRoute] Guid id)
        {
            var post = await _context.Posts.Where(p => p.Id == id).FirstOrDefaultAsync();

            if (post == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<PostViewModel>(post));
        }

        [HttpGet("{slug}", Name = "Get Post By Slug")]
        public async Task<IActionResult> GetPostBySlug([FromRoute] string slug)
        {
            var post = await _context.Posts.ThatArePublished().Where(p => p.Slug == slug.ToLower()).FirstOrDefaultAsync();

            if (post == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<PostViewModel>(post));
        }

        [HttpGet(Name = "Get Posts")]
        [Authorize(AuthPolicies.Bearer)]
        [AllowAnonymous]
        public async Task<IActionResult> GetPosts(int pageSize = 50, bool includeDrafts = false)
        {
            pageSize = Math.Min(pageSize, 100); // restrict size to 100.

            if (includeDrafts && !User.IsInRole(Roles.Admin))
            {
                // If they've requested drafts, they must be administrators. Send 403 if they're not.
                return new StatusCodeResult((int)HttpStatusCode.Forbidden);
            }

            var posts = (includeDrafts)
                 ? await _context.Posts.OrderByDescending(p => p.DatePublished).Take(pageSize).ToListAsync()
                 : await _context.Posts.ThatArePublished().OrderByDescending(p => p.DatePublished).Take(pageSize).ToListAsync();

            foreach(var post in posts.Where(p => !string.IsNullOrWhiteSpace(p.Content)))
            {
                post.Content = post.Content.StripHtml();
                if (!string.IsNullOrWhiteSpace(post.Content) && post.Content.Length > 500)
                    post.Content = post.Content.Substring(0, 500);
            }

            return Ok(_mapper.Map<List<PostViewModel>>(posts));
        }

        [HttpPost(Name = "Create Post")]
        [Authorize(AuthPolicies.Bearer, Roles = Roles.Admin)]
        public async Task<IActionResult> CreatePost([FromBody] PostViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            model.Id = Guid.NewGuid();
            model.Slug = model.Slug.ToLower();
            model.DateUpdated = DateTime.UtcNow;

            var post = _mapper.Map<Post>(model);

            _context.Add(post);
            await _context.SaveChangesAsync();

            return Ok(model);
        }

        [HttpPut(Name = "Update Post")]
        [Authorize(AuthPolicies.Bearer, Roles = Roles.Admin)]
        public async Task<IActionResult> UpdatePost([FromBody] PostViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var post = await _context.Posts.Where(p => p.Id == model.Id).SingleOrDefaultAsync();

            if(post == null)
            {
                return NotFound();
            }

            model.Slug = model.Slug.ToLower();
            model.DateUpdated = DateTime.UtcNow;

            _mapper.Map(model, post);

            _context.Posts.Update(post);
            await _context.SaveChangesAsync();

            return Ok(model);
        }

        [HttpDelete("{id:guid}", Name = "Delete Post")]
        [Authorize(AuthPolicies.Bearer, Roles = Roles.Admin)]
        public async Task<IActionResult> DeletePost([FromRoute] Guid id)
        {
            var post = await _context.Posts.Where(p => p.Id == id).SingleOrDefaultAsync();

            if (post == null)
            {
                return NotFound();
            }

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            return Ok();
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