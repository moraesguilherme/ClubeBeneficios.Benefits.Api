namespace ClubeBeneficios.Benefits.Domain.Dtos.Partner
{
    public class PartnerBenefitRequestListItemDto
    {
        public Guid Id { get; set; }

        public Guid BenefitId { get; set; }
        public string? BenefitTitle { get; set; }
        public string? BenefitType { get; set; }
        public string? BenefitDirection { get; set; }
        public string? TargetActorType { get; set; }

        public string? OperationalOwner { get; set; }
        public string? ProviderLabel { get; set; }

        public Guid? PartnerId { get; set; }
        public string? PartnerName { get; set; }

        public string? RequesterType { get; set; }
        public string? RequesterName { get; set; }
        public string? RequesterEmail { get; set; }
        public string? RequesterPhone { get; set; }

        public string? PetName { get; set; }
        public string? PetBreed { get; set; }
        public string? PetSex { get; set; }
        public string? PetBehaviorStatus { get; set; }

        public string? RequestStatus { get; set; }
        public bool ReviewRequired { get; set; }
        public string? ApprovalStatus { get; set; }

        public DateTime RequestedAt { get; set; }
        public DateTime? ScheduledFor { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
