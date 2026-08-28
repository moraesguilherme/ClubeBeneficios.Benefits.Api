namespace ClubeBeneficios.Benefits.Domain.Dtos.PublicCatalog;

public class PublicPartnerCatalogDto
{
    public PublicPartnerCatalogLinkDto? Catalog { get; set; }
    public IReadOnlyCollection<PublicPartnerCatalogBenefitDto> Benefits { get; set; } =
        Array.Empty<PublicPartnerCatalogBenefitDto>();
}

public class PublicPartnerCatalogLinkDto
{
    public Guid? Id { get; set; }
    public Guid? PartnerId { get; set; }

    public string? PartnerName { get; set; }
    public string? PartnerLogoUrl { get; set; }
    public string? PartnerSegment { get; set; }
    public string? PartnerCategory { get; set; }
    public string? PartnerServiceRegion { get; set; }

    public string? Slug { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Status { get; set; }

    public bool Found { get; set; }
}

public class PublicPartnerCatalogBenefitDto
{
    public Guid Id { get; set; }
    public Guid PartnerId { get; set; }

    public string? PartnerName { get; set; }
    public string? Title { get; set; }
    public string? BenefitType { get; set; }
    public string? Direction { get; set; }
    public string? TargetActorType { get; set; }

    public string? ShortDescription { get; set; }
    public string? FullDescription { get; set; }

    public string? EligibilityType { get; set; }
    public string? RecurrenceType { get; set; }
    public int? RecurrenceValue { get; set; }
    public string? RecurrencePeriod { get; set; }

    public string? ValidityType { get; set; }
    public DateTime? StartsAt { get; set; }
    public DateTime? EndsAt { get; set; }

    public bool RequiresManualRelease { get; set; }
    public bool RequiresActiveAccessCode { get; set; }
    public bool RequiresMatilhaAcceptanceRules { get; set; }

    public string? StackingRule { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}