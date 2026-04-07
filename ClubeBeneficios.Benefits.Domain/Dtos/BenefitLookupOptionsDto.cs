namespace ClubeBeneficios.Benefits.Domain.Dtos;

public class BenefitLookupOptionsDto
{
    public IEnumerable<LookupItemDto> Statuses { get; set; } = Enumerable.Empty<LookupItemDto>();
    public IEnumerable<LookupItemDto> Directions { get; set; } = Enumerable.Empty<LookupItemDto>();
    public IEnumerable<LookupItemDto> TargetActorTypes { get; set; } = Enumerable.Empty<LookupItemDto>();
    public IEnumerable<LookupItemDto> EligibilityTypes { get; set; } = Enumerable.Empty<LookupItemDto>();

    public IEnumerable<LookupItemDto> BenefitTypes { get; set; } = Enumerable.Empty<LookupItemDto>();
    public BenefitEligibleLevelsLookupDto EligibleLevels { get; set; } = new();

    public IEnumerable<BenefitLookupCardDto> RecurrenceCards { get; set; } = Enumerable.Empty<BenefitLookupCardDto>();
    public IEnumerable<LookupItemDto> RecurrencePeriods { get; set; } = Enumerable.Empty<LookupItemDto>();

    public IEnumerable<BenefitLookupCardDto> ValidityCards { get; set; } = Enumerable.Empty<BenefitLookupCardDto>();
    public IEnumerable<LookupItemDto> StackingRules { get; set; } = Enumerable.Empty<LookupItemDto>();
}