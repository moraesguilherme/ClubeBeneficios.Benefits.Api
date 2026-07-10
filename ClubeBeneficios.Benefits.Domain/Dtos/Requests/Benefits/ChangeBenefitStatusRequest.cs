namespace ClubeBeneficios.Benefits.Domain.Dtos.Requests.Benefits;

public class ChangeBenefitStatusRequest
{
    public string? Status { get; set; }
    public string? Reason { get; set; }
    public Guid? ChangedByUserId { get; set; }
}