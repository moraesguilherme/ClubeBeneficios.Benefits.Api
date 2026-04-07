using ClubeBeneficios.Benefits.Domain.Dtos;
using ClubeBeneficios.Benefits.Domain.Exceptions;
using ClubeBeneficios.Benefits.Domain.Repositories;
using ClubeBeneficios.Benefits.Domain.Security;
using ClubeBeneficios.Benefits.Domain.Services;

namespace ClubeBeneficios.Benefits.Infrastructure.Services;

public class BenefitLookupService : IBenefitLookupService
{
    private readonly IBenefitLookupRepository _repository;
    private readonly ICurrentUser _currentUser;

    public BenefitLookupService(
        IBenefitLookupRepository repository,
        ICurrentUser currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public Task<BenefitLookupOptionsDto> GetAdminOptionsAsync(
        Guid? partnerId = null,
        CancellationToken cancellationToken = default)
        => _repository.GetOptionsAsync(partnerId, cancellationToken);

    public Task<BenefitLookupOptionsDto> GetPartnerOptionsAsync(
        CancellationToken cancellationToken = default)
    {
        if (!_currentUser.IsAuthenticated || !_currentUser.PartnerId.HasValue)
        {
            throw new ForbiddenException("Não foi possível identificar o parceiro autenticado.");
        }

        return _repository.GetOptionsAsync(_currentUser.PartnerId, cancellationToken);
    }
}