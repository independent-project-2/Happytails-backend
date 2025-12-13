using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HappyTailBackend.Data;
using HappyTailBackend.Models;
using HappyTailBackend.DTOs;
using System.Threading.Tasks;
using HappyTailBackend.Services;


namespace HappyTailBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController(DataContext context, AuthService authService) : ControllerBase
    {
        private readonly DataContext _context = context;
        private readonly AuthService _authService = authService;

        [HttpPost("signup")]
        public async Task<IActionResult> Signup(RegisterDto dto)
        {
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
                return BadRequest("Email already in use");

            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = _authService.HashPassword(dto.Password)
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var token = _authService.GenerateJwtToken(user);

            return Ok(new { token });
        }

        [HttpPost("signin")]
        public async Task<IActionResult> Signin(LoginDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null || !_authService.VerifyPassword(dto.Password, user.PasswordHash))
                return Unauthorized("Invalid credentials");

            var token = _authService.GenerateJwtToken(user);
            return Ok(new { token });
        }
    }
}
