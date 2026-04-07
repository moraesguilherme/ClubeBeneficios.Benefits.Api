using System;
using System.Collections.Generic;

namespace ClubeBeneficios.Benefits.Domain.Dtos.Requests;

public class UpdateBenefitRequest
{
    public string? Title { get; set; }
    public string? BenefitType { get; set; }
    public Guid? PartnerId { get; set; }
    public string? Direction { get; set; }
    public string? TargetActorType { get; set; }

    public string? ShortDescription { get; set; }
    public string? FullDescription { get; set; }
    public string? InternalNotes { get; set; }

    public string? Status { get; set; }

    public string? EligibilityType { get; set; }
    public string? EligibilitySummary { get; set; }
    public List<string> EligibilityChips { get; set; } = [];

    public string? LevelType { get; set; }
    public List<string> AllowedLevels { get; set; } = [];

    public bool MinFrequencyEnabled { get; set; }
    public int? MinFrequencyValue { get; set; }
    public int? FrequencyWindowMonths { get; set; }

    public bool MinTicketEnabled { get; set; }
    public decimal? MinTicketValue { get; set; }
    public int? TicketWindowMonths { get; set; }

    public bool FirstUseOnly { get; set; }
    public bool RequiresMatilhaApproval { get; set; }
    public string? CustomRuleText { get; set; }

    public bool RequiresAccessCode { get; set; }
    public bool AllowAnyActivePartnerCode { get; set; } = true;
    public Guid? SpecificAccessCodeId { get; set; }
    public string? CodeValidationMode { get; set; }

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
    public Guid? UpdatedByUserId { get; set; }
}