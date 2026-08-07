using FluentValidation;
using FluentValidation.AspNetCore;
using ClubeBeneficios.Benefits.Api.Validators;

namespace ClubeBeneficios.Benefits.Api.Extensions;

public static class ControllersExtensions
{
    public static IServiceCollection AddApiControllers(this IServiceCollection services)
    {
        services.AddControllers();

        services.AddFluentValidationAutoValidation();
        services.AddFluentValidationClientsideAdapters();

        services.AddValidatorsFromAssemblyContaining<ConfirmBenefitUsageRequestValidator>();

        services.AddEndpointsApiExplorer();

        return services;
    }
}