namespace ClubeBeneficios.Benefits.Domain.Dtos;

public class BenefitRequestFilterDto
{
    public string? Search { get; set; }
    public Guid? BenefitId { get; set; }
    public Guid? PartnerId { get; set; }
    public string? RequestStatus { get; set; }
    public string? RequesterType { get; set; }
    public DateTime? RequestedFrom { get; set; }
    public DateTime? RequestedTo { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}