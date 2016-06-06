using AirBears.Web.Models;
using AirBears.Web.ViewModels;
using AutoMapper;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        [HttpGet("{id:guid}", Name = "Get Post By ID")]
        public async Task<IActionResult> GetPostById([FromRoute] Guid id)
        {
            var post = await _context.Posts.Where(p => p.Id == id).FirstOrDefaultAsync();

            if (post == null)
            {
                return HttpNotFound();
            }

            return Ok(_mapper.Map<PostViewModel>(post));
        }

        [HttpGet("{slug:string}", Name = "Get Post By Slug")]
        public async Task<IActionResult> GetPostBySlug([FromRoute] string slug)
        {
            var post = await _context.Posts.Where(p => p.Slug == slug.ToLower()).FirstOrDefaultAsync();

            if (post == null)
            {
                return HttpNotFound();
            }

            return Ok(_mapper.Map<PostViewModel>(post));
        }

        [HttpGet(Name = "Get Posts")]
        public async Task<IActionResult> GetPosts()
        {
            var posts = await _context.Posts.OrderByDescending(p => p.DatePublished).ToListAsync();

            return Ok(_mapper.Map<List<PostViewModel>>(posts));
        }

        [HttpPost(Name = "Create Post")]
        [Authorize(AuthPolicies.Bearer, Roles = Roles.Admin)]
        public async Task<IActionResult> CreatePost([FromBody] PostViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            var post = _mapper.Map<Post>(model);

            post.DateUpdated = DateTime.UtcNow;

            _context.Add(post);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPut(Name = "Update Post")]
        [Authorize(AuthPolicies.Bearer, Roles = Roles.Admin)]
        public async Task<IActionResult> UpdatePost([FromBody] PostViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            var post = await _context.Posts.Where(p => p.Id == model.Id).SingleOrDefaultAsync();

            if(post == null)
            {
                return HttpNotFound();
            }

            _mapper.Map(model, post);
            post.DateUpdated = DateTime.UtcNow;

            _context.Posts.Update(post);
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