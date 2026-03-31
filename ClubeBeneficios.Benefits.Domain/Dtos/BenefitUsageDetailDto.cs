namespace ClubeBeneficios.Benefits.Domain.Dtos;

public class BenefitUsageDetailDto
{
    public Guid Id { get; set; }
    public Guid BenefitId { get; set; }
    public Guid? BenefitRequestId { get; set; }
    public Guid? PartnerId { get; set; }
    public Guid? UsedByUserId { get; set; }
    public Guid? UsedByPartnerCustomerId { get; set; }
    public string? UsedByType { get; set; }
    public Guid? PetId { get; set; }
    public string? UsageStatus { get; set; }
    public DateTime? UsedAt { get; set; }
    public Guid? ConfirmedByPartnerUserId { get; set; }
    public Guid? ConfirmedByAdminUserId { get; set; }
    public decimal? MonetaryValue { get; set; }
    public decimal? DiscountValue { get; set; }
    public string? SnapshotTitle { get; set; }
    public string? SnapshotPartnerName { get; set; }
    public string? SnapshotRuleSummary { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}