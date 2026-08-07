using FluentValidation;
using ClubeBeneficios.Benefits.Domain.Dtos.Requests.Benefits;

namespace ClubeBeneficios.Benefits.Api.Validators;

public class ChangeBenefitStatusRequestValidator : AbstractValidator<ChangeBenefitOfferStatusRequest>
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