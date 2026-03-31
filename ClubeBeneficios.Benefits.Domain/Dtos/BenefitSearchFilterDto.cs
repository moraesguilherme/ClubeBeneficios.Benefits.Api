namespace ClubeBeneficios.Benefits.Domain.Dtos;

public class BenefitSearchFilterDto
{
    public string? Search { get; set; }
    public Guid? PartnerId { get; set; }
    public string? Status { get; set; }
    public string? Direction { get; set; }
    public string? TargetActorType { get; set; }
}