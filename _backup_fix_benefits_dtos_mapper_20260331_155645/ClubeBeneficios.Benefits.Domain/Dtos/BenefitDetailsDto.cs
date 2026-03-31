using System.Linq;

namespace ClubeBeneficios.Benefits.Domain.Dtos;

public class BenefitDetailsDto
{
    public Guid Id { get; set; }
    public Guid? PartnerId { get; set; }

    public string Title { get; set; } = string.Empty;
    public string? Slug { get; set; }
    public string? BenefitType { get; set; }
    public string? Direction { get; set; }
    public string? TargetActorType { get; set; }

    public string? ShortDescription { get; set; }
    public string? FullDescription { get; set; }
    public string? InternalNotes { get; set; }

    public string? EligibilityType { get; set; }
    public string? EligibilitySummary { get; set; }
    public List<string> EligibilityChips { get; set; } = new();

    public List<BenefitLevelScopeDto> LevelScopes { get; set; } = new();

    public BenefitLevelScopeDto? LevelScope
    {
        get => LevelScopes.FirstOrDefault();
        set
        {
            LevelScopes = value is null
                ? new List<BenefitLevelScopeDto>()
                : new List<BenefitLevelScopeDto> { value };
        }
    }

    public BenefitBehaviorRulesDto? BehaviorRules { get; set; }
    public BenefitCodeRulesDto? CodeRules { get; set; }

    public string? RecurrenceType { get; set; }
    public int? RecurrenceLimit { get; set; }
    public string? RecurrencePeriod { get; set; }

    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidUntil { get; set; }

    public bool AutoActivateWhenApproved { get; set; }
    public bool RequiresManualRelease { get; set; }
    public bool HighlightInShowcase { get; set; }

    public string? Status { get; set; }
    public string? StackingRule { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public string? PartnerName { get; set; }
    public string? CreatedByName { get; set; }
    public string? ApprovedByName { get; set; }
}
