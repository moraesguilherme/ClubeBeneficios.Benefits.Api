namespace ClubeBeneficios.Benefits.Domain.Dtos;

public class PartnerLevelAutomationResultDto
{
    public Guid PartnerId { get; set; }
    public string? PreviousLevel { get; set; }
    public string? CurrentLevel { get; set; }
    public DateTime ReferenceDate { get; set; }
    public bool Changed { get; set; }
    public string? Reason { get; set; }
}