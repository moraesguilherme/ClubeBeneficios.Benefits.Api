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
        CreateBenefitRequestDto request,
        CancellationToken cancellationToken = default);

    Task SubmitHealthAsync(
        Guid requestId,
        SubmitBenefitRequestHealthRequest request,
        CancellationToken cancellationToken = default);

    Task AddReviewAsync(
        Guid requestId,
        AddBenefitRequestReviewRequest request,
        CancellationToken cancellationToken = default);

    Task ApproveAsync(
        Guid requestId,
        AddBenefitRequestReviewRequest request,
        CancellationToken cancellationToken = default);

    Task RequestChangesAsync(
        Guid requestId,
        AddBenefitRequestReviewRequest request,
        CancellationToken cancellationToken = default);

    Task RejectAsync(
        Guid requestId,
        AddBenefitRequestReviewRequest request,
        CancellationToken cancellationToken = default);

    Task ChangeStatusAsync(
        Guid requestId,
        ChangeBenefitRequestStatusRequest request,
        CancellationToken cancellationToken = default);

    Task<BenefitRequestDetailDto?> GetByIdAsync(
        Guid requestId,
        CancellationToken cancellationToken = default);

    Task<PagedResultDto<BenefitRequestListItemDto>> SearchAdminAsync(
        BenefitRequestFilterDto filter,
        CancellationToken cancellationToken = default);

    Task<PagedResultDto<BenefitRequestListItemDto>> SearchPartnerAsync(
        BenefitRequestFilterDto filter,
        CancellationToken cancellationToken = default);

    Task<PagedResultDto<BenefitRequestListItemDto>> SearchPendingReviewAsync(
        BenefitRequestFilterDto filter,
        CancellationToken cancellationToken = default);

    Task<BenefitRequestApprovalSummaryDto> GetApprovalSummaryAsync(
        CancellationToken cancellationToken = default);

    Task<BenefitUsageConfirmationPairResultDto> CreateUsageConfirmationPairAsync(
    Guid requestId,
    CreateBenefitUsageConfirmationPairRequest request,
    CancellationToken cancellationToken = default);

    Task<BenefitUsageConfirmationTokenDto?> GetUsageConfirmationByTokenAsync(
        string token,
        CancellationToken cancellationToken = default);

    Task<BenefitUsageConfirmationConfirmResultDto> ConfirmUsageConfirmationAsync(
        string token,
        CancellationToken cancellationToken = default);
}