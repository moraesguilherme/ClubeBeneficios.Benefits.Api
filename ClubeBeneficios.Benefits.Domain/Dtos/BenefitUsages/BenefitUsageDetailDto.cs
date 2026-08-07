namespace ClubeBeneficios.Benefits.Domain.Dtos.BenefitUsages;

public class BenefitUsageDetailDto
{
    public Guid Id { get; set; }

    public Guid BenefitId { get; set; }
    public Guid? BenefitRequestId { get; set; }

    public Guid? PartnerId { get; set; }
    public string? PartnerName { get; set; }

    public string? UsedByType { get; set; }
    public Guid? UsedByUserId { get; set; }
    public Guid? UsedByClientId { get; set; }
    public Guid? UsedByPartnerCustomerId { get; set; }

    public string? ClientName { get; set; }
    public string? PartnerCustomerName { get; set; }
    public string? UsedByName { get; set; }

    public string? PetSourceType { get; set; }
    public Guid? ClientPetId { get; set; }
    public Guid? PartnerCustomerPetId { get; set; }
    public string? PetName { get; set; }

    public string? UsageStatus { get; set; }
    public DateTime? UsedAt { get; set; }

    public Guid? ConfirmedByPartnerUserId { get; set; }
    public Guid? ConfirmedByAdminUserId { get; set; }
    public Guid? RecordedByUserId { get; set; }

    public decimal? MonetaryValue { get; set; }
    public decimal? DiscountValue { get; set; }

    public string? SnapshotTitle { get; set; }
    public string? SnapshotPartnerName { get; set; }
    public string? SnapshotRuleSummary { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}