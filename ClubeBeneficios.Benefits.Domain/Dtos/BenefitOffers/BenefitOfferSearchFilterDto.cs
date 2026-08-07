namespace ClubeBeneficios.Benefits.Domain.Dtos.Benefits;

public class BenefitOfferSearchFilterDto
{
    public string? Search { get; set; }
    public Guid? PartnerId { get; set; }
    public string? Status { get; set; }
    public string? Direction { get; set; }
    public string? TargetActorType { get; set; }
}