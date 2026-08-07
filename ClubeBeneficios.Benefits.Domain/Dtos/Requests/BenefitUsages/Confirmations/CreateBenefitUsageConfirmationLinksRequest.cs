namespace ClubeBeneficios.Benefits.Domain.Dtos.Requests.BenefitUsages.Confirmations;

public class CreateBenefitUsageConfirmationLinksRequest
{
    public string PartnerRecipientEmail { get; set; } = string.Empty;
    public string? PartnerRecipientName { get; set; }

    public int ExpirationHours { get; set; }
}
