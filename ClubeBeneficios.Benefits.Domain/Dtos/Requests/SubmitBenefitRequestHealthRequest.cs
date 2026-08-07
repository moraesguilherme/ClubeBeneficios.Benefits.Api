namespace ClubeBeneficios.Benefits.Domain.Dtos.Requests;

public class SubmitBenefitRequestHealthRequest
{
    public BenefitRequestVaccinationCardRequest? VaccinationCard { get; set; }
    public BenefitRequestPreventiveRequest? Dewormer { get; set; }
    public BenefitRequestPreventiveRequest? FleaTick { get; set; }
}

public class BenefitRequestVaccinationCardRequest
{
    public string? SourceType { get; set; } = "uploaded";

    public Guid? ClientDocumentId { get; set; }
    public Guid? PartnerCustomerDocumentId { get; set; }

    public string? FileUrl { get; set; }
    public string? FileName { get; set; }

    public string? SubmissionStatus { get; set; } = "submitted";
    public string? Notes { get; set; }
}

public class BenefitRequestPreventiveRequest
{
    public string? SourceType { get; set; } = "uploaded";

    public Guid? ClientPetHealthRecordId { get; set; }
    public Guid? PartnerCustomerPetHealthRecordId { get; set; }

    public string? ApplicationType { get; set; }
    public string? BrandName { get; set; }

    public DateTime? AppliedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }

    public string? SubmissionStatus { get; set; } = "submitted";
    public string? Notes { get; set; }
}