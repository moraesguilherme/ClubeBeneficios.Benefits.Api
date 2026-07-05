using ClubeBeneficios.Benefits.Domain.Dtos;
using ClubeBeneficios.Benefits.Domain.Dtos.Partner;

namespace ClubeBeneficios.Benefits.Api.Mappers;

public static class PartnerBenefitsMapper
{
    public static PagedResultDto<PartnerBenefitListItemDto> ToPartnerPagedResult(
        this PagedResultDto<BenefitListItemDto> source)
    {
        return new PagedResultDto<PartnerBenefitListItemDto>
        {
            Items = source.Items.Select(x => x.ToPartnerDto()).ToList(),
            Page = source.Page,
            PageSize = source.PageSize,
            TotalCount = source.TotalCount
        };
    }

    public static PartnerBenefitListItemDto ToPartnerDto(this BenefitListItemDto source)
    {
        return new PartnerBenefitListItemDto
        {
            Id = source.Id,
            PartnerId = source.PartnerId,
            PartnerName = source.PartnerName,

            Title = source.Title,
            BenefitType = source.BenefitType,

            Direction = source.Direction,
            DirectionLabel = source.DirectionLabel,

            Status = source.Status,

            TargetActorType = source.TargetActorType,
            TargetActorLabel = source.TargetActorLabel,

            EligibilityType = source.EligibilityType,
            EligibilitySummary = source.EligibilitySummary,

            RecurrenceType = source.RecurrenceType,
            RecurrencePeriod = source.RecurrencePeriod,
            RecurrenceLabel = source.RecurrenceLabel,

            ValidityType = source.ValidityType,
            ValidityLabel = source.ValidityLabel,

            HighlightInShowcase = source.HighlightInShowcase,
            AutoActivateWhenApproved = source.AutoActivateWhenApproved,

            RequestsCount = source.RequestsCount,
            UsagesCount = source.UsagesCount,
            ConversionRate = source.ConversionRate,

            CreatedAt = source.CreatedAt,
            UpdatedAt = source.UpdatedAt
        };
    }

    public static PartnerBenefitDetailsDto ToPartnerDto(this BenefitDetailsDto source)
    {
        return new PartnerBenefitDetailsDto
        {
            Id = source.Id,
            PartnerId = source.PartnerId,
            PartnerName = source.PartnerName,

            Title = source.Title,
            Slug = source.Slug,
            BenefitType = source.BenefitType,
            Direction = source.Direction,
            TargetActorType = source.TargetActorType,

            ShortDescription = source.ShortDescription,
            FullDescription = source.FullDescription,

            EligibilityType = source.EligibilityType,
            EligibilitySummary = source.EligibilitySummary,

            RecurrenceType = source.RecurrenceType,
            RecurrenceLimit = source.RecurrenceLimit,
            RecurrencePeriod = source.RecurrencePeriod,

            ValidFrom = source.ValidFrom,
            ValidUntil = source.ValidUntil,

            AutoActivateWhenApproved = source.AutoActivateWhenApproved,
            RequiresManualRelease = source.RequiresManualRelease,
            HighlightInShowcase = source.HighlightInShowcase,

            Status = source.Status,
            StackingRule = source.StackingRule,

            CreatedAt = source.CreatedAt,
            UpdatedAt = source.UpdatedAt
        };
    }

    public static PagedResultDto<PartnerBenefitRequestListItemDto> ToPartnerPagedResult(
        this PagedResultDto<BenefitRequestListItemDto> source)
    {
        return new PagedResultDto<PartnerBenefitRequestListItemDto>
        {
            Items = source.Items.Select(x => x.ToPartnerDto()).ToList(),
            Page = source.Page,
            PageSize = source.PageSize,
            TotalCount = source.TotalCount
        };
    }

    public static PartnerBenefitRequestListItemDto ToPartnerDto(this BenefitRequestListItemDto source)
    {
        return new PartnerBenefitRequestListItemDto
        {
            Id = source.Id,

            BenefitId = source.BenefitId,
            BenefitTitle = source.BenefitTitle,
            BenefitType = source.BenefitType,
            BenefitDirection = source.BenefitDirection,
            TargetActorType = source.TargetActorType,

            OperationalOwner = source.OperationalOwner,
            ProviderLabel = source.ProviderLabel,

            PartnerId = source.PartnerId,
            PartnerName = source.PartnerName,

            RequesterType = source.RequesterType,
            RequesterName = source.RequesterName,
            RequesterEmail = source.RequesterEmail,
            RequesterPhone = source.RequesterPhone,

            PetName = source.PetName,
            PetBreed = source.PetBreed,
            PetSex = source.PetSex,
            PetBehaviorStatus = source.PetBehaviorStatus,

            RequestStatus = source.RequestStatus,
            ReviewRequired = source.ReviewRequired,
            ApprovalStatus = source.ApprovalStatus,

            RequestedAt = source.RequestedAt,
            ScheduledFor = source.ScheduledFor,
            ExpiresAt = source.ExpiresAt,
            CreatedAt = source.CreatedAt,
            UpdatedAt = source.UpdatedAt
        };
    }

