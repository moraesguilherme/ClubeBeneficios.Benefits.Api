namespace ClubeBeneficios.Benefits.Domain.Dtos;

public class ClientLevelAutomationResultDto
{
    public Guid UserId { get; set; }
    public string? PreviousLevel { get; set; }
    public string? CurrentLevel { get; set; }
    public DateTime ReferenceDate { get; set; }
    public bool Changed { get; set; }
    public string? Reason { get; set; }
}