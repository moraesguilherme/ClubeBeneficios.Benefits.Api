using ClubeBeneficios.Benefits.Domain.Dtos;
using ClubeBeneficios.Benefits.Domain.Dtos.Requests;

namespace ClubeBeneficios.Benefits.Domain.Services;

public interface IBenefitRequestService
{
    Task<Guid> CreateAsync(CreateBenefitRequestRequest request, CancellationToken cancellationToken = default);
    Task ChangeStatusAsync(Guid requestId, ChangeBenefitRequestStatusRequest request, CancellationToken cancellationToken = default);
    Task<BenefitRequestDetailDto?> GetByIdAsync(Guid requestId, CancellationToken cancellationToken = default);
    Task<PagedResultDto<BenefitRequestListItemDto>> SearchAdminAsync(BenefitRequestFilterDto filter, CancellationToken cancellationToken = default);
    Task<PagedResultDto<BenefitRequestListItemDto>> SearchPartnerAsync(BenefitRequestFilterDto filter, CancellationToken cancellationToken = default);
}