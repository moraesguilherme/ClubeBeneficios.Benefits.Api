namespace ClubeBeneficios.Benefits.Api.Extensions;

public static class CorsExtensions
{
    public static IServiceCollection AddApiCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("DefaultPolicy", policy =>
            {
                policy
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        return services;
    }
}