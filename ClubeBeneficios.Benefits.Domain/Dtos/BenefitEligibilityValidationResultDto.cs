namespace ClubeBeneficios.Benefits.Domain.Dtos;

public class BenefitEligibilityValidationResultDto
{
    public bool IsAllowed { get; set; }
    public string? BlockReason { get; set; }
    public DateTime? NextAvailableAt { get; set; }
    public int? AllowedUses { get; set; }
    public int? UsedCount { get; set; }
    public string? RuleSummary { get; set; }
}