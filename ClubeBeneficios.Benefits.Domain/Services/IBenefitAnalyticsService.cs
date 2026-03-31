using ClubeBeneficios.Benefits.Domain.Dtos;
using ClubeBeneficios.Benefits.Domain.Dtos.Requests;

namespace ClubeBeneficios.Benefits.Domain.Services;

public interface IBenefitAnalyticsService
{
    Task<BenefitDashboardSummaryDto> GetAdminDashboardSummaryAsync(BenefitDashboardSummaryFilterDto filter, CancellationToken cancellationToken = default);
    Task<BenefitDashboardSummaryDto> GetPartnerDashboardSummaryAsync(BenefitDashboardSummaryFilterDto filter, CancellationToken cancellationToken = default);
    Task<IEnumerable<BenefitMetricItemDto>> GetAdminMetricsAsync(BenefitMetricsFilterDto filter, CancellationToken cancellationToken = default);
    Task<IEnumerable<BenefitMetricItemDto>> GetPartnerMetricsAsync(BenefitMetricsFilterDto filter, CancellationToken cancellationToken = default);
    Task<IEnumerable<BenefitHistoryItemDto>> GetAdminHistoryAsync(BenefitHistoryFilterDto filter, CancellationToken cancellationToken = default);
    Task<IEnumerable<BenefitHistoryItemDto>> GetPartnerHistoryAsync(BenefitHistoryFilterDto filter, CancellationToken cancellationToken = default);
}