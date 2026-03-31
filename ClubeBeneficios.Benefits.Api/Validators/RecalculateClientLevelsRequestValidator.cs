using FluentValidation;
using ClubeBeneficios.Benefits.Domain.Dtos.Requests;

namespace ClubeBeneficios.Benefits.Api.Validators;

public class RecalculateClientLevelsRequestValidator : AbstractValidator<RecalculateClientLevelsRequest>
{
    public RecalculateClientLevelsRequestValidator()
    {
        RuleFor(x => x.ReferenceDate)
            .LessThanOrEqualTo(DateTime.UtcNow.AddDays(1))
            .When(x => x.ReferenceDate.HasValue)
            .WithMessage("A data de referÃªncia nÃ£o pode ser maior que a data atual.");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .When(x => x.UserId.HasValue);
    }
}