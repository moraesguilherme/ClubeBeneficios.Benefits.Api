using ClubeBeneficios.Benefits.Domain.Dtos;
using ClubeBeneficios.Benefits.Domain.Dtos.Filters;
using ClubeBeneficios.Benefits.Domain.Dtos.Requests;

namespace ClubeBeneficios.Benefits.Domain.Repositories;

public interface IBenefitRepository
{
    Task<PagedResultDto<BenefitListItemDto>> GetPagedAsync(BenefitFilterDto filter, CancellationToken cancellationToken = default);
    Task<PagedResultDto<BenefitApprovalQueueItemDto>> GetApprovalQueueAsync(BenefitFilterDto filter, CancellationToken cancellationToken = default);
    Task<BenefitSummaryDto> GetSummaryAsync(Guid? partnerId, CancellationToken cancellationToken = default);
    Task<BenefitFilterOptionsDto> GetFilterOptionsAsync(Guid? partnerId, CancellationToken cancellationToken = default);
    Task<BenefitDetailsDto?> GetByIdAsync(Guid id, Guid? partnerId, bool enforcePartnerOwnership, CancellationToken cancellationToken = default);
    Task<Guid> CreateAsync(CreateBenefitRequest request, Guid createdByUserId, Guid? partnerContextId, bool isPartnerScope, CancellationToken cancellationToken = default);
    Task UpdateAsync(Guid id, UpdateBenefitRequest request, Guid updatedByUserId, Guid? partnerContextId, bool isPartnerScope, CancellationToken cancellationToken = default);
    Task ChangeStatusAsync(Guid id, ChangeBenefitStatusRequest request, Guid changedByUserId, Guid? partnerContextId, bool isPartnerScope, CancellationToken cancellationToken = default);
    Task AddReviewAsync(Guid id, ReviewBenefitRequest request, Guid reviewedByUserId, CancellationToken cancellationToken = default);
}