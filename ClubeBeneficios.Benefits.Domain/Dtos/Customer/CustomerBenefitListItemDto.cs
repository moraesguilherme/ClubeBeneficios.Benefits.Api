namespace ClubeBeneficios.Benefits.Domain.Dtos.Customer;

public class CustomerBenefitListItemDto
{
    public Guid Id { get; set; }
    public Guid PartnerId { get; set; }

    public string Title { get; set; } = string.Empty;
    public string? ShortDescription { get; set; }
    public string? FullDescription { get; set; }

    public string? PartnerName { get; set; }
    public string? PartnerSegment { get; set; }
    public string? PartnerCategory { get; set; }

    public string? BenefitType { get; set; }
    public string? EligibilityType { get; set; }
    public string? EligibilitySummary { get; set; }

    public string? UsageScope { get; set; }
    public string? UsageScopeLabel { get; set; }

    public string? RecurrenceType { get; set; }
    public string? RecurrenceLabel { get; set; }

    public string? ValidityType { get; set; }
    public string? ValidityLabel { get; set; }

    public bool RequiresPartnerAvailability { get; set; }
    public bool RequiresMatilhaAcceptanceRules { get; set; }
    public bool RequiresManualRelease { get; set; }

    public DateTime? StartsAt { get; set; }
    public DateTime? EndsAt { get; set; }
}