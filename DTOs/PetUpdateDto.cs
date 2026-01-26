namespace HappyTails_backend.DTOs
{
    public class PetUpdateDto
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Breed { get; set; }
        public int? Age { get; set; }
        public string Location { get; set; }
        public decimal? Price { get; set; }
        public string Description { get; set; }
        public bool? Vaccinated { get; set; }
        public bool? Neutered { get; set; }
        public string Health_notes { get; set; }
        public string Status { get; set; }
    }
}
