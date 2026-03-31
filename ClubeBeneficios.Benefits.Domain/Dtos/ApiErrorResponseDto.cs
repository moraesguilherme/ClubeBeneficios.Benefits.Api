namespace ClubeBeneficios.Benefits.Domain.Dtos;

public class ApiErrorResponseDto
{
    public string? Code { get; set; }
    public string? Message { get; set; }
    public int Status { get; set; }
    public string? TraceId { get; set; }
    public IDictionary<string, string[]>? Errors { get; set; }
}