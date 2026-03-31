using ClubeBeneficios.Benefits.Domain.Dtos;
using ClubeBeneficios.Benefits.Domain.Dtos.Requests;

namespace ClubeBeneficios.Benefits.Domain.Repositories;

public interface IBenefitRequestRepository
{
    Task<Guid> CreateAsync(CreateBenefitRequestRequest request, Guid? performedByUserId, CancellationToken cancellationToken = default);
    Task ChangeStatusAsync(Guid requestId, ChangeBenefitRequestStatusRequest request, Guid? performedByUserId, CancellationToken cancellationToken = default);
    Task<BenefitRequestDetailDto?> GetByIdAsync(Guid requestId, CancellationToken cancellationToken = default);
    Task<PagedResultDto<BenefitRequestListItemDto>> SearchAsync(BenefitRequestFilterDto filter, CancellationToken cancellationToken = default);
}