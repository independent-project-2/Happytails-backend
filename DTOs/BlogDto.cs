using System.ComponentModel.DataAnnotations;

namespace HappyTailBackend.DTOs
{
    public class BlogDto
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }
    }
}
