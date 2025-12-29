using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using HappyTailBackend.Models;
using Microsoft.Extensions.Configuration;
using BCrypt.Net;


namespace HappyTailBackend.Services
{
    public class AuthService
    {
        private readonly string _jwtSecret;
        private readonly int _jwtLifespan; 

        public AuthService(IConfiguration configuration)
        {
            _jwtSecret = configuration["Jwt:Secret"] 
                ?? throw new Exception("JWT Secret missing");
            _jwtLifespan = int.Parse(configuration["Jwt:Lifespan"] ?? "10080"); 

            
        }

        // Hash password using BCrypt
        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        // Verify password
        public bool VerifyPassword(string password, string Password)
        {
            return BCrypt.Net.BCrypt.Verify(password, Password);
        }

        // Generate JWT token
        public string GenerateJwtToken(User user)
{
    var tokenHandler = new JwtSecurityTokenHandler();
    var key = Encoding.UTF8.GetBytes(_jwtSecret);
    var now = DateTime.UtcNow;

var tokenDescriptor = new SecurityTokenDescriptor
{
    Subject = new ClaimsIdentity(new[] {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Name, user.Name)
    }),
    NotBefore = now,
    Expires = now.AddMinutes(_jwtLifespan > 0 ? _jwtLifespan : 1), 
    SigningCredentials = new SigningCredentials(
        new SymmetricSecurityKey(key),
        SecurityAlgorithms.HmacSha256Signature
    )
};



    var token = tokenHandler.CreateToken(tokenDescriptor);
    var jwt = tokenHandler.WriteToken(token);
    Console.WriteLine($"Generated JWT Token: {jwt}");
    return tokenHandler.WriteToken(token);
}

    }
}
