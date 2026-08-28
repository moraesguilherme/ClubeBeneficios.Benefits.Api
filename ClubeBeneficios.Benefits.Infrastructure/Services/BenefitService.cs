using System;
using System.Threading;
using System.Threading.Tasks;
using ClubeBeneficios.Benefits.Domain.Dtos.Benefits;
using ClubeBeneficios.Benefits.Domain.Dtos.Common;
using ClubeBeneficios.Benefits.Domain.Dtos.Requests.Benefits;
using ClubeBeneficios.Benefits.Domain.Dtos.Requests.Filters;
using ClubeBeneficios.Benefits.Domain.Repositories;
using ClubeBeneficios.Benefits.Domain.Services;

namespace ClubeBeneficios.Benefits.Infrastructure.Services;

public class BenefitService : IBenefitService
{
    private readonly IBenefitRepository _benefitRepository;

    public BenefitService(IBenefitRepository benefitRepository)
    {
        _benefitRepository = benefitRepository;
    }

    public Task<PagedResultDto<BenefitOfferListItemDto>> GetPagedAsync(
        BenefitFilterDto filter,
        CancellationToken cancellationToken = default)
        => _benefitRepository.GetPagedAsync(filter, cancellationToken);

    public Task<PagedResultDto<BenefitOfferApprovalItemDto>> GetPendingAsync(
        BenefitFilterDto filter,
        CancellationToken cancellationToken = default)
        => _benefitRepository.GetPendingAsync(filter, cancellationToken);

    public Task<BenefitOfferDashboardSummaryDto> GetDashboardSummaryAsync(
        CancellationToken cancellationToken = default)
        => _benefitRepository.GetDashboardSummaryAsync(cancellationToken);

    public Task<BenefitOfferFilterOptionsDto> GetFilterOptionsAsync(
        CancellationToken cancellationToken = default)
        => _benefitRepository.GetFilterOptionsAsync(cancellationToken);

    public Task<BenefitOfferDetailsDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
        => _benefitRepository.GetByIdAsync(id, cancellationToken);

    public Task<Guid> CreateAsync(
        CreateBenefitOfferRequest request,
        CancellationToken cancellationToken = default)
        => _benefitRepository.CreateAsync(request, cancellationToken);

    public Task<Guid> CreatePartnerAsync(
    CreateBenefitOfferRequest request,
    Guid partnerId,
    Guid? createdByUserId,
    CancellationToken cancellationToken = default)
    {
        request.PartnerId = partnerId;
        request.CreatedByUserId = createdByUserId;

        request.Status = "pending_review";

        request.HighlightInShowcase = false;

        if (request.Direction == "matilha_to_partner")
        {
            request.RequiresAccessCode = true;
            request.CodeValidationMode = "partner_code";

            if (request.EligibilityType == "open")
            {
                request.EligibilityType = "code";
            }
        }

        return _benefitRepository.CreateAsync(request, cancellationToken);
    }

    public Task<bool> UpdateAsync(
        Guid id,
        UpdateBenefitOfferRequest request,
        CancellationToken cancellationToken = default)
        => _benefitRepository.UpdateAsync(id, request, cancellationToken);

    public async Task<bool> UpdatePartnerAsync(
        Guid id,
        UpdateBenefitOfferRequest request,
        Guid partnerId,
        Guid? updatedByUserId,
        CancellationToken cancellationToken = default)
    {
        var current = await _benefitRepository.GetByIdAsync(id, cancellationToken);

        if (current is null)
            return false;

        if (current.PartnerId != partnerId)
            throw new UnauthorizedAccessException("Este benefício não pertence ao parceiro autenticado.");

        var editableStatuses = new[]
            {
                "draft",
                "pending_review",
                "under_review",
                "rejected"
            };

        if (!editableStatuses.Contains(current.Status))
        {
            throw new InvalidOperationException(
                "Este benefício não pode ser editado pelo parceiro no status atual.");
        }

        request.PartnerId = partnerId;
        request.UpdatedByUserId = updatedByUserId;

        request.Status = "pending_review";

        request.HighlightInShowcase = false;

        if (request.Direction == "matilha_to_partner")
        {
            request.RequiresAccessCode = true;
            request.CodeValidationMode = "partner_code";

            if (request.EligibilityType == "open")
            {
                request.EligibilityType = "code";
            }
        }

        return await _benefitRepository.UpdateAsync(id, request, cancellationToken);
    }

    public Task<bool> ChangeStatusAsync(
        Guid id,
        ChangeBenefitOfferStatusRequest request,
        CancellationToken cancellationToken = default)
        => _benefitRepository.ChangeStatusAsync(id, request, cancellationToken);

    public Task<bool> AddReviewAsync(
        Guid id,
        AddBenefitOfferReviewRequest request,
        CancellationToken cancellationToken = default)
        => _benefitRepository.AddReviewAsync(id, request, cancellationToken);
}