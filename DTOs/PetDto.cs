namespace HappyTailBackend.DTOs
{
    public class PetDto
    {
        public string Name { get; set; } = null!;
        public string Type { get; set; } = null!;
        public string? Breed { get; set; }
        public int Age { get; set; }
        public string Location { get; set; } = null!;
        public decimal Price { get; set; }
        public string? Description { get; set; }
        public bool Vaccinated { get; set; }
        public bool Neutered { get; set; }
        public string? Health_notes { get; set; }
        public string Status { get; set; } = "AVAILABLE";
        public int Shelter_id { get; set; }
        public DateTime Created_at { get; set; } = DateTime.UtcNow;
        public IFormFile? Image { get; set; }
    }
}
