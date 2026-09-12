namespace ClubeBeneficios.Benefits.Domain.Dtos.Customer;

public sealed class CustomerBenefitUsageDetailDto : CustomerBenefitUsageListItemDto
{
    public string? BenefitDescription { get; set; }
    public string? PartnerSegment { get; set; }
    public string? PartnerCategory { get; set; }

    public string? UsageScope { get; set; }
    public string? UsageScopeLabel { get; set; }

    public string? RequestStatus { get; set; }
    public DateTime? RequestedAt { get; set; }

    public string? ConfirmedByPartnerName { get; set; }
    public string? ConfirmedByAdminName { get; set; }
}