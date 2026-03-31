using ClubeBeneficios.Benefits.Api.Middleware;

namespace ClubeBeneficios.Benefits.Api.Extensions;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseApiExceptionHandling(this IApplicationBuilder app)
    {
        app.UseMiddleware<ExceptionHandlingMiddleware>();
        return app;
    }

    public static IApplicationBuilder UseUserContext(this IApplicationBuilder app)
    {
        app.UseMiddleware<UserContextMiddleware>();
        return app;
    }
}