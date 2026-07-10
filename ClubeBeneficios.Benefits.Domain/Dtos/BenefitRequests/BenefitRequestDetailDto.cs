using ClubeBeneficios.Benefits.Domain.Dtos.Requests;

namespace ClubeBeneficios.Benefits.Domain.Dtos.BenefitRequests;

public class BenefitRequestDetailDto
{
    public Guid Id { get; set; }

    public Guid BenefitId { get; set; }
    public string? BenefitTitle { get; set; }
    public string? BenefitStatus { get; set; }
    public string? BenefitType { get; set; }
    public string? BenefitDirection { get; set; }
    public string? TargetActorType { get; set; }
    public string? EligibilityType { get; set; }

    public string? OperationalOwner { get; set; }
    public string? ProviderLabel { get; set; }

    public Guid? PartnerId { get; set; }
    public string? PartnerName { get; set; }
    public string? PartnerSegment { get; set; }
    public string? PartnerCategory { get; set; }

    public string? RequesterType { get; set; }
    public Guid? RequesterUserId { get; set; }
    public Guid? RequesterClientId { get; set; }
    public Guid? RequesterPartnerCustomerId { get; set; }
    public Guid? RequestedByUserId { get; set; }

    public string? ClientName { get; set; }
    public string? ClientDocument { get; set; }
    public string? ClientEmail { get; set; }
    public string? ClientPhone { get; set; }
    public string? ClientStatus { get; set; }

    public string? PartnerCustomerName { get; set; }
    public string? PartnerCustomerDocument { get; set; }
    public string? PartnerCustomerEmail { get; set; }
    public string? PartnerCustomerPhone { get; set; }
    public string? PartnerCustomerStatus { get; set; }
    public string? PartnerCustomerRegistrationStage { get; set; }

    public string? RequesterName { get; set; }
    public string? RequesterEmail { get; set; }
    public string? RequesterPhone { get; set; }

    public string? PetSourceType { get; set; }
    public Guid? RequesterClientPetId { get; set; }
    public Guid? RequesterPartnerCustomerPetId { get; set; }

    public string? PetName { get; set; }
    public string? PetSpecies { get; set; }
    public string? PetBreed { get; set; }
    public string? PetSex { get; set; }
    public int? PetAgeMonths { get; set; }
    public decimal? PetWeightKg { get; set; }
    public string? PetSize { get; set; }
    public bool? PetIsNeutered { get; set; }
    public string? PetBehaviorStatus { get; set; }
    public string? PetStatus { get; set; }

    public Guid? AccessCodeId { get; set; }
    public string? AccessCode { get; set; }

    public string? RequestStatus { get; set; }
    public bool ReviewRequired { get; set; }
    public string? ApprovalStatus { get; set; }
    public DateTime? ApprovalRequestedAt { get; set; }
    public DateTime? ApprovalDecidedAt { get; set; }
    public Guid? ApprovalDecidedByUserId { get; set; }
    public string? ApprovalReason { get; set; }

    public DateTime? ReviewedAt { get; set; }
    public Guid? ReviewedByUserId { get; set; }
    public string? ReviewNotes { get; set; }

    public string? LatestReviewStatus { get; set; }
    public string? LatestReviewPoint { get; set; }
    public string? LatestReviewRecommendation { get; set; }
    public Guid? LatestReviewedByUserId { get; set; }
    public DateTime? LatestReviewedAt { get; set; }

    public string? RequestHealthReviewStatus { get; set; }

    public BenefitRequestDocumentDto? VaccinationCard { get; set; }
    public BenefitRequestPreventiveDto? Dewormer { get; set; }
    public BenefitRequestPreventiveDto? FleaTick { get; set; }

    public DateTime RequestedAt { get; set; }
    public DateTime? ScheduledFor { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? UsageId { get; set; }
    public string? UsageStatus { get; set; }
    public DateTime? UsedAt { get; set; }
    public decimal? MonetaryValue { get; set; }
    public decimal? DiscountValue { get; set; }
    public List<BenefitRequestTimelineEventDto> TimelineEvents { get; set; } = new();

    public List<BenefitRequestReviewDto> Reviews { get; set; } = new();
}