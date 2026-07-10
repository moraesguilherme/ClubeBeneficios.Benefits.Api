using ClubeBeneficios.Benefits.Domain.Dtos;
using ClubeBeneficios.Benefits.Domain.Dtos.BenefitRequests;
using ClubeBeneficios.Benefits.Domain.Dtos.BenefitUsages.Confirmations;
using ClubeBeneficios.Benefits.Domain.Dtos.Common;
using ClubeBeneficios.Benefits.Domain.Dtos.Requests;
using ClubeBeneficios.Benefits.Domain.Dtos.Requests.BenefitRequests;

namespace ClubeBeneficios.Benefits.Domain.Repositories;

public interface IBenefitRequestRepository
{
    Task<Guid> CreateAsync(
        CreateBenefitUsageRequest request,
        Guid? performedByUserId,
        CancellationToken cancellationToken = default);

    Task SubmitHealthAsync(
        Guid requestId,
        SubmitBenefitRequestHealthRequest request,
        CancellationToken cancellationToken = default);

    Task AddReviewAsync(
        Guid requestId,
        AddBenefitUsageRequestReviewRequest request,
        Guid? performedByUserId,
        CancellationToken cancellationToken = default);

    Task ChangeStatusAsync(
        Guid requestId,
        ChangeBenefitUsageRequestStatusRequest request,
        Guid? performedByUserId,
        CancellationToken cancellationToken = default);

    Task<BenefitUsageRequestDetailDto?> GetByIdAsync(
        Guid requestId,
        CancellationToken cancellationToken = default);

    Task<PagedResultDto<BenefitUsageRequestListItemDto>> SearchAsync(
        BenefitUsageRequestFilterDto filter,
        CancellationToken cancellationToken = default);

    Task<PagedResultDto<BenefitUsageRequestListItemDto>> SearchPendingReviewAsync(
        BenefitUsageRequestFilterDto filter,
        CancellationToken cancellationToken = default);

    Task<BenefitRequestApprovalSummaryDto> GetApprovalSummaryAsync(
        CancellationToken cancellationToken = default);

    Task<BenefitUsageConfirmationPairResultDto> CreateUsageConfirmationPairAsync(
    Guid requestId,
    string clientTokenHash,
    string partnerTokenHash,
    string clientConfirmationUrl,
    string partnerConfirmationUrl,
    DateTime confirmationExpiresAt,
    string partnerRecipientEmail,
    string? partnerRecipientName,
    Guid? createdByUserId,
    CancellationToken cancellationToken = default);

    Task<BenefitUsageConfirmationTokenDto?> GetUsageConfirmationByTokenHashAsync(
        string tokenHash,
        CancellationToken cancellationToken = default);

    Task<ConfirmBenefitUsageConfirmationResultDto> ConfirmUsageConfirmationAsync(
        string tokenHash,
        CancellationToken cancellationToken = default);
}