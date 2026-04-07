namespace ClubeBeneficios.Benefits.Domain.Dtos;

public class BenefitEligibleLevelsLookupDto
{
    public IEnumerable<LookupItemDto> ClientLevels { get; set; } = Enumerable.Empty<LookupItemDto>();
    public IEnumerable<LookupItemDto> PartnerLevels { get; set; } = Enumerable.Empty<LookupItemDto>();
}