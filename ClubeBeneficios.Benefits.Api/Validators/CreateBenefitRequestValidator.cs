using FluentValidation;
using ClubeBeneficios.Benefits.Domain.Dtos.Requests;

namespace ClubeBeneficios.Benefits.Api.Validators;

public class CreateBenefitRequestValidator : AbstractValidator<CreateBenefitRequest>
{
    public CreateBenefitRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(160);

        RuleFor(x => x.BenefitType)
            .NotEmpty()
            .MaximumLength(80);

        RuleFor(x => x.Direction)
            .NotEmpty()
            .MaximumLength(40);

        RuleFor(x => x.TargetActorType)
            .NotEmpty()
            .MaximumLength(40);

        RuleFor(x => x.ShortDescription)
            .NotEmpty()
            .MaximumLength(400);

        RuleFor(x => x.FullDescription)
            .NotEmpty()
            .MaximumLength(4000);

        RuleFor(x => x.EligibilityType)
            .NotEmpty()
            .MaximumLength(40);

        RuleFor(x => x.EligibilitySummary)
            .MaximumLength(300);

        RuleFor(x => x.EligibilityChips)
            .MaximumLength(600);

        RuleFor(x => x.RecurrenceType)
            .MaximumLength(40);

        RuleFor(x => x.RecurrencePeriod)
            .MaximumLength(40);

        RuleFor(x => x.ValidityType)
            .MaximumLength(40);

        RuleFor(x => x.StackingRule)
            .MaximumLength(40);

        RuleFor(x => x.RecurrenceValue)
            .GreaterThan(0)
            .When(x => x.RecurrenceValue.HasValue);

        RuleFor(x => x)
            .Must(x => !x.StartsAt.HasValue || !x.EndsAt.HasValue || x.EndsAt.Value >= x.StartsAt.Value)
            .WithMessage("A data final deve ser maior ou igual Ã  data inicial.");
    }
}