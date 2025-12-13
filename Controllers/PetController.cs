using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using HappyTailBackend.Data;
using HappyTailBackend.Models;
using HappyTailBackend.DTOs;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;

namespace HappyTailBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PetController : ControllerBase
    {
        private readonly DataContext _context;

        public PetController(DataContext context)
        {
            _context = context;
        }

        [HttpGet("all")] // Get all pets
        public async Task<IActionResult> GetAll()
        {
            var pets = await _context.Pets.Include(p => p.Owner).ToListAsync();
            return Ok(pets);
        }

        [HttpGet("filter")] // Filter pets by type
        public async Task<IActionResult> Filter(string type)
        {
            var pets = await _context.Pets.Where(p => p.Type.ToLower() == type.ToLower()).ToListAsync();
            return Ok(pets);
        }

        [Authorize]
        [HttpPost("Create")] // Add pet (authenticated users only)
        public async Task<IActionResult> AddPet(PetDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var pet = new Pet
            {
                Name = dto.Name,
                Type = dto.Type,
                Age = dto.Age,
                OwnerId = userId
            };

            _context.Pets.Add(pet);
            await _context.SaveChangesAsync();

            return Ok(pet);
        }
    }
}
