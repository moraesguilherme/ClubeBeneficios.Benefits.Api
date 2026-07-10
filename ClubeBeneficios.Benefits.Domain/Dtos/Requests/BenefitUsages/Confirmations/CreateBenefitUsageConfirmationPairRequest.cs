namespace ClubeBeneficios.Benefits.Domain.Dtos.Requests.BenefitUsages.Confirmations;

public class CreateBenefitUsageConfirmationPairRequest
{
    public string PartnerRecipientEmail { get; set; } = string.Empty;
    public string? PartnerRecipientName { get; set; }

    public int ExpirationHours { get; set; }
}
