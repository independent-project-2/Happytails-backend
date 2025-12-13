using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HappyTailBackend.Models
{
    public class Pet
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Type { get; set; } 

        public string Breed { get; set; }

        public int Age { get; set; }

        // Foreign key to User
        [Required]
        public int OwnerId { get; set; }

        [ForeignKey("OwnerId")]
        public User Owner { get; set; }

        [Required]
        public string Location { get; set; }

        [Required]
        public string Description { get; set; }

        public List<string> Photos { get; set; }

        
        public VaccinationRecords VaccinationRecords { get; set; } = new VaccinationRecords();
    
    }


    

    public class VaccinationRecords
    {
        public int Id { get; set; }

        [Required]
        public int PetId { get; set; }

        [ForeignKey("PetId")]
        public Pet Pet { get; set; }

        public bool Vaccinated { get; set; }
        public bool Neutered { get; set; }
        public bool Dewormed { get; set; }
        public string AdditionalNotes { get; set; }
    }
}


