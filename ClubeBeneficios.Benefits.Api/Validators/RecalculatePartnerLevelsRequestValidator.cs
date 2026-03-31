using FluentValidation;
using ClubeBeneficios.Benefits.Domain.Dtos.Requests;

namespace ClubeBeneficios.Benefits.Api.Validators;

public class RecalculatePartnerLevelsRequestValidator : AbstractValidator<RecalculatePartnerLevelsRequest>
{
    public RecalculatePartnerLevelsRequestValidator()
    {
        RuleFor(x => x.PartnerId)
            .NotEmpty()
            .When(x => x.PartnerId.HasValue);
    }
}