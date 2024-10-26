using ApecMovieCore.Common;
using BlogServices.Common;
using BlogServices.Models;
using BlogServices.Models.DTO;
using BlogServices.Models.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly Genarater _genarater;

        public BlogController(ApplicationDbContext context, Genarater genarater)
        {
            _context = context;
            _genarater = genarater;
        }

        // GET: api/blog
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BlogPost>>> GetBlogs(string? authorName = null, bool isActiveOnly = true)
        {
            var query = _context.BlogPosts.AsQueryable();
            // sử dụng custiomize query trong Common
            query = query
                .WhereIf(!string.IsNullOrEmpty(authorName), b => b.AuthorName.Contains(authorName))
                .WhereIf(isActiveOnly, b => b.IsActive);

            return await query.ToListAsync();
        }

        // GET: api/blog/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<BlogPost>> GetBlog(Guid id)
        {
            var blog = await _context.BlogPosts.FindAsync(id);

            if (blog == null)
            {
                return NotFound();
            }

            return blog;
        }

        [HttpPost]
        public async Task<ActionResult<BlogPost>> CreateBlog(BlogCreateDto blogCreateDto)
        {
            var blog = new BlogPost
            {
                Id = Guid.NewGuid(),
                Title = blogCreateDto.Title,
                Description = blogCreateDto.Description,
                Content = blogCreateDto.Content,
                AuthorName = blogCreateDto.AuthorName,
                Slug = _genarater.GenerateUniqueSlug<BlogPost>(blogCreateDto.Title, b => b.Slug),
                ViewCount = 0,
                IsActive = true
            };
            _context.BlogPosts.Add(blog);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBlog), new { id = blog.Id }, blog);
        }


        // PUT: api/blog/{id}
        //[HttpPut("{id}")]
        //public async Task<IActionResult> UpdateBlog(Guid id, BlogPost blog)
        //{
        //    if (id != blog.Id)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(blog).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!BlogExists(id))
        //        {
        //            return NotFound();
        //        }
        //        throw;
        //    }

        //    return NoContent();
        //}

        // DELETE: api/blog/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBlog(Guid id)
        {
            var blog = await _context.BlogPosts.FindAsync(id);
            if (blog == null)
            {
                return NotFound();
            }

            _context.BlogPosts.Remove(blog);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BlogExists(Guid id)
        {
            return _context.BlogPosts.Any(e => e.Id == id);
        }
    }
}
