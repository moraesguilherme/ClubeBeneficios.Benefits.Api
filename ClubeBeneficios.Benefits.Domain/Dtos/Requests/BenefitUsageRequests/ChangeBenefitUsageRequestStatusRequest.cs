namespace ClubeBeneficios.Benefits.Domain.Dtos.Requests.BenefitRequests;

public class ChangeBenefitUsageRequestStatusRequest
{
    public string? RequestStatus { get; set; }
    public string? ReviewNotes { get; set; }
    public DateTime? ScheduledFor { get; set; }
}