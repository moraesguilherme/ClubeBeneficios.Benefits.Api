using System;

namespace ClubeBeneficios.Benefits.Domain.Dtos.Benefits;

public class BenefitListItemDto
{
    public Guid Id { get; set; }
    public Guid? PartnerId { get; set; }
    public string? PartnerName { get; set; }

    public string? Title { get; set; }
    public string? BenefitType { get; set; }

    public string? Direction { get; set; }
    public string? DirectionLabel { get; set; }

    public string? Status { get; set; }

    public string? TargetActorType { get; set; }
    public string? TargetActorLabel { get; set; }

    public string? EligibilityType { get; set; }
    public string? EligibilitySummary { get; set; }

    public string? RecurrenceType { get; set; }
    public string? RecurrencePeriod { get; set; }
    public string? RecurrenceLabel { get; set; }

    public string? ValidityType { get; set; }
    public string? ValidityLabel { get; set; }

    public bool HighlightInShowcase { get; set; }
    public bool AutoActivateWhenApproved { get; set; }

    public int RequestsCount { get; set; }
    public int UsagesCount { get; set; }
    public int ConversionRate { get; set; }

    public string? PartnerLevel { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}