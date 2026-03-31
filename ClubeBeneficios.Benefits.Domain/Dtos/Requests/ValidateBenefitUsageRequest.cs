namespace ClubeBeneficios.Benefits.Domain.Dtos.Requests;

public class ValidateBenefitUsageRequest
{
    public Guid BenefitId { get; set; }
    public Guid? PartnerId { get; set; }
    public Guid? UserId { get; set; }
    public Guid? PartnerCustomerId { get; set; }
    public string? ActorType { get; set; }
    public DateTime? ValidationReferenceDate { get; set; }
}