namespace ClubeBeneficios.Benefits.Domain.Dtos.Requests;

public class BenefitDashboardSummaryFilterDto
{
    public Guid? PartnerId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}