    public static PartnerBenefitRequestDetailDto ToPartnerDto(this BenefitRequestDetailDto source)
    {
        return new PartnerBenefitRequestDetailDto
        {
            Id = source.Id,

            BenefitId = source.BenefitId,
            BenefitTitle = source.BenefitTitle,
            BenefitStatus = source.BenefitStatus,
            BenefitType = source.BenefitType,
            BenefitDirection = source.BenefitDirection,
            TargetActorType = source.TargetActorType,

            OperationalOwner = source.OperationalOwner,
            ProviderLabel = source.ProviderLabel,

            PartnerId = source.PartnerId,
            PartnerName = source.PartnerName,

            RequesterType = source.RequesterType,
            RequesterName = source.RequesterName,
            RequesterEmail = source.RequesterEmail,
            RequesterPhone = source.RequesterPhone,

            PetName = source.PetName,
            PetSpecies = source.PetSpecies,
            PetBreed = source.PetBreed,
            PetSex = source.PetSex,
            PetAgeMonths = source.PetAgeMonths,
            PetWeightKg = source.PetWeightKg,
            PetSize = source.PetSize,
            PetIsNeutered = source.PetIsNeutered,
            PetBehaviorStatus = source.PetBehaviorStatus,
            PetStatus = source.PetStatus,

            AccessCodeId = source.AccessCodeId,
            AccessCode = source.AccessCode,

            RequestStatus = source.RequestStatus,
            ReviewRequired = source.ReviewRequired,
            ApprovalStatus = source.ApprovalStatus,

            RequestedAt = source.RequestedAt,
            ScheduledFor = source.ScheduledFor,
            ExpiresAt = source.ExpiresAt,
            CreatedAt = source.CreatedAt,
            UpdatedAt = source.UpdatedAt
        };
    }

    public static PagedResultDto<PartnerBenefitUsageListItemDto> ToPartnerPagedResult(
        this PagedResultDto<BenefitUsageListItemDto> source)
    {
        return new PagedResultDto<PartnerBenefitUsageListItemDto>
        {
            Items = source.Items.Select(x => x.ToPartnerDto()).ToList(),
            Page = source.Page,
            PageSize = source.PageSize,
            TotalCount = source.TotalCount
        };
    }

    public static PartnerBenefitUsageListItemDto ToPartnerDto(this BenefitUsageListItemDto source)
    {
        return new PartnerBenefitUsageListItemDto
        {
            Id = source.Id,

            BenefitId = source.BenefitId,
            BenefitRequestId = source.BenefitRequestId,

            PartnerId = source.PartnerId,
            PartnerName = source.PartnerName,

            UsedByType = source.UsedByType,
            UsedByName = source.UsedByName,

            PetSourceType = source.PetSourceType,
            PetName = source.PetName,

            UsageStatus = source.UsageStatus,
            UsedAt = source.UsedAt,

            SnapshotTitle = source.SnapshotTitle,
            SnapshotPartnerName = source.SnapshotPartnerName,
            SnapshotRuleSummary = source.SnapshotRuleSummary,

            MonetaryValue = source.MonetaryValue,
            DiscountValue = source.DiscountValue,

            CreatedAt = source.CreatedAt,
            UpdatedAt = source.UpdatedAt
        };
    }

    public static PartnerBenefitUsageDetailDto ToPartnerDto(this BenefitUsageDetailDto source)
    {
        return new PartnerBenefitUsageDetailDto
        {
            Id = source.Id,

            BenefitId = source.BenefitId,
            BenefitRequestId = source.BenefitRequestId,

            PartnerId = source.PartnerId,
            PartnerName = source.PartnerName,

            UsedByType = source.UsedByType,
            UsedByName = source.UsedByName,

            PetSourceType = source.PetSourceType,
            PetName = source.PetName,

            UsageStatus = source.UsageStatus,
            UsedAt = source.UsedAt,

            MonetaryValue = source.MonetaryValue,
            DiscountValue = source.DiscountValue,

            SnapshotTitle = source.SnapshotTitle,
            SnapshotPartnerName = source.SnapshotPartnerName,
            SnapshotRuleSummary = source.SnapshotRuleSummary,

            CreatedAt = source.CreatedAt,
            UpdatedAt = source.UpdatedAt
        };
    }
}