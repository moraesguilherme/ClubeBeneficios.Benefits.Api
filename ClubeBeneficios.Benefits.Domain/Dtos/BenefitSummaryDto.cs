namespace ClubeBeneficios.Benefits.Domain.Dtos;

public class BenefitSummaryDto
{
    public int ActiveCount { get; set; }
    public int PendingCount { get; set; }
    public int InReviewCount { get; set; }
    public int LowConversionCount { get; set; }
    public int ActivePartnersCount { get; set; }
    public int TotalRequests { get; set; }
    public int TotalUsages { get; set; }
    public int ConversionRate { get; set; }
}