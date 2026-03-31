using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ClubeBeneficios.Benefits.Domain.Repositories;
using ClubeBeneficios.Benefits.Domain.Security;
using ClubeBeneficios.Benefits.Domain.Services;
using ClubeBeneficios.Benefits.Infrastructure.Authentication;
using ClubeBeneficios.Benefits.Infrastructure.Repositories;
using ClubeBeneficios.Benefits.Infrastructure.Services;

namespace ClubeBeneficios.Benefits.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
                services.AddProblemDetails();

        services.AddHttpContextAccessor();

        services.AddScoped<ICurrentUser, CurrentUserAccessor>();

        services.AddScoped<IDbConnection>(_ =>
            new SqlConnection(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IBenefitRepository, BenefitRepository>();
        services.AddScoped<IBenefitService, BenefitService>();
        services.AddScoped<IBenefitRequestRepository, BenefitRequestRepository>();
        services.AddScoped<IBenefitRequestService, BenefitRequestService>();
        services.AddScoped<IBenefitUsageRepository, BenefitUsageRepository>();
        services.AddScoped<IBenefitUsageService, BenefitUsageService>();

                services.AddScoped<IBenefitAnalyticsRepository, BenefitAnalyticsRepository>();

                services.AddScoped<IBenefitLevelAutomationRepository, BenefitLevelAutomationRepository>();

                services.AddScoped<IBenefitAnalyticsService, BenefitAnalyticsService>();

                services.AddScoped<IBenefitLevelAutomationService, BenefitLevelAutomationService>();

                services.AddScoped<IBenefitLookupRepository, BenefitLookupRepository>();

                services.AddScoped<IBenefitLookupService, BenefitLookupService>();

        return services;
    }
}