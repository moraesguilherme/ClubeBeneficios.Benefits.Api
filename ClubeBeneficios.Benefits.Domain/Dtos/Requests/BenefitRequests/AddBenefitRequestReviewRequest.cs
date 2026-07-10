namespace ClubeBeneficios.Benefits.Domain.Dtos.Requests.BenefitRequests;

public class AddBenefitRequestReviewRequest
{
    public string ReviewStatus { get; set; } = string.Empty;
    public string? ReviewPoint { get; set; }
    public string? ReviewRecommendation { get; set; }
    public Guid? ReviewedByUserId { get; set; }
}