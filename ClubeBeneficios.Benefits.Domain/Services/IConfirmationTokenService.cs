namespace ClubeBeneficios.Benefits.Domain.Services;

public interface IConfirmationTokenService
{
    string GenerateToken();
    string ComputeHash(string token);
}