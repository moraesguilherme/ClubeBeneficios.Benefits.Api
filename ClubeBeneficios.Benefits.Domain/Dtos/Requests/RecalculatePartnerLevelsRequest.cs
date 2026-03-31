namespace ClubeBeneficios.Benefits.Domain.Dtos.Requests;

public class RecalculatePartnerLevelsRequest
{
    public Guid? PartnerId { get; set; }
    public DateTime? ReferenceDate { get; set; }
}