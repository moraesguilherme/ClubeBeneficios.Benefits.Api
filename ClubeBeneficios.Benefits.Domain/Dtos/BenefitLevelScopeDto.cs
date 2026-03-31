namespace ClubeBeneficios.Benefits.Domain.Dtos;

public class BenefitLevelScopeDto
{
    public string? LevelType { get; set; }
    public List<string> LevelCodes { get; set; } = new();
    public string? Summary { get; set; }
}
