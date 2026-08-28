using Microsoft.AspNetCore.Http;

namespace ClubeBeneficios.Benefits.Domain.Dtos.PublicCatalog
{
    public class CreatePublicPartnerBenefitRequestFormDto
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

        public string? PetSize { get; set; }
        public bool PetIsNeutered { get; set; }

        public bool AcceptedTerms { get; set; }
        public bool AcceptedPrivacyPolicy { get; set; }

        public string? CustomerNotes { get; set; }

        public IFormFile? VaccinationCardFile { get; set; }

        public string? DewormerBrandName { get; set; }
        public DateTime? DewormerAppliedAt { get; set; }
        public DateTime? DewormerExpiresAt { get; set; }

        public string? FleaTickBrandName { get; set; }
        public DateTime? FleaTickAppliedAt { get; set; }
        public DateTime? FleaTickExpiresAt { get; set; }
    }
}
