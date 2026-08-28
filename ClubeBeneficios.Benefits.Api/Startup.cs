using ClubeBeneficios.Benefits.Api.Extensions;
using ClubeBeneficios.Benefits.Domain.Options;
using ClubeBeneficios.Benefits.Infrastructure.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;

namespace ClubeBeneficios.Benefits.Api;

public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddApiControllers();
        services.AddApiSwagger();
        services.AddApiCors();
        services.AddApiAuthentication(Configuration);
        services.AddApiAuthorization();
        services.AddInfrastructure(Configuration);
        services.AddApplicationServices();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseApiExceptionHandling();
        app.UseApiSwagger();

        app.UseHttpsRedirection();

        ConfigureFileStorage(app);

        app.UseRouting();

        app.UseCors("DefaultPolicy");

        app.UseAuthentication();
        app.UseUserContext();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }

    private static void ConfigureFileStorage(IApplicationBuilder app)
    {
        var fileStorageOptions = app.ApplicationServices
            .GetRequiredService<IOptions<FileStorageOptions>>()
            .Value;

        if (string.IsNullOrWhiteSpace(fileStorageOptions.LocalRootPath))
            return;

        Directory.CreateDirectory(fileStorageOptions.LocalRootPath);

        var publicBasePath = string.IsNullOrWhiteSpace(fileStorageOptions.PublicBasePath)
            ? "/uploads"
            : fileStorageOptions.PublicBasePath;

        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(fileStorageOptions.LocalRootPath),
            RequestPath = publicBasePath
        });
    }
}