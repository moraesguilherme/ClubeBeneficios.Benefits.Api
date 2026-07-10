using ClubeBeneficios.Benefits.Domain.Dtos.Benefits;
using ClubeBeneficios.Benefits.Domain.Dtos.Requests.Filters;

namespace ClubeBeneficios.Benefits.Domain.Repositories;

public interface IBenefitAnalyticsRepository
{
    Task<BenefitDashboardSummaryDto> GetDashboardSummaryAsync(BenefitDashboardSummaryFilterDto filter, CancellationToken cancellationToken = default);
    Task<IEnumerable<BenefitMetricItemDto>> GetMetricsAsync(BenefitMetricsFilterDto filter, CancellationToken cancellationToken = default);
    Task<IEnumerable<BenefitHistoryItemDto>> GetHistoryAsync(BenefitHistoryFilterDto filter, CancellationToken cancellationToken = default);
}