namespace ClubeBeneficios.Benefits.Domain.Dtos;

public class BenefitFilterOptionsDto
{
    public IReadOnlyCollection<LookupOptionDto> Origins { get; set; } = Array.Empty<LookupOptionDto>();
    public IReadOnlyCollection<LookupOptionDto> Statuses { get; set; } = Array.Empty<LookupOptionDto>();
    public IReadOnlyCollection<LookupOptionDto> Audiences { get; set; } = Array.Empty<LookupOptionDto>();
    public IReadOnlyCollection<LookupOptionDto> EligibilityTypes { get; set; } = Array.Empty<LookupOptionDto>();
    public IReadOnlyCollection<LookupOptionDto> Partners { get; set; } = Array.Empty<LookupOptionDto>();

    // Compatibilidade com o repository/refactors em andamento
    public IReadOnlyCollection<LookupOptionDto> Directions { get; set; } = Array.Empty<LookupOptionDto>();
    public IReadOnlyCollection<LookupOptionDto> TargetActorTypes { get; set; } = Array.Empty<LookupOptionDto>();
    public IReadOnlyCollection<LookupOptionDto> RecurrencePeriods { get; set; } = Array.Empty<LookupOptionDto>();
}