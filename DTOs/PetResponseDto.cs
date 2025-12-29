public class PetResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Type { get; set; } = null!;
    public string? Breed { get; set; }
    public int Age { get; set; }
    public decimal Price { get; set; }
    public bool Vaccinated { get; set; }
    public bool Neutered { get; set; }
    public string Status { get; set; } = "AVAILABLE";
    public ShelterDto Shelter { get; set; } = null!;
}

public class ShelterDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Email { get; set; }
}
