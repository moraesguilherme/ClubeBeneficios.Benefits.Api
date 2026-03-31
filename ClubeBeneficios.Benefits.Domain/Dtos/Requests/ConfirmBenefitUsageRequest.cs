namespace ClubeBeneficios.Benefits.Domain.Dtos.Requests;

public class ConfirmBenefitUsageRequest
{
    public Guid BenefitId { get; set; }
    public Guid? BenefitRequestId { get; set; }
    public Guid? PartnerId { get; set; }
    public Guid? UsedByUserId { get; set; }
    public Guid? UsedByPartnerCustomerId { get; set; }
    public string? UsedByType { get; set; }
    public Guid? PetId { get; set; }
    public DateTime? UsedAt { get; set; }
    public decimal? MonetaryValue { get; set; }
    public decimal? DiscountValue { get; set; }
    public string? SnapshotTitle { get; set; }
    public string? SnapshotPartnerName { get; set; }
    public string? SnapshotRuleSummary { get; set; }
}