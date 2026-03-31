namespace ClubeBeneficios.Benefits.Domain.Dtos;

public class BenefitFilterOptionsDto
{
    public IReadOnlyCollection<LookupItemDto> Origins { get; set; } = Array.Empty<LookupItemDto>();
    public IReadOnlyCollection<LookupItemDto> Statuses { get; set; } = Array.Empty<LookupItemDto>();
    public IReadOnlyCollection<LookupItemDto> Audiences { get; set; } = Array.Empty<LookupItemDto>();
    public IReadOnlyCollection<LookupItemDto> EligibilityTypes { get; set; } = Array.Empty<LookupItemDto>();
    public IReadOnlyCollection<LookupItemDto> Partners { get; set; } = Array.Empty<LookupItemDto>();
}