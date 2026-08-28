using ClubeBeneficios.Benefits.Domain.Dtos.PublicCatalog;
using ClubeBeneficios.Benefits.Domain.Repositories;
using ClubeBeneficios.Benefits.Domain.Services;
using Dapper;
using System.Data.Common;

namespace ClubeBeneficios.Benefits.Infrastructure.Services;

public class PublicPartnerCatalogService : IPublicPartnerCatalogService
{
    private readonly IPublicPartnerCatalogRepository _repository;

    public PublicPartnerCatalogService(IPublicPartnerCatalogRepository repository)
    {
        _repository = repository;
    }

    public Task<PublicPartnerCatalogDto?> GetBySlugAsync(
        string slug,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(slug))
            return Task.FromResult<PublicPartnerCatalogDto?>(null);

        return _repository.GetBySlugAsync(slug.Trim(), cancellationToken);
    }

    public Task<PublicPartnerBenefitRequestCreatedDto> CreateRequestAsync(
        string slug,
        Guid benefitId,
        CreatePublicPartnerBenefitRequestDto request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(slug))
            throw new ArgumentException("Vitrine pública inválida.");

        if (benefitId == Guid.Empty)
            throw new ArgumentException("Benefício inválido.");

        if (request is null)
            throw new ArgumentException("Dados da solicitação não informados.");

        if (string.IsNullOrWhiteSpace(request.CustomerName)
            && string.IsNullOrWhiteSpace(request.CustomerEmail)
            && string.IsNullOrWhiteSpace(request.CustomerPhone))
        {
            throw new ArgumentException("Informe ao menos nome, e-mail ou telefone.");
        }

        if (!request.AcceptedTerms)
            throw new ArgumentException("É necessário aceitar os termos para solicitar o benefício.");

        if (!request.AcceptedPrivacyPolicy)
            throw new ArgumentException("É necessário aceitar a política de privacidade para solicitar o benefício.");

        return _repository.CreateRequestAsync(
            slug.Trim(),
            benefitId,
            request,
            cancellationToken);
    }

    public async Task<PublicPartnerBenefitRequestCreatedDto> CreateRequestFromFormAsync(
        string slug,
        Guid benefitId,
        CreatePublicPartnerBenefitRequestFormDto form,
        CancellationToken cancellationToken = default)
    {
        if (form is null)
            throw new ArgumentException("Dados da solicitação não informados.");

        var ageYears = form.PetAgeYears.GetValueOrDefault();
        var ageMonthsAdditional = form.PetAgeMonthsAdditional.GetValueOrDefault();

        if (ageYears < 0 || ageMonthsAdditional < 0 || ageMonthsAdditional > 11)
            throw new ArgumentException("Informe uma idade válida para o pet.");

        var totalAgeMonths =
            form.PetAgeYears.HasValue || form.PetAgeMonthsAdditional.HasValue
                ? (ageYears * 12) + ageMonthsAdditional
                : (int?)null;

        var request = new CreatePublicPartnerBenefitRequestDto
        {
            CustomerName = form.CustomerName,
            CustomerEmail = form.CustomerEmail,
            CustomerPhone = form.CustomerPhone,
            CustomerDocument = form.CustomerDocument,

            PetName = form.PetName,
            PetBreed = form.PetBreed,
            PetSex = form.PetSex,
            PetAgeYears = form.PetAgeYears,
            PetAgeMonthsAdditional = form.PetAgeMonthsAdditional,
            PetAgeMonths = totalAgeMonths,
            PetSize = form.PetSize,
            PetIsNeutered = form.PetIsNeutered,

            AcceptedTerms = form.AcceptedTerms,
            AcceptedPrivacyPolicy = form.AcceptedPrivacyPolicy,
            CustomerNotes = form.CustomerNotes
        };

        return await CreateRequestAsync(
            slug,
            benefitId,
            request,
            cancellationToken);
    }

    public Task<Guid> InsertPartnerCustomerDocumentAsync(
        Guid partnerCustomerId,
        Guid? partnerCustomerPetId,
        string documentType,
        string fileUrl,
        string? fileName,
        string? mimeType,
        CancellationToken cancellationToken = default)
    {
        if (partnerCustomerId == Guid.Empty)
            throw new ArgumentException("Cliente do parceiro inválido.");

        if (string.IsNullOrWhiteSpace(documentType))
            throw new ArgumentException("Tipo de documento inválido.");

        if (string.IsNullOrWhiteSpace(fileUrl))
            throw new ArgumentException("Arquivo inválido.");

        return _repository.InsertPartnerCustomerDocumentAsync(
            partnerCustomerId,
            partnerCustomerPetId,
            documentType,
            fileUrl,
            fileName,
            mimeType,
            cancellationToken);
    }

    public Task LinkVaccinationCardToBenefitRequestAsync(
        Guid benefitRequestId,
        Guid partnerCustomerDocumentId,
        string fileUrl,
        string? fileName,
        CancellationToken cancellationToken = default)
    {
        if (benefitRequestId == Guid.Empty)
            throw new ArgumentException("Solicitação inválida.");

        if (partnerCustomerDocumentId == Guid.Empty)
            throw new ArgumentException("Documento inválido.");

        if (string.IsNullOrWhiteSpace(fileUrl))
            throw new ArgumentException("Arquivo inválido.");

        return _repository.LinkVaccinationCardToBenefitRequestAsync(
            benefitRequestId,
            partnerCustomerDocumentId,
            fileUrl,
            fileName,
            cancellationToken);
    }

    public Task UpsertBenefitRequestPreventiveAsync(
        Guid benefitRequestId,
        string preventiveType,
        string? brandName,
        DateTime? appliedAt,
        DateTime? expiresAt,
        CancellationToken cancellationToken = default)
    {
        if (benefitRequestId == Guid.Empty)
            throw new ArgumentException("Solicitação inválida.");

        if (preventiveType is not "dewormer" and not "flea_tick")
            throw new ArgumentException("Tipo de preventivo inválido.");

        if (string.IsNullOrWhiteSpace(brandName))
            throw new ArgumentException("Informe a marca do preventivo.");

        if (!appliedAt.HasValue)
            throw new ArgumentException("Informe a data de aplicação do preventivo.");

        if (!expiresAt.HasValue)
            throw new ArgumentException("Informe a validade do preventivo.");

        if (expiresAt.Value.Date < appliedAt.Value.Date)
            throw new ArgumentException("A validade do preventivo não pode ser anterior à data de aplicação.");

        return _repository.UpsertBenefitRequestPreventiveAsync(
            benefitRequestId,
            preventiveType,
            brandName,
            appliedAt,
            expiresAt,
            cancellationToken);
    }

    public Task EnqueueRequestNotificationAsync(
        Guid requestId,
        string eventType,
        string? reason = null,
        CancellationToken cancellationToken = default)
    {
        if (requestId == Guid.Empty)
            throw new ArgumentException("Solicitação inválida.");

        if (string.IsNullOrWhiteSpace(eventType))
            throw new ArgumentException("Tipo de evento inválido.");

        return _repository.EnqueueRequestNotificationAsync(
            requestId,
            eventType,
            reason,
            cancellationToken);
    }
}