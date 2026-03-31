using FluentValidation;
using ClubeBeneficios.Benefits.Domain.Dtos.Requests;

namespace ClubeBeneficios.Benefits.Api.Validators;

public class ReviewBenefitRequestValidator : AbstractValidator<ReviewBenefitRequest>
{
    public ReviewBenefitRequestValidator()
    {
        RuleFor(x => x.ReviewStatus)
            .NotEmpty()
            .MaximumLength(40);

        RuleFor(x => x.ReviewPoint)
            .MaximumLength(250);

        RuleFor(x => x.ReviewRecommendation)
            .MaximumLength(2000);
    }
}