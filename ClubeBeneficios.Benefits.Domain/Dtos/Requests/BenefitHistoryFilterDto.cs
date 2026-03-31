namespace ClubeBeneficios.Benefits.Domain.Dtos.Requests;

public class BenefitHistoryFilterDto
{
    public Guid BenefitId { get; set; }
    public Guid? PartnerId { get; set; }
}