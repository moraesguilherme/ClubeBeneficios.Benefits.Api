using ClubeBeneficios.Benefits.Domain.Dtos.Benefits;
using ClubeBeneficios.Benefits.Domain.Dtos.Requests.Filters;

namespace ClubeBeneficios.Benefits.Domain.Repositories;

public interface IBenefitAnalyticsRepository
{
    Task<BenefitOfferDashboardSummaryDto> GetDashboardSummaryAsync(BenefitDashboardSummaryFilterDto filter, CancellationToken cancellationToken = default);
    Task<IEnumerable<BenefitOfferMetricItemDto>> GetMetricsAsync(BenefitMetricsFilterDto filter, CancellationToken cancellationToken = default);
    Task<IEnumerable<BenefitOfferHistoryItemDto>> GetHistoryAsync(BenefitHistoryFilterDto filter, CancellationToken cancellationToken = default);
}