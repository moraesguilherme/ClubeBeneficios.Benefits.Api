using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using ClubeBeneficios.Benefits.Domain.Security;

namespace ClubeBeneficios.Benefits.Infrastructure.Authentication;

public class CurrentUserAccessor : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserAccessor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public Guid? UserId => TryParseGuid(User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? User?.FindFirstValue("sub"));
    public Guid? PartnerId => TryParseGuid(User?.FindFirstValue("partner_id"));
    public Guid? SessionId => TryParseGuid(User?.FindFirstValue("session_id") ?? User?.FindFirstValue("sid"));
    public string? Role => User?.FindFirstValue(ClaimTypes.Role) ?? User?.FindFirstValue("role");
    public string? Origin => User?.FindFirstValue("origin");
    public string? Email => User?.FindFirstValue(ClaimTypes.Email) ?? User?.FindFirstValue("email");
    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;

    private static Guid? TryParseGuid(string? value)
    {
        return Guid.TryParse(value, out var parsed) ? parsed : null;
    }
}