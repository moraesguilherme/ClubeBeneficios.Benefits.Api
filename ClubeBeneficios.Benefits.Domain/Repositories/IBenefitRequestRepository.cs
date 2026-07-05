using ClubeBeneficios.Benefits.Domain.Dtos;
using ClubeBeneficios.Benefits.Domain.Dtos.Requests;

namespace ClubeBeneficios.Benefits.Domain.Repositories;

public interface IBenefitRequestRepository
{
    Task<Guid> CreateAsync(
        CreateBenefitRequestDto request,
        Guid? performedByUserId,
        CancellationToken cancellationToken = default);

    Task SubmitHealthAsync(
        Guid requestId,
        SubmitBenefitRequestHealthRequest request,
        CancellationToken cancellationToken = default);

    Task AddReviewAsync(
        Guid requestId,
        AddBenefitRequestReviewRequest request,
        Guid? performedByUserId,
        CancellationToken cancellationToken = default);

    Task ChangeStatusAsync(
        Guid requestId,
        ChangeBenefitRequestStatusRequest request,
        Guid? performedByUserId,
        CancellationToken cancellationToken = default);

    Task<BenefitRequestDetailDto?> GetByIdAsync(
        Guid requestId,
        CancellationToken cancellationToken = default);

    Task<PagedResultDto<BenefitRequestListItemDto>> SearchAsync(
        BenefitRequestFilterDto filter,
        CancellationToken cancellationToken = default);

    Task<PagedResultDto<BenefitRequestListItemDto>> SearchPendingReviewAsync(
        BenefitRequestFilterDto filter,
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

    Task<BenefitUsageConfirmationConfirmResultDto> ConfirmUsageConfirmationAsync(
        string tokenHash,
        CancellationToken cancellationToken = default);
}