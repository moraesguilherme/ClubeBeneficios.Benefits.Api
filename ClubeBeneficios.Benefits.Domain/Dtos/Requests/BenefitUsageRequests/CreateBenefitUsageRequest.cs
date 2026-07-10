using System;

namespace ClubeBeneficios.Benefits.Domain.Dtos.Requests.BenefitRequests;

public class CreateBenefitUsageRequest
{
    public Guid BenefitId { get; set; }

    public string? RequesterType { get; set; }

    public Guid? RequesterUserId { get; set; }
    public Guid? RequesterClientId { get; set; }
    public Guid? RequesterPartnerCustomerId { get; set; }
    public Guid? RequestedByUserId { get; set; }

    public Guid? AccessCodeId { get; set; }

    public string? PetSourceType { get; set; }
    public Guid? RequesterClientPetId { get; set; }
    public Guid? RequesterPartnerCustomerPetId { get; set; }

    public DateTime? ScheduledFor { get; set; }
    public DateTime? ExpiresAt { get; set; }

    public bool ReviewRequired { get; set; }
}