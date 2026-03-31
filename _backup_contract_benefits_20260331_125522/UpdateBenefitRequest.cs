namespace ClubeBeneficios.Benefits.Domain.Dtos.Requests;

public class UpdateBenefitRequest
{
    public string? Title { get; set; }
    public string? BenefitType { get; set; }
    public string? TargetActorType { get; set; }
    public string? ShortDescription { get; set; }
    public string? FullDescription { get; set; }
    public string? InternalNotes { get; set; }
    public string? EligibilityType { get; set; }
    public string? EligibilitySummary { get; set; }
    public string? EligibilityChips { get; set; }
    public string? RecurrenceType { get; set; }
    public int? RecurrenceValue { get; set; }
    public string? RecurrencePeriod { get; set; }
    public string? ValidityType { get; set; }
    public DateTime? StartsAt { get; set; }
    public DateTime? EndsAt { get; set; }
    public bool AutoActivateWhenApproved { get; set; }
    public bool RequiresManualRelease { get; set; }
    public bool HighlightInShowcase { get; set; }
    public string? StackingRule { get; set; }
}