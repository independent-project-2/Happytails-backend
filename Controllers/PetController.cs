using Microsoft.AspNetCore.Mvc;
using HappyTailBackend.Data;
using HappyTailBackend.DTOs;
using HappyTailBackend.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;


namespace HappyTailBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PetController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly string _jwtSecret;

        public PetController(DataContext context, IConfiguration configuration)
        {
            _context = context;
            _jwtSecret = configuration["Jwt:Secret"] 
                ?? throw new Exception("JWT Secret missing");
        }




        //  Public: Get all pets
        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
           var pets = await _context.Pets
    .Include(p => p.Shelter)
    .Select(p => new PetResponseDto
    {
        Id = p.Id,
        Name = p.Name,
        Type = p.Type,
        Breed = p.Breed,
        Age = p.Age,
        Price = p.Price,
        Vaccinated = p.Vaccinated,
        Neutered = p.Neutered,
        Status = p.Status,
        Shelter = new ShelterDto
        {
            Id = p.Shelter.Id,
            Name = p.Shelter.Name,
            Email = p.Shelter.Email
        }
    })
    .ToListAsync();
            return Ok(pets);

        }



        // Public: Filter pets by type
[HttpGet("filter")]
public async Task<IActionResult> Filter([FromQuery] string type)
{
    if (string.IsNullOrWhiteSpace(type))
        return BadRequest("Type query parameter is required");

    var pets = await _context.Pets
        .Where(p => p.Type != null && p.Type.ToLower() == type.ToLower())
        .ToListAsync();

    if (pets.Count == 0)
        return NotFound($"No pets found with type '{type}'");

    return Ok(pets);
}


        // Protected: Add pet manually with JWT
        [HttpPost("create")]
        public async Task<IActionResult> AddPet([FromBody] PetDto dto)
        {
            //  Read Authorization header
            var authHeader = Request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                return Unauthorized(new { message = "Token missing" });

            var token = authHeader.Substring("Bearer ".Length).Trim();

            int shelterId;
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
                var shelterIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (shelterIdClaim == null || !int.TryParse(shelterIdClaim, out shelterId))
                    return Unauthorized(new { message = "Invalid token or user not found" });

                Console.WriteLine($"Token valid, shelterId: {shelterId}");
            }
            catch
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            //  Create new pet
            var pet = new Pet
            {
                Name = dto.Name,
                Type = dto.Type,
                Breed = dto.Breed,
                Age = dto.Age,
                Location = dto.Location,
                Price = dto.Price,
                Description = dto.Description,
                Vaccinated = dto.Vaccinated,
                Neutered = dto.Neutered,
                Health_notes = dto.Health_notes,
                Status = "AVAILABLE",
                Shelter_id = shelterId, 
                Created_at = DateTime.UtcNow
            };

            _context.Pets.Add(pet);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Pet added successfully 🐾",
                pet
            });
        }
    }
}
