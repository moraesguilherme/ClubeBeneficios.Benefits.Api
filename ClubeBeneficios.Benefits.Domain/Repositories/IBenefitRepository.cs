using System;
using System.Threading;
using System.Threading.Tasks;
using ClubeBeneficios.Benefits.Domain.Dtos.Benefits;
using ClubeBeneficios.Benefits.Domain.Dtos.Common;
using ClubeBeneficios.Benefits.Domain.Dtos.Requests.Benefits;
using ClubeBeneficios.Benefits.Domain.Dtos.Requests.Filters;

namespace ClubeBeneficios.Benefits.Domain.Repositories;

public interface IBenefitRepository
{
    Task<PagedResultDto<BenefitOfferListItemDto>> GetPagedAsync(
        BenefitFilterDto filter,
        CancellationToken cancellationToken = default);

    Task<PagedResultDto<BenefitOfferApprovalItemDto>> GetPendingAsync(
        BenefitFilterDto filter,
        CancellationToken cancellationToken = default);

    Task<BenefitOfferDashboardSummaryDto> GetDashboardSummaryAsync(
        CancellationToken cancellationToken = default);

    Task<BenefitOfferFilterOptionsDto> GetFilterOptionsAsync(
        CancellationToken cancellationToken = default);

    Task<BenefitOfferDetailsDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<Guid> CreateAsync(
        CreateBenefitOfferRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(
        Guid id,
        UpdateBenefitOfferRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> ChangeStatusAsync(
        Guid id,
        ChangeBenefitOfferStatusRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> AddReviewAsync(
        Guid id,
        AddBenefitOfferReviewRequest request,
        CancellationToken cancellationToken = default);
}