namespace ClubeBeneficios.Benefits.Domain.Dtos;

public class BenefitRequestDetailDto
{
    public Guid Id { get; set; }
    public Guid BenefitId { get; set; }
    public Guid? PartnerId { get; set; }
    public Guid? RequesterUserId { get; set; }
    public Guid? RequesterPartnerCustomerId { get; set; }
    public string? RequesterType { get; set; }
    public Guid? PetId { get; set; }
    public Guid? AccessCodeId { get; set; }
    public string? RequestStatus { get; set; }
    public DateTime RequestedAt { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public Guid? ReviewedByUserId { get; set; }
    public string? ReviewNotes { get; set; }
    public DateTime? ScheduledFor { get; set; }
    public string? BenefitTitle { get; set; }
    public string? BenefitDirection { get; set; }
    public string? TargetActorType { get; set; }
    public string? PartnerName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}