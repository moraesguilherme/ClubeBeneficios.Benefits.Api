using System.Data;
using ClubeBeneficios.Benefits.Domain.Options;
using ClubeBeneficios.Benefits.Domain.Repositories;
using ClubeBeneficios.Benefits.Domain.Security;
using ClubeBeneficios.Benefits.Domain.Services;
using ClubeBeneficios.Benefits.Infrastructure.Authentication;
using ClubeBeneficios.Benefits.Infrastructure.Repositories;
using ClubeBeneficios.Benefits.Infrastructure.Services;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ClubeBeneficios.Benefits.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        DefaultTypeMap.MatchNamesWithUnderscores = true;

        services.AddProblemDetails();

        services.AddHttpContextAccessor();

        services.AddScoped<ICurrentUser, CurrentUserAccessor>();

        services.AddScoped<IDbConnection>(_ =>
            new SqlConnection(configuration.GetConnectionString("DefaultConnection")));

        services.Configure<FileStorageOptions>(
            configuration.GetSection("FileStorage"));

        services.AddScoped<IFileStorageService, LocalFileStorageService>();

        services.AddScoped<IBenefitRepository, BenefitRepository>();
        services.AddScoped<IBenefitService, BenefitService>();

        services.AddScoped<IBenefitRequestRepository, BenefitRequestRepository>();
        services.AddScoped<IBenefitRequestService, BenefitRequestService>();

        services.AddScoped<IBenefitUsageRepository, BenefitUsageRepository>();
        services.AddScoped<IBenefitUsageService, BenefitUsageService>();

        services.AddScoped<IBenefitAnalyticsRepository, BenefitAnalyticsRepository>();
        services.AddScoped<IBenefitAnalyticsService, BenefitAnalyticsService>();

        services.AddScoped<IBenefitLevelAutomationRepository, BenefitLevelAutomationRepository>();
        services.AddScoped<IBenefitLevelAutomationService, BenefitLevelAutomationService>();

        services.AddScoped<IBenefitLookupRepository, BenefitLookupRepository>();
        services.AddScoped<IBenefitLookupService, BenefitLookupService>();

        services.AddScoped<IConfirmationTokenService, ConfirmationTokenService>();

        services.AddScoped<IPublicPartnerCatalogRepository, PublicPartnerCatalogRepository>();
        services.AddScoped<IPublicPartnerCatalogService, PublicPartnerCatalogService>();

        return services;
    }
}