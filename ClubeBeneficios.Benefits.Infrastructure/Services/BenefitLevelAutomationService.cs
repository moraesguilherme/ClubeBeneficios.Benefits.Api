using ClubeBeneficios.Benefits.Domain.Dtos;
using ClubeBeneficios.Benefits.Domain.Dtos.Requests;
using ClubeBeneficios.Benefits.Domain.Repositories;
using ClubeBeneficios.Benefits.Domain.Services;

namespace ClubeBeneficios.Benefits.Infrastructure.Services;

public class BenefitLevelAutomationService : IBenefitLevelAutomationService
{
    private readonly IBenefitLevelAutomationRepository _repository;

    public BenefitLevelAutomationService(IBenefitLevelAutomationRepository repository)
    {
        _repository = repository;
    }

    public Task<IEnumerable<PartnerLevelAutomationResultDto>> RecalculatePartnerLevelsAsync(
        RecalculatePartnerLevelsRequest request,
        Guid? changedByUserId,
        CancellationToken cancellationToken = default)
        => _repository.RecalculatePartnerLevelsAsync(request, changedByUserId, cancellationToken);

    public Task<IEnumerable<ClientLevelAutomationResultDto>> RecalculateClientLevelsAsync(RecalculateClientLevelsRequest request, CancellationToken cancellationToken = default)
        => _repository.RecalculateClientLevelsAsync(request, cancellationToken);
}