namespace ClubeBeneficios.Benefits.Domain.Dtos;

public class BenefitUsageConfirmationConfirmResultDto
{
    public Guid ConfirmationId { get; set; }
    public Guid BenefitRequestId { get; set; }
    public string? ConfirmationType { get; set; }
    public string? Result { get; set; }
    public Guid? BenefitUsageId { get; set; }
}