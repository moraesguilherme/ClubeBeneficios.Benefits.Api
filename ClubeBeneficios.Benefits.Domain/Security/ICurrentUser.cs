namespace ClubeBeneficios.Benefits.Domain.Security;

public interface ICurrentUser
{
    Guid? UserId { get; }
    Guid? PartnerId { get; }
    Guid? SessionId { get; }
    string? Role { get; }
    string? Origin { get; }
    string? Email { get; }
    bool IsAuthenticated { get; }
}