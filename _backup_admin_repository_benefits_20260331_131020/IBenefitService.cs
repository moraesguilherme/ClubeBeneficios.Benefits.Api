using ClubeBeneficios.Benefits.Domain.Dtos;
using ClubeBeneficios.Benefits.Domain.Dtos.Filters;
using ClubeBeneficios.Benefits.Domain.Dtos.Requests;

namespace ClubeBeneficios.Benefits.Domain.Services;

public interface IBenefitService
{
    Task<PagedResultDto<BenefitListItemDto>> GetAdminPagedAsync(BenefitFilterDto filter, CancellationToken cancellationToken = default);
    Task<PagedResultDto<BenefitListItemDto>> GetPartnerPagedAsync(BenefitFilterDto filter, CancellationToken cancellationToken = default);
    Task<PagedResultDto<BenefitApprovalQueueItemDto>> GetApprovalQueueAsync(BenefitFilterDto filter, CancellationToken cancellationToken = default);
    Task<BenefitSummaryDto> GetAdminSummaryAsync(CancellationToken cancellationToken = default);
    Task<BenefitSummaryDto> GetPartnerSummaryAsync(CancellationToken cancellationToken = default);
    Task<BenefitFilterOptionsDto> GetAdminFilterOptionsAsync(CancellationToken cancellationToken = default);
    Task<BenefitFilterOptionsDto> GetPartnerFilterOptionsAsync(CancellationToken cancellationToken = default);
    Task<BenefitDetailsDto?> GetAdminByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<BenefitDetailsDto?> GetPartnerByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Guid> CreateAdminAsync(CreateBenefitRequest request, CancellationToken cancellationToken = default);
    Task<Guid> CreatePartnerAsync(CreateBenefitRequest request, CancellationToken cancellationToken = default);
    Task UpdateAdminAsync(Guid id, UpdateBenefitRequest request, CancellationToken cancellationToken = default);
    Task UpdatePartnerAsync(Guid id, UpdateBenefitRequest request, CancellationToken cancellationToken = default);
    Task ChangeAdminStatusAsync(Guid id, ChangeBenefitStatusRequest request, CancellationToken cancellationToken = default);
    Task ChangePartnerStatusAsync(Guid id, ChangeBenefitStatusRequest request, CancellationToken cancellationToken = default);
    Task AddAdminReviewAsync(Guid id, ReviewBenefitRequest request, CancellationToken cancellationToken = default);
}