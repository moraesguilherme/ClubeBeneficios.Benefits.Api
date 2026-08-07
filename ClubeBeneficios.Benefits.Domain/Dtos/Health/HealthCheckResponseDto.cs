namespace ClubeBeneficios.Benefits.Domain.Dtos.Health;

public class HealthCheckResponseDto
{
    public string Status { get; set; } = "ok";
    public string Service { get; set; } = "ClubeBeneficios.Benefits.Api";
    public DateTime UtcNow { get; set; }
}