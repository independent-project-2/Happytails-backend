using Microsoft.AspNetCore.Mvc;
using HappyTailBackend.Data;
using HappyTailBackend.DTOs;
using HappyTailBackend.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;
using HappyTails_backend.DTOs;


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

        // Update Pet
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdatePet(int id, [FromBody] PetUpdateDto dto)
        {
            var authHeader = Request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                return Unauthorized(new { message = "Token missing" });

            var token = authHeader.Substring("Bearer ".Length).Trim();

            int shelterId;
            try
            {
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

                var principal = tokenHandler.ValidateToken(token, validationParameters, out _);

                var shelterIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (shelterIdClaim == null || !int.TryParse(shelterIdClaim, out shelterId))
                    return Unauthorized(new { message = "Invalid token" });
            }
            catch
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var pet = await _context.Pets.FindAsync(id);

            if (pet == null)
            {
                return NotFound(new { message = "Pet not found" });
            }

            if (pet.Shelter_id != shelterId)
            {
                return Forbid("You are not allowed to update this pet");
            }

            if (dto.Name != null) pet.Name = dto.Name;
            if (dto.Type != null) pet.Type = dto.Type;
            if (dto.Breed != null) pet.Breed = dto.Breed;
            if (dto.Age.HasValue) pet.Age = dto.Age.Value;
            if (dto.Location != null) pet.Location = dto.Location;
            if (dto.Price.HasValue) pet.Price = dto.Price.Value;
            if (dto.Description != null) pet.Description = dto.Description;
            if (dto.Vaccinated.HasValue) pet.Vaccinated = dto.Vaccinated.Value;
            if (dto.Neutered.HasValue) pet.Neutered = dto.Neutered.Value;
            if (dto.Health_notes != null) pet.Health_notes = dto.Health_notes;
            if (dto.Status != null) pet.Status = dto.Status;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Pet updated successfully 🐾",
                pet
            });




        }
    }
}
