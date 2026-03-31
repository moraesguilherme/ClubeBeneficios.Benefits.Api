using FluentValidation;
using ClubeBeneficios.Benefits.Api.Validators;

namespace ClubeBeneficios.Benefits.Api.Extensions;

public static class ApplicationServicesExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<RecalculatePartnerLevelsRequestValidator>();
        return services;
    }
}