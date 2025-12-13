using System.ComponentModel.DataAnnotations;

namespace HappyTailBackend.DTOs
{
    public class PetDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Type { get; set; } // e.g., Dog, Cat

        [Required]
        public int Age { get; set; }
    }
}
