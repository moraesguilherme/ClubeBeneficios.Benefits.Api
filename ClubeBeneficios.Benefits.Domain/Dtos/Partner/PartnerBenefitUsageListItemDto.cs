namespace ClubeBeneficios.Benefits.Domain.Dtos.Partner
{
    public class PartnerBenefitUsageListItemDto
    {
        public Guid Id { get; set; }

        public Guid BenefitId { get; set; }
        public Guid? BenefitRequestId { get; set; }

        public Guid? PartnerId { get; set; }
        public string? PartnerName { get; set; }

        public string? UsedByType { get; set; }
        public string? UsedByName { get; set; }

        public string? PetSourceType { get; set; }
        public string? PetName { get; set; }

        public string? UsageStatus { get; set; }
        public DateTime? UsedAt { get; set; }

        public string? SnapshotTitle { get; set; }
        public string? SnapshotPartnerName { get; set; }
        public string? SnapshotRuleSummary { get; set; }

        public decimal? MonetaryValue { get; set; }
        public decimal? DiscountValue { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
