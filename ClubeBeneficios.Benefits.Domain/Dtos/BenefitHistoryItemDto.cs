namespace ClubeBeneficios.Benefits.Domain.Dtos;

public class BenefitHistoryItemDto
{
    public long Id { get; set; }
    public Guid BenefitId { get; set; }
    public string? EventType { get; set; }
    public string? FromStatus { get; set; }
    public string? ToStatus { get; set; }
    public string? Notes { get; set; }
    public Guid? ChangedByUserId { get; set; }
    public string? ChangedByName { get; set; }
    public DateTime CreatedAt { get; set; }
}