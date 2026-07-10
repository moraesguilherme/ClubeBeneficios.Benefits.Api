namespace ClubeBeneficios.Benefits.Domain.Dtos.Requests.BenefitUsages;

public class ConfirmBenefitUsageRequest
{
    public Guid BenefitId { get; set; }
    public Guid? BenefitRequestId { get; set; }

    public Guid? PartnerId { get; set; }

    public string? UsedByType { get; set; }

    public Guid? UsedByUserId { get; set; }
    public Guid? UsedByClientId { get; set; }
    public Guid? UsedByPartnerCustomerId { get; set; }

    public string? PetSourceType { get; set; }
    public Guid? ClientPetId { get; set; }
    public Guid? PartnerCustomerPetId { get; set; }

    public decimal? MonetaryValue { get; set; }
    public decimal? DiscountValue { get; set; }

    public string? RuleSummary { get; set; }
}