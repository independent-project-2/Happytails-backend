using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HappyTailBackend.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        
    
        public string Name { get; set; }
        
       
        public string? Profile_picture { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        // Navigation property
        public List<Pet> Pets { get; set; }
    }
}
