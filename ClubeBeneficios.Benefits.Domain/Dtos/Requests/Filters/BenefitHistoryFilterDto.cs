namespace ClubeBeneficios.Benefits.Domain.Dtos.Requests.Filters;

public class BenefitHistoryFilterDto
{
    public Guid BenefitId { get; set; }
    public Guid? PartnerId { get; set; }
}