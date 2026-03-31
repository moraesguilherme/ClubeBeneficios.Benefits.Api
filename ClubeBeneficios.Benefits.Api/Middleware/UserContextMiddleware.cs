namespace ClubeBeneficios.Benefits.Api.Middleware;

public class UserContextMiddleware
{
    private readonly RequestDelegate _next;

    public UserContextMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var userId =
                context.User.FindFirst("sub")?.Value ??
                context.User.FindFirst("nameidentifier")?.Value ??
                context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            var role =
                context.User.FindFirst("role")?.Value ??
                context.User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

            if (!string.IsNullOrWhiteSpace(userId))
            {
                context.Items["UserId"] = userId;
            }

            if (!string.IsNullOrWhiteSpace(role))
            {
                context.Items["UserRole"] = role;
            }
        }

        await _next(context);
    }
}