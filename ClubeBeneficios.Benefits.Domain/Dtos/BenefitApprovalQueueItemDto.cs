namespace ClubeBeneficios.Benefits.Domain.Dtos;

public class BenefitApprovalQueueItemDto
{
    public Guid Id { get; set; }
    public Guid? PartnerId { get; set; }
    public string? PartnerName { get; set; }
    public string? Title { get; set; }
    public string? Direction { get; set; }
    public string? Status { get; set; }
    public string? ApprovalStatus { get; set; }
    public string? EligibilitySummary { get; set; }
    public string? RecurrenceLabel { get; set; }
    public string? ValidityLabel { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}