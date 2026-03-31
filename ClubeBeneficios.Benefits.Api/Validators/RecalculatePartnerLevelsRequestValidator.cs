using FluentValidation;
using ClubeBeneficios.Benefits.Domain.Dtos.Requests;

namespace ClubeBeneficios.Benefits.Api.Validators;

public class RecalculatePartnerLevelsRequestValidator : AbstractValidator<RecalculatePartnerLevelsRequest>
{
    public RecalculatePartnerLevelsRequestValidator()
    {
        RuleFor(x => x.ReferenceDate)
            .LessThanOrEqualTo(DateTime.UtcNow.AddDays(1))
            .When(x => x.ReferenceDate.HasValue)
            .WithMessage("A data de referÃªncia nÃ£o pode ser maior que a data atual.");

        RuleFor(x => x.PartnerId)
            .NotEmpty()
            .When(x => x.PartnerId.HasValue);
    }
}