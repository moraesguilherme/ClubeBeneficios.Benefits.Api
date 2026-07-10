namespace ClubeBeneficios.Benefits.Domain.Dtos.Requests.Automation;

public class RecalculateClientLevelsRequest
{
    public Guid? UserId { get; set; }
    public DateTime? ReferenceDate { get; set; }
}