namespace ClubeBeneficios.Benefits.Domain.Dtos.PublicCatalog;

public class CreatePublicPartnerBenefitRequestDto
{
    public string? CustomerName { get; set; }
    public string? CustomerEmail { get; set; }
    public string? CustomerPhone { get; set; }
    public string? CustomerDocument { get; set; }

    public string? PetName { get; set; }
    public string? PetBreed { get; set; }
    public string? PetSex { get; set; }

    public int? PetAgeYears { get; set; }
    public int? PetAgeMonthsAdditional { get; set; }

    public int? PetAgeMonths { get; set; }

    public string? PetSize { get; set; }
    public bool PetIsNeutered { get; set; }

    public bool AcceptedTerms { get; set; }
    public bool AcceptedPrivacyPolicy { get; set; }

    public string? CustomerNotes { get; set; }
}

public class PublicPartnerBenefitRequestCreatedDto
{
    public Guid RequestId { get; set; }
    public Guid PartnerCustomerId { get; set; }
    public Guid? PartnerCustomerPetId { get; set; }
    public Guid BenefitId { get; set; }
    public Guid PartnerId { get; set; }

    public string? RequestStatus { get; set; }
    public string? ApprovalStatus { get; set; }
    public string? Message { get; set; }
}