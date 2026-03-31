namespace ClubeBeneficios.Benefits.Domain.Dtos;

public class BenefitRequestListItemDto
{
    public Guid Id { get; set; }
    public Guid BenefitId { get; set; }
    public Guid? PartnerId { get; set; }
    public Guid? RequesterUserId { get; set; }
    public Guid? RequesterPartnerCustomerId { get; set; }
    public string? RequesterType { get; set; }
    public string? RequestStatus { get; set; }
    public DateTime RequestedAt { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public DateTime? ScheduledFor { get; set; }
    public string? BenefitTitle { get; set; }
    public string? PartnerName { get; set; }
}