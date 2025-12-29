using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HappyTailBackend.Data;
using HappyTailBackend.Models;
using HappyTailBackend.DTOs;
using System.Threading.Tasks;
using HappyTailBackend.Services;
using Microsoft.AspNetCore.Authorization;


namespace HappyTailBackend.Controllers
{    
    [ApiController]
     [AllowAnonymous]
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
                Name = dto.Name,
                Email = dto.Email,
                Password = _authService.HashPassword(dto.Password)
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var token = _authService.GenerateJwtToken(user);

            return Ok(new{ message = "Registration successful",token});
        }




        [HttpPost("signin")]
        public async Task<IActionResult> Signin(LoginDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null || !_authService.VerifyPassword(dto.Password, user.Password))
                return Unauthorized("please check your email and password");

            var token = _authService.GenerateJwtToken(user);
            return Ok(new { token });
        }
    }
}
