using ClubeBeneficios.Benefits.Domain.Dtos.Automation;
using ClubeBeneficios.Benefits.Domain.Dtos.Requests.Automation;

namespace ClubeBeneficios.Benefits.Domain.Repositories;

public interface IBenefitLevelAutomationRepository
{
    Task<IEnumerable<PartnerLevelAutomationResultDto>> RecalculatePartnerLevelsAsync(RecalculatePartnerLevelsRequest request, Guid? changedByUserId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ClientLevelAutomationResultDto>> RecalculateClientLevelsAsync(RecalculateClientLevelsRequest request, CancellationToken cancellationToken = default);
}