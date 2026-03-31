namespace ClubeBeneficios.Benefits.Domain.Dtos;

public class BenefitDashboardSummaryDto
{
    public int TotalBenefits { get; set; }
    public int ActiveBenefits { get; set; }
    public int PendingBenefits { get; set; }
    public int RejectedBenefits { get; set; }
    public int TotalRequests { get; set; }
    public int ApprovedRequests { get; set; }
    public int TotalUsages { get; set; }
    public int ConfirmedUsages { get; set; }
    public decimal TotalDiscountValue { get; set; }
    public decimal TotalRevenueImpacted { get; set; }
}