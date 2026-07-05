namespace ClubeBeneficios.Benefits.Domain.Dtos;

public class BenefitRequestApprovalSummaryDto
{
    public int TotalQueueItems { get; set; }
    public int PendingReviewCount { get; set; }
    public int UnderReviewCount { get; set; }
    public int ApprovedCount { get; set; }
    public int RejectedCount { get; set; }
    public int ExpiredCount { get; set; }
    public DateTime? LatestRequestedAt { get; set; }
}