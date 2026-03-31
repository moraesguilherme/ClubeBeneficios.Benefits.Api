namespace ClubeBeneficios.Benefits.Domain.Dtos;

public class BenefitUsageFilterDto
{
    public string? Search { get; set; }
    public Guid? BenefitId { get; set; }
    public Guid? BenefitRequestId { get; set; }
    public Guid? PartnerId { get; set; }
    public string? UsageStatus { get; set; }
    public string? UsedByType { get; set; }
    public DateTime? UsedFrom { get; set; }
    public DateTime? UsedTo { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}