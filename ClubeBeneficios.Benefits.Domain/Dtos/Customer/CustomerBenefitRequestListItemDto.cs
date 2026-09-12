namespace ClubeBeneficios.Benefits.Domain.Dtos.Customer;

public class CustomerBenefitRequestListItemDto
{
    public Guid Id { get; set; }

    public Guid BenefitId { get; set; }
    public string BenefitTitle { get; set; } = string.Empty;
    public string? BenefitType { get; set; }

    public Guid PartnerId { get; set; }
    public string? PartnerName { get; set; }

    public string? RequestStatus { get; set; }
    public bool ReviewRequired { get; set; }
    public string? ApprovalStatus { get; set; }
    public string? ApprovalReason { get; set; }

    public string? PetSourceType { get; set; }
    public Guid? PetId { get; set; }
    public string? PetName { get; set; }

    public DateTime RequestedAt { get; set; }
    public DateTime? ScheduledFor { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public Guid? UsageId { get; set; }
    public string? UsageStatus { get; set; }
    public DateTime? UsedAt { get; set; }
}