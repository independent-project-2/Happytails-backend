using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HappyTailBackend.Models
{   
    [Table("blog")]
    public class Blog
    {
        public int Id { get; set; }

        
     
        [Required]
        public string banner { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        public DateTime created_at	 { get; set; } = DateTime.UtcNow;
        
        [Required]
        public int creater_id { get; set; } 
    }
}
