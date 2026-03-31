using ClubeBeneficios.Benefits.Domain.Dtos;
using ClubeBeneficios.Benefits.Domain.Dtos.Requests;
using ClubeBeneficios.Benefits.Domain.Repositories;
using ClubeBeneficios.Benefits.Domain.Security;
using ClubeBeneficios.Benefits.Domain.Services;

namespace ClubeBeneficios.Benefits.Infrastructure.Services;

public class BenefitUsageService : IBenefitUsageService
{
    private readonly IBenefitUsageRepository _repository;
    private readonly ICurrentUser _currentUser;

    public BenefitUsageService(IBenefitUsageRepository repository, ICurrentUser currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public Task<Guid> ConfirmAdminAsync(ConfirmBenefitUsageRequest request, CancellationToken cancellationToken = default)
        => _repository.ConfirmAsync(request, _currentUser.UserId, performedByPartner: false, cancellationToken);

    public Task<Guid> ConfirmPartnerAsync(ConfirmBenefitUsageRequest request, CancellationToken cancellationToken = default)
    {
        request.PartnerId = _currentUser.PartnerId;
        return _repository.ConfirmAsync(request, _currentUser.UserId, performedByPartner: true, cancellationToken);
    }

    public Task CancelAsync(Guid usageId, CancelBenefitUsageRequest request, CancellationToken cancellationToken = default)
        => _repository.CancelAsync(usageId, request, _currentUser.UserId, cancellationToken);

    public Task<BenefitUsageDetailDto?> GetByIdAsync(Guid usageId, CancellationToken cancellationToken = default)
        => _repository.GetByIdAsync(usageId, cancellationToken);

    public Task<PagedResultDto<BenefitUsageListItemDto>> SearchAdminAsync(BenefitUsageFilterDto filter, CancellationToken cancellationToken = default)
        => _repository.SearchAsync(filter, cancellationToken);

    public Task<PagedResultDto<BenefitUsageListItemDto>> SearchPartnerAsync(BenefitUsageFilterDto filter, CancellationToken cancellationToken = default)
    {
        filter.PartnerId = _currentUser.PartnerId;
        return _repository.SearchAsync(filter, cancellationToken);
    }

    public Task<BenefitEligibilityValidationResultDto?> ValidateAsync(ValidateBenefitUsageRequest request, CancellationToken cancellationToken = default)
    {
        if (string.Equals(request.ActorType, "partner_customer", StringComparison.OrdinalIgnoreCase))
        {
            request.PartnerId = _currentUser.PartnerId ?? request.PartnerId;
        }

        return _repository.ValidateAsync(request, cancellationToken);
    }
}