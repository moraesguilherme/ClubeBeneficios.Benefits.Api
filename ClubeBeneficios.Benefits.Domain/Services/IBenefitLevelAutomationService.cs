using ClubeBeneficios.Benefits.Domain.Dtos;
using ClubeBeneficios.Benefits.Domain.Dtos.Requests;

namespace ClubeBeneficios.Benefits.Domain.Services;

public interface IBenefitLevelAutomationService
{
    Task<IEnumerable<PartnerLevelAutomationResultDto>> RecalculatePartnerLevelsAsync(RecalculatePartnerLevelsRequest request, CancellationToken cancellationToken = default);
    Task<IEnumerable<ClientLevelAutomationResultDto>> RecalculateClientLevelsAsync(RecalculateClientLevelsRequest request, CancellationToken cancellationToken = default);
}