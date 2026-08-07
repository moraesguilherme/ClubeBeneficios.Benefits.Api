namespace ClubeBeneficios.Benefits.Domain.Dtos;

public class BenefitRequestPreventiveDto
{
    public Guid? Id { get; set; }
    public string? SourceType { get; set; }

    public Guid? ClientPetHealthRecordId { get; set; }
    public Guid? PartnerCustomerPetHealthRecordId { get; set; }

    public string? ApplicationType { get; set; }
    public string? BrandName { get; set; }

    public DateTime? AppliedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }

    public string? SubmissionStatus { get; set; }
    public string? Notes { get; set; }

    public DateTime? ReviewedAt { get; set; }
    public Guid? ReviewedByUserId { get; set; }
}