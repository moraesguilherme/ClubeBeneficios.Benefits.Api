namespace ClubeBeneficios.Benefits.Domain.Dtos.Customer;

public sealed class CustomerBenefitRequestDetailDto : CustomerBenefitRequestListItemDto
{
    public string? BenefitDescription { get; set; }
    public string? EligibilitySummary { get; set; }
    public string? UsageScope { get; set; }
    public string? UsageScopeLabel { get; set; }
    public string? RecurrenceType { get; set; }
    public string? RecurrenceLabel { get; set; }
    public string? ValidityType { get; set; }
    public string? ValidityLabel { get; set; }

    public string? PartnerSegment { get; set; }
    public string? PartnerCategory { get; set; }

    public string? ReviewNotes { get; set; }
    public DateTime? ReviewedAt { get; set; }

    public string? LatestReviewStatus { get; set; }
    public string? LatestReviewPoint { get; set; }
    public string? LatestReviewRecommendation { get; set; }
    public DateTime? LatestReviewedAt { get; set; }
}