namespace ClubeBeneficios.Benefits.Domain.Dtos.BenefitUsages.Confirmations;

public class BenefitUsageConfirmationTokenDto
{
    public Guid Id { get; set; }
    public Guid BenefitRequestId { get; set; }
    public Guid? BenefitUsageId { get; set; }
    public Guid BenefitId { get; set; }
    public Guid PartnerId { get; set; }

    public string? ConfirmationType { get; set; }
    public string? ConfirmationStatus { get; set; }

    public string? RecipientEmail { get; set; }
    public string? RecipientName { get; set; }

    public DateTime ExpiresAt { get; set; }
    public DateTime? ConfirmedAt { get; set; }
    public DateTime? RejectedAt { get; set; }

    public Guid? NotificationId { get; set; }

    public string? BenefitTitle { get; set; }
    public string? PartnerName { get; set; }
    public string? RequestStatus { get; set; }
    public string? RequesterType { get; set; }
    public string? RequesterName { get; set; }
    public string? PetName { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}