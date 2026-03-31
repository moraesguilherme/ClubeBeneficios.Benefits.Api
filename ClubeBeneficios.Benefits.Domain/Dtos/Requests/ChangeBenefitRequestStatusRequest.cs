namespace ClubeBeneficios.Benefits.Domain.Dtos.Requests;

public class ChangeBenefitRequestStatusRequest
{
    public string? RequestStatus { get; set; }
    public string? ReviewNotes { get; set; }
    public DateTime? ScheduledFor { get; set; }
}