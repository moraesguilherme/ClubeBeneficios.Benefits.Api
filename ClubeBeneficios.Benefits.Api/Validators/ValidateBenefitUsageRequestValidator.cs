using FluentValidation;
using ClubeBeneficios.Benefits.Domain.Dtos.Requests;

namespace ClubeBeneficios.Benefits.Api.Validators;

public class ValidateBenefitUsageRequestValidator : AbstractValidator<ValidateBenefitUsageRequest>
{
    public ValidateBenefitUsageRequestValidator()
    {
        RuleFor(x => x.BenefitId).NotEmpty();
        RuleFor(x => x.ActorType).NotEmpty().MaximumLength(50);
    }
}