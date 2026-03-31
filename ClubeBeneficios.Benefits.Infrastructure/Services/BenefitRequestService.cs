using ClubeBeneficios.Benefits.Domain.Dtos;
using ClubeBeneficios.Benefits.Domain.Dtos.Requests;
using ClubeBeneficios.Benefits.Domain.Repositories;
using ClubeBeneficios.Benefits.Domain.Security;
using ClubeBeneficios.Benefits.Domain.Services;

namespace ClubeBeneficios.Benefits.Infrastructure.Services;

public class BenefitRequestService : IBenefitRequestService
{
    private readonly IBenefitRequestRepository _repository;
    private readonly ICurrentUser _currentUser;

    public BenefitRequestService(IBenefitRequestRepository repository, ICurrentUser currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public Task<Guid> CreateAsync(CreateBenefitRequestRequest request, CancellationToken cancellationToken = default)
        => _repository.CreateAsync(request, _currentUser.UserId, cancellationToken);

    public Task ChangeStatusAsync(Guid requestId, ChangeBenefitRequestStatusRequest request, CancellationToken cancellationToken = default)
        => _repository.ChangeStatusAsync(requestId, request, _currentUser.UserId, cancellationToken);

    public Task<BenefitRequestDetailDto?> GetByIdAsync(Guid requestId, CancellationToken cancellationToken = default)
        => _repository.GetByIdAsync(requestId, cancellationToken);

    public Task<PagedResultDto<BenefitRequestListItemDto>> SearchAdminAsync(BenefitRequestFilterDto filter, CancellationToken cancellationToken = default)
        => _repository.SearchAsync(filter, cancellationToken);

    public Task<PagedResultDto<BenefitRequestListItemDto>> SearchPartnerAsync(BenefitRequestFilterDto filter, CancellationToken cancellationToken = default)
    {
        filter.PartnerId = _currentUser.PartnerId;
        return _repository.SearchAsync(filter, cancellationToken);
    }
}