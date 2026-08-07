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

    public Guid? UserId =>
        TryParseGuid(
            FindFirstValue(
                ClaimTypes.NameIdentifier,
                "sub",
                "user_id",
                "userId"));

    public Guid? PartnerId =>
        TryParseGuid(
            FindFirstValue(
                "partner_id",
                "partnerId",
                "partner"));

    public Guid? SessionId =>
        TryParseGuid(
            FindFirstValue(
                "session_id",
                "sessionId",
                "sid"));

    public string? Role =>
        FindFirstValue(
            ClaimTypes.Role,
            "role");

    public string? Origin =>
        FindFirstValue(
            "origin",
            "account_origin",
            "accountOrigin");

    public string? Email =>
        FindFirstValue(
            ClaimTypes.Email,
            "email");

    public bool IsAuthenticated =>
        User?.Identity?.IsAuthenticated ?? false;

    private string? FindFirstValue(params string[] claimTypes)
    {
        if (User is null)
            return null;

        foreach (var claimType in claimTypes)
        {
            var value = User.FindFirstValue(claimType);

            if (!string.IsNullOrWhiteSpace(value))
                return value;
        }

        return null;
    }

    private static Guid? TryParseGuid(string? value)
    {
        return Guid.TryParse(value, out var parsed)
            ? parsed
            : null;
    }
}