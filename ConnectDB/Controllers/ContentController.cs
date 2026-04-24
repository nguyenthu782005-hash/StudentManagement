using ConnectDB.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ConnectDB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContentController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ContentController(AppDbContext context)
        {
            _context = context;
        }

        // --- BANNERS ---

        [HttpGet("banners")]
        public async Task<IActionResult> GetBanners()
        {
            return Ok(await _context.Banners.OrderByDescending(b => b.Priority).ToListAsync());
        }

        [HttpPost("banners")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> CreateBanner(Banner banner)
        {
            banner.CreatedAt = DateTime.Now;
            _context.Banners.Add(banner);
            await _context.SaveChangesAsync();
            return Ok(banner);
        }

        [HttpPut("banners/{id}")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> UpdateBanner(int id, Banner banner)
        {
            if (id != banner.Id) return BadRequest();

            _context.Entry(banner).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(banner);
        }

        [HttpDelete("banners/{id}")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> DeleteBanner(int id)
        {
            var banner = await _context.Banners.FindAsync(id);
            if (banner == null) return NotFound();

            _context.Banners.Remove(banner);
            await _context.SaveChangesAsync();
            return Ok();
        }

        // --- BLOGS ---

        [HttpGet("blogs")]
        public async Task<IActionResult> GetBlogs()
        {
            return Ok(await _context.BlogPosts.OrderByDescending(b => b.CreatedAt).ToListAsync());
        }

        [HttpPost("blogs")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> CreateBlog(BlogPost blog)
        {
            blog.CreatedAt = DateTime.Now;
            if (string.IsNullOrEmpty(blog.Slug))
            {
                blog.Slug = blog.Title.ToLower().Replace(" ", "-");
            }
            _context.BlogPosts.Add(blog);
            await _context.SaveChangesAsync();
            return Ok(blog);
        }

        [HttpPut("blogs/{id}")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> UpdateBlog(int id, BlogPost blog)
        {
            if (id != blog.Id) return BadRequest();

            _context.Entry(blog).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(blog);
        }

        [HttpDelete("blogs/{id}")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> DeleteBlog(int id)
        {
            var blog = await _context.BlogPosts.FindAsync(id);
            if (blog == null) return NotFound();

            _context.BlogPosts.Remove(blog);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
