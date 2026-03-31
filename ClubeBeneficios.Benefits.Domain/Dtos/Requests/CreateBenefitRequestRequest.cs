namespace ClubeBeneficios.Benefits.Domain.Dtos.Requests;

public class CreateBenefitRequestRequest
{
    public Guid BenefitId { get; set; }
    public Guid? PartnerId { get; set; }
    public Guid? RequesterUserId { get; set; }
    public Guid? RequesterPartnerCustomerId { get; set; }
    public string? RequesterType { get; set; }
    public Guid? PetId { get; set; }
    public Guid? AccessCodeId { get; set; }
    public DateTime? ScheduledFor { get; set; }
    public string? ReviewNotes { get; set; }
}