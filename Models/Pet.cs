using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HappyTailBackend.Models
{
    public class Pet
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        [Required]
        public string Type { get; set; } = null!; // Dog, Cat

        public string? Breed { get; set; }

        public int Age { get; set; }

        [Required]
        public string Location { get; set; } = null!;

        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }

        public string? Description { get; set; }

        public bool Vaccinated { get; set; }

        public bool Neutered { get; set; }

        public string? Health_notes { get; set; }

        [Required]
        public string Status { get; set; } = "AVAILABLE";

        // FK → users.id
        [ForeignKey("Shelter")]
        public int Shelter_id { get; set; }

        public User Shelter { get; set; } = null!;
        public string? Image { get; set; }


        public DateTime Created_at { get; set; } = DateTime.UtcNow;
    }
}
