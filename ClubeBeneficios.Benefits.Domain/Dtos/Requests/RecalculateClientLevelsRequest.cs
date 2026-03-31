namespace ClubeBeneficios.Benefits.Domain.Dtos.Requests;

public class RecalculateClientLevelsRequest
{
    public Guid? UserId { get; set; }
    public DateTime? ReferenceDate { get; set; }
}