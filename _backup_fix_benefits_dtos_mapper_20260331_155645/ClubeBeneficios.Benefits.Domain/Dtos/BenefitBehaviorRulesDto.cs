namespace ClubeBeneficios.Benefits.Domain.Dtos;

public class BenefitBehaviorRulesDto
{
    public bool MinFrequencyEnabled { get; set; }
    public int? MinFrequencyValue { get; set; }
    public string? MinFrequencyWindow { get; set; }

    public bool MinTicketEnabled { get; set; }
    public decimal? MinTicketValue { get; set; }

    public bool FirstUseOnly { get; set; }
    public bool RequiresMatilhaApproval { get; set; }

    public string? CustomRuleText { get; set; }
}
