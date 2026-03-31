using FluentValidation;
using ClubeBeneficios.Benefits.Domain.Dtos.Requests;

namespace ClubeBeneficios.Benefits.Api.Validators;

public class CancelBenefitUsageRequestValidator : AbstractValidator<CancelBenefitUsageRequest>
{
    public CancelBenefitUsageRequestValidator()
    {
        RuleFor(x => x.CancellationReason).MaximumLength(1000).When(x => !string.IsNullOrWhiteSpace(x.CancellationReason));
    }
}