namespace ClubeBeneficios.Benefits.Domain.Dtos.Requests.Benefits;

public class AddBenefitOfferReviewRequest
{
    public string ReviewStatus { get; set; } = string.Empty;
    public string ReviewPoint { get; set; } = string.Empty;
    public string? ReviewRecommendation { get; set; }
    public Guid? ReviewedByUserId { get; set; }
}