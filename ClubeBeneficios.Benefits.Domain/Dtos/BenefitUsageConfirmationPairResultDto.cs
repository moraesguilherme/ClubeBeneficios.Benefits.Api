namespace ClubeBeneficios.Benefits.Domain.Dtos;

public class BenefitUsageConfirmationPairResultDto
{
    public Guid BenefitRequestId { get; set; }
    public Guid ClientConfirmationId { get; set; }
    public Guid PartnerConfirmationId { get; set; }
    public Guid? ClientNotificationId { get; set; }
    public Guid? PartnerNotificationId { get; set; }
}