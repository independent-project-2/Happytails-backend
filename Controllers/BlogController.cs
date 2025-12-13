using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HappyTailBackend.Data;
using HappyTailBackend.Models;
using HappyTailBackend.DTOs;
using System.Threading.Tasks;
using System;



namespace HappyTailBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BlogController : ControllerBase
    {
        private readonly DataContext _context;

        [HttpGet("all")] // Get all blogs
        public async Task<IActionResult> GetAll()
        {
            var blogs = await _context.Blogs
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
            return Ok(blogs);
        }

        [HttpPost("create")] // Add blog (optional: can require auth)
        public async Task<IActionResult> AddBlog(BlogDto dto)
        {
            var blog = new Blog
            {
                Title = dto.Title,
                Content = dto.Content,
                CreatedAt = DateTime.UtcNow
            };

            _context.Blogs.Add(blog);
            await _context.SaveChangesAsync();

            return Ok(blog);
        }
    }
}
