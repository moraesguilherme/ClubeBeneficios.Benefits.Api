namespace ClubeBeneficios.Benefits.Domain.Dtos;

public class BenefitLookupOptionsDto
{
    public IEnumerable<string> Statuses { get; set; } = Enumerable.Empty<string>();
    public IEnumerable<string> Directions { get; set; } = Enumerable.Empty<string>();
    public IEnumerable<string> TargetActorTypes { get; set; } = Enumerable.Empty<string>();
    public IEnumerable<string> EligibilityTypes { get; set; } = Enumerable.Empty<string>();
    public IEnumerable<string> RecurrencePeriods { get; set; } = Enumerable.Empty<string>();
}