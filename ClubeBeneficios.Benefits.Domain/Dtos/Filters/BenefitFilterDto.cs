namespace ClubeBeneficios.Benefits.Domain.Dtos.Filters;

public class BenefitFilterDto
{
    public string? Search { get; set; }
    public Guid? PartnerId { get; set; }
    public string? Origin { get; set; }
    public string? Status { get; set; }
    public string? TargetActorType { get; set; }
    public string? EligibilityType { get; set; }
    public bool OnlyPendingApproval { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 12;
}