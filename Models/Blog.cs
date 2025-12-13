using System;
using System.ComponentModel.DataAnnotations;

namespace HappyTailBackend.Models
{
    public class Blog
    {
        public int Id { get; set; }

        [Required]
        public string BannerImage { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
