using Microsoft.AspNetCore.Http;

namespace ClubeBeneficios.Benefits.Domain.Dtos.PublicCatalog;

public class PublicBenefitRequestCorrectionDto
{
    public Guid RequestId { get; set; }
    public Guid BenefitId { get; set; }
    public Guid PartnerId { get; set; }
    public Guid PartnerCustomerId { get; set; }
    public Guid? PartnerCustomerPetId { get; set; }

    public string BenefitTitle { get; set; } = string.Empty;
    public string PartnerName { get; set; } = string.Empty;

    public string RequesterName { get; set; } = string.Empty;
    public string? RequesterEmail { get; set; }
    public string? RequesterPhone { get; set; }

    public string? PetName { get; set; }
    public string? PetBreed { get; set; }
    public string? PetSex { get; set; }
    public int? PetAgeMonths { get; set; }
    public string? PetSize { get; set; }
    public bool? PetIsNeutered { get; set; }

    public string RequestStatus { get; set; } = string.Empty;
    public string RequestStatusLabel { get; set; } = string.Empty;

    public string? ReviewPoint { get; set; }
    public string? ReviewRecommendation { get; set; }

    public string? VaccinationCardFileUrl { get; set; }
    public string? VaccinationCardFileName { get; set; }

    public string? DewormerBrandName { get; set; }
    public DateTime? DewormerAppliedAt { get; set; }
    public DateTime? DewormerExpiresAt { get; set; }

    public string? FleaTickBrandName { get; set; }
    public DateTime? FleaTickAppliedAt { get; set; }
    public DateTime? FleaTickExpiresAt { get; set; }
}

public class SubmitPublicBenefitRequestCorrectionFormDto
{
    public IFormFile? VaccinationCardFile { get; set; }

    public string? DewormerBrandName { get; set; }
    public DateTime? DewormerAppliedAt { get; set; }
    public DateTime? DewormerExpiresAt { get; set; }

    public string? FleaTickBrandName { get; set; }
    public DateTime? FleaTickAppliedAt { get; set; }
    public DateTime? FleaTickExpiresAt { get; set; }

    public string? CustomerNotes { get; set; }
}

public class PublicBenefitRequestCorrectionSubmittedDto
{
    public Guid RequestId { get; set; }
    public string RequestStatus { get; set; } = string.Empty;
    public string ApprovalStatus { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}