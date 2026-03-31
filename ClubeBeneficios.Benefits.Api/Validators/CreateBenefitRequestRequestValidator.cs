using FluentValidation;
using ClubeBeneficios.Benefits.Domain.Dtos.Requests;

namespace ClubeBeneficios.Benefits.Api.Validators;

public class CreateBenefitRequestRequestValidator : AbstractValidator<CreateBenefitRequestRequest>
{
    public CreateBenefitRequestRequestValidator()
    {
        RuleFor(x => x.BenefitId).NotEmpty();
        RuleFor(x => x.RequesterType).NotEmpty().MaximumLength(50);
        RuleFor(x => x.ReviewNotes).MaximumLength(1000).When(x => !string.IsNullOrWhiteSpace(x.ReviewNotes));
    }
}