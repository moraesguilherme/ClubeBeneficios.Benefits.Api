using ClubeBeneficios.Benefits.Domain.Dtos;
using ClubeBeneficios.Benefits.Domain.Dtos.BenefitRequests;
using ClubeBeneficios.Benefits.Domain.Dtos.BenefitUsages.Confirmations;
using ClubeBeneficios.Benefits.Domain.Dtos.Common;
using ClubeBeneficios.Benefits.Domain.Dtos.Requests;
using ClubeBeneficios.Benefits.Domain.Dtos.Requests.BenefitRequests;
using ClubeBeneficios.Benefits.Domain.Dtos.Requests.BenefitUsages.Confirmations;

namespace ClubeBeneficios.Benefits.Domain.Services;

public interface IBenefitRequestService
{
    Task<Guid> CreateAsync(
        CreateBenefitUsageRequest request,
        CancellationToken cancellationToken = default);

    Task SubmitHealthAsync(
        Guid requestId,
        SubmitBenefitRequestHealthRequest request,
        CancellationToken cancellationToken = default);

    Task AddReviewAsync(
        Guid requestId,
        AddBenefitUsageRequestReviewRequest request,
        CancellationToken cancellationToken = default);

    Task ApproveAsync(
        Guid requestId,
        AddBenefitUsageRequestReviewRequest request,
        CancellationToken cancellationToken = default);

    Task RequestChangesAsync(
        Guid requestId,
        AddBenefitUsageRequestReviewRequest request,
        CancellationToken cancellationToken = default);

    Task RejectAsync(
        Guid requestId,
        AddBenefitUsageRequestReviewRequest request,
        CancellationToken cancellationToken = default);

    Task ChangeStatusAsync(
        Guid requestId,
        ChangeBenefitUsageRequestStatusRequest request,
        CancellationToken cancellationToken = default);

    Task<BenefitUsageRequestDetailDto?> GetByIdAsync(
        Guid requestId,
        CancellationToken cancellationToken = default);

    Task<PagedResultDto<BenefitUsageRequestListItemDto>> SearchAdminAsync(
        BenefitUsageRequestFilterDto filter,
        CancellationToken cancellationToken = default);

    Task<PagedResultDto<BenefitUsageRequestListItemDto>> SearchPartnerAsync(
        BenefitUsageRequestFilterDto filter,
        CancellationToken cancellationToken = default);

    Task<PagedResultDto<BenefitUsageRequestListItemDto>> SearchPendingReviewAsync(
        BenefitUsageRequestFilterDto filter,
        CancellationToken cancellationToken = default);

    Task<BenefitRequestApprovalSummaryDto> GetApprovalSummaryAsync(
        CancellationToken cancellationToken = default);

    Task<BenefitUsageConfirmationPairResultDto> CreateUsageConfirmationPairAsync(
    Guid requestId,
    CreateBenefitUsageConfirmationLinksRequest request,
    CancellationToken cancellationToken = default);

    Task<BenefitUsageConfirmationTokenDto?> GetUsageConfirmationByTokenAsync(
        string token,
        CancellationToken cancellationToken = default);

    Task<ConfirmBenefitUsageConfirmationResultDto> ConfirmUsageConfirmationAsync(
        string token,
        CancellationToken cancellationToken = default);
}