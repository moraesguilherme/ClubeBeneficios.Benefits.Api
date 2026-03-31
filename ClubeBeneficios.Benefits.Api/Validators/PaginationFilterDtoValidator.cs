using FluentValidation;
using ClubeBeneficios.Benefits.Domain.Dtos.Requests;

namespace ClubeBeneficios.Benefits.Api.Validators;

public class PaginationFilterDtoValidator : AbstractValidator<PaginationFilterDto>
{
    public PaginationFilterDtoValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0)
            .WithMessage("A pÃ¡gina deve ser maior que zero.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 200)
            .WithMessage("O tamanho da pÃ¡gina deve estar entre 1 e 200.");
    }
}