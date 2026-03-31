using FluentValidation;
using ClubeBeneficios.Benefits.Domain.Dtos.Requests;

namespace ClubeBeneficios.Benefits.Api.Validators;

public class UpdateBenefitRequestValidator : AbstractValidator<UpdateBenefitRequest>
{
    public UpdateBenefitRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(160);

        RuleFor(x => x.BenefitType)
            .NotEmpty()
            .MaximumLength(80);

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
            .Must(chips => chips == null || chips.Count <= 12)
            .WithMessage("É permitido informar no máximo 12 chips de elegibilidade.");

        RuleForEach(x => x.EligibilityChips)
            .NotEmpty()
            .MaximumLength(80);

        RuleFor(x => x.RecurrenceValue)
            .GreaterThan(0)
            .When(x => x.RecurrenceValue.HasValue);

        RuleFor(x => x)
            .Must(x => !x.StartsAt.HasValue || !x.EndsAt.HasValue || x.EndsAt.Value >= x.StartsAt.Value)
            .WithMessage("A data final deve ser maior ou igual à data inicial.");
    }
}