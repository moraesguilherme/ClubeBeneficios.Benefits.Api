namespace ClubeBeneficios.Benefits.Domain.Dtos;

public class BenefitRequestReviewDto
{
    public Guid Id { get; set; }
    public Guid BenefitRequestId { get; set; }

    public string? ReviewStatus { get; set; }
    public string? ReviewPoint { get; set; }
    public string? ReviewRecommendation { get; set; }

    public Guid ReviewedByUserId { get; set; }
    public string? ReviewedByName { get; set; }

    public DateTime ReviewedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}