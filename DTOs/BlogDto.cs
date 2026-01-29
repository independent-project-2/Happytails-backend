using System.ComponentModel.DataAnnotations;

namespace HappyTailBackend.DTOs
{
    public class BlogDto
    {

        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public string Banner { get; set; }
        
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


    }
}
