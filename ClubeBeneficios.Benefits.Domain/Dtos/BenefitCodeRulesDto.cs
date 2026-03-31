namespace ClubeBeneficios.Benefits.Domain.Dtos;

public class BenefitCodeRulesDto
{
    public bool RequiresAccessCode { get; set; }
    public bool AllowAnyActivePartnerCode { get; set; }
    public Guid? SpecificAccessCodeId { get; set; }
    public string? ValidationMode { get; set; }
    public string? Summary { get; set; }
}
