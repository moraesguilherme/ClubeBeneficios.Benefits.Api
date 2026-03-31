namespace ClubeBeneficios.Benefits.Domain.Entities;

public class BenefitEntity
{
    public Guid Id { get; set; }
    public Guid? PartnerId { get; set; }
    public string? Title { get; set; }
    public string? BenefitType { get; set; }
    public string? Direction { get; set; }
    public string? TargetActorType { get; set; }
    public string? Status { get; set; }
    public string? ShortDescription { get; set; }
    public string? FullDescription { get; set; }
    public string? InternalNotes { get; set; }
    public string? EligibilityType { get; set; }
    public string? EligibilitySummary { get; set; }
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
    public string? ApprovalNotes { get; set; }
    public string? RejectionReason { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}