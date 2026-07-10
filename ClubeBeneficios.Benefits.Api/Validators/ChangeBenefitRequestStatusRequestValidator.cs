using FluentValidation;
using ClubeBeneficios.Benefits.Domain.Dtos.Requests.BenefitRequests;

namespace ClubeBeneficios.Benefits.Api.Validators;

public class ChangeBenefitRequestStatusRequestValidator : AbstractValidator<ChangeBenefitUsageRequestStatusRequest>
{
    public ChangeBenefitRequestStatusRequestValidator()
    {
        RuleFor(x => x.RequestStatus).NotEmpty().MaximumLength(50);
        RuleFor(x => x.ReviewNotes).MaximumLength(1000).When(x => !string.IsNullOrWhiteSpace(x.ReviewNotes));
    }
}