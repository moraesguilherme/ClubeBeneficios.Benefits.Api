namespace ClubeBeneficios.Benefits.Domain.Dtos.Requests;

public class BenefitMetricsFilterDto
{
    public Guid BenefitId { get; set; }
    public Guid? PartnerId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}