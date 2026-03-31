using System;

namespace ClubeBeneficios.Benefits.Domain.Dtos;

public class BenefitApprovalItemDto
{
    public Guid Id { get; set; }
    public Guid? PartnerId { get; set; }
    public string? PartnerName { get; set; }

    public string? Title { get; set; }
    public string? BenefitType { get; set; }

    public string? Direction { get; set; }
    public string? DirectionLabel { get; set; }

    public string? Status { get; set; }
    public string? ApprovalStage { get; set; }

    public string? TargetActorType { get; set; }
    public string? TargetActorLabel { get; set; }

    public string? EligibilityType { get; set; }
    public string? EligibilitySummary { get; set; }

    public bool AutoActivateWhenApproved { get; set; }
    public bool RequiresManualRelease { get; set; }
    public bool HighlightInShowcase { get; set; }

    public string? LastReviewType { get; set; }
    public string? LastReviewNotes { get; set; }
    public string? LastReviewedBy { get; set; }
    public DateTime? LastReviewedAt { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}