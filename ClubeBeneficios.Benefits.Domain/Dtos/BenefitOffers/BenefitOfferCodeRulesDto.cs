namespace ClubeBeneficios.Benefits.Domain.Dtos.Benefits;

public class BenefitOfferCodeRulesDto
{
    public bool RequiresAccessCode { get; set; }
    public bool AllowAnyActivePartnerCode { get; set; }
    public Guid? SpecificAccessCodeId { get; set; }

    // Campo esperado hoje pelo BenefitContractMapper
    public string? CodeValidationMode { get; set; }
}
