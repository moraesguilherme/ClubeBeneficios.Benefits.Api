using ClubeBeneficios.Benefits.Domain.Dtos.PublicCatalog;

namespace ClubeBeneficios.Benefits.Domain.Services;

public interface IPublicPartnerCatalogService
{
    Task<PublicPartnerCatalogDto?> GetBySlugAsync(
        string slug,
        CancellationToken cancellationToken = default);

    Task<PublicPartnerBenefitRequestCreatedDto> CreateRequestAsync(
        string slug,
        Guid benefitId,
        CreatePublicPartnerBenefitRequestDto request,
        CancellationToken cancellationToken = default);

    Task<PublicPartnerBenefitRequestCreatedDto> CreateRequestFromFormAsync(
        string slug,
        Guid benefitId,
        CreatePublicPartnerBenefitRequestFormDto form,
        CancellationToken cancellationToken = default);

    Task<Guid> InsertPartnerCustomerDocumentAsync(
        Guid partnerCustomerId,
        Guid? partnerCustomerPetId,
        string documentType,
        string fileUrl,
        string? fileName,
        string? mimeType,
        CancellationToken cancellationToken = default);

    Task LinkVaccinationCardToBenefitRequestAsync(
        Guid benefitRequestId,
        Guid partnerCustomerDocumentId,
        string fileUrl,
        string? fileName,
        CancellationToken cancellationToken = default);

    Task UpsertBenefitRequestPreventiveAsync(
        Guid benefitRequestId,
        string preventiveType,
        string? brandName,
        DateTime? appliedAt,
        DateTime? expiresAt,
        CancellationToken cancellationToken = default);

    Task EnqueueRequestNotificationAsync(
        Guid requestId,
        string eventType,
        string? reason = null,
        CancellationToken cancellationToken = default);
}