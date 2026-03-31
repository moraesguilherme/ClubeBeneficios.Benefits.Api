namespace ClubeBeneficios.Benefits.Domain.Dtos;

public class BenefitMetricItemDto
{
    public Guid BenefitId { get; set; }
    public string? MetricCode { get; set; }
    public string? MetricName { get; set; }
    public decimal MetricValue { get; set; }
    public DateTime ReferenceDate { get; set; }
    public DateTime CreatedAt { get; set; }
}