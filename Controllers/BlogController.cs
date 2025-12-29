using Microsoft.AspNetCore.Mvc;
using HappyTailBackend.Data;
using HappyTailBackend.DTOs;
using HappyTailBackend.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Xml;
using System.ComponentModel;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Security.AccessControl;
using System.Net;

namespace HappyTailBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BlogController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly string _jwtSecret;

        public BlogController(DataContext context, IConfiguration configuration)
        {
            _context = context;
            _jwtSecret = configuration["Jwt:Secret"] 
                ?? throw new Exception("JWT Secret missing");
        }

        // Protected: create blog
        [HttpPost("create")]
        public async Task<IActionResult> AddBlog([FromBody] BlogDto dto)
        {
          
            var authHeader = Request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                return Unauthorized(new { message = "Token missing" });

            var token = authHeader.Substring("Bearer ".Length).Trim();

            int Createrid;
            try
            {
                //  Validate JWT token
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_jwtSecret);

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

                //  Extract user ID
                var createrIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (createrIdClaim == null || !int.TryParse(createrIdClaim, out Createrid))
                    return Unauthorized(new { message = "Invalid token or user not found" });

                Console.WriteLine($"Token valid, shelterId: {Createrid}");
            }
            catch
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            //  Create new blog
            var blog = new Blog
            {
                Title = dto.Title,
                Content = dto.Content,
                banner = dto.Banner,
                created_at = dto.CreatedAt
                ,creater_id = Createrid

                
            };

            _context.Blogs.Add(blog);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Blog created successfully 📝",
                blog
            });
        }







        

        // Public: get all blogs
        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var blogs = await _context.Blogs.ToListAsync();
            return Ok(blogs);
        }

        
    }
}
