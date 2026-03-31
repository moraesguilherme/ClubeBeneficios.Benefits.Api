namespace ClubeBeneficios.Benefits.Domain.Dtos.Requests;

public class AddBenefitReviewRequest
{
    public string ReviewType { get; set; } = string.Empty;
    public string Decision { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public string? InternalNotes { get; set; }
    public string? RequestedChanges { get; set; }
    public string? ReviewedByUserId { get; set; }
    public string? ReviewedByName { get; set; }
}