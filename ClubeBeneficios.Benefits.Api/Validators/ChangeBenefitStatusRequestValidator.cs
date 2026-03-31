using FluentValidation;
using ClubeBeneficios.Benefits.Domain.Dtos.Requests;

namespace ClubeBeneficios.Benefits.Api.Validators;

public class ChangeBenefitStatusRequestValidator : AbstractValidator<ChangeBenefitStatusRequest>
{
    public ChangeBenefitStatusRequestValidator()
    {
        RuleFor(x => x.Status)
            .NotEmpty()
            .MaximumLength(40);

        RuleFor(x => x.Reason)
            .MaximumLength(500);
    }
}