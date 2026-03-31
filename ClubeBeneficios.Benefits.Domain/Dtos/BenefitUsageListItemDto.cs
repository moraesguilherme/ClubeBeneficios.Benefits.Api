namespace ClubeBeneficios.Benefits.Domain.Dtos;

public class BenefitUsageListItemDto
{
    public Guid Id { get; set; }
    public Guid BenefitId { get; set; }
    public Guid? BenefitRequestId { get; set; }
    public Guid? PartnerId { get; set; }
    public string? UsageStatus { get; set; }
    public string? UsedByType { get; set; }
    public DateTime? UsedAt { get; set; }
    public decimal? MonetaryValue { get; set; }
    public decimal? DiscountValue { get; set; }
    public string? SnapshotTitle { get; set; }
    public string? SnapshotPartnerName { get; set; }
}