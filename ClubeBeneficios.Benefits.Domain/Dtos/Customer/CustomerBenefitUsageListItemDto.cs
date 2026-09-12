namespace ClubeBeneficios.Benefits.Domain.Dtos.Customer;

public class CustomerBenefitUsageListItemDto
{
    public Guid Id { get; set; }

    public Guid? BenefitRequestId { get; set; }
    public Guid BenefitId { get; set; }

    public string BenefitTitle { get; set; } = string.Empty;
    public string? BenefitType { get; set; }

    public Guid PartnerId { get; set; }
    public string? PartnerName { get; set; }

    public string? UsageStatus { get; set; }
    public DateTime? UsedAt { get; set; }

    public Guid? PetId { get; set; }
    public string? PetName { get; set; }

    public decimal? MonetaryValue { get; set; }
    public decimal? DiscountValue { get; set; }

    public string? SnapshotRuleSummary { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}