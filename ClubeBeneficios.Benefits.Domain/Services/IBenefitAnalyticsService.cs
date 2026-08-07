using ClubeBeneficios.Benefits.Domain.Dtos.Benefits;
using ClubeBeneficios.Benefits.Domain.Dtos.Requests.Filters;

namespace ClubeBeneficios.Benefits.Domain.Services;

public interface IBenefitAnalyticsService
{
    Task<BenefitOfferDashboardSummaryDto> GetAdminDashboardSummaryAsync(BenefitDashboardSummaryFilterDto filter, CancellationToken cancellationToken = default);
    Task<BenefitOfferDashboardSummaryDto> GetPartnerDashboardSummaryAsync(BenefitDashboardSummaryFilterDto filter, CancellationToken cancellationToken = default);
    Task<IEnumerable<BenefitOfferMetricItemDto>> GetAdminMetricsAsync(BenefitMetricsFilterDto filter, CancellationToken cancellationToken = default);
    Task<IEnumerable<BenefitOfferMetricItemDto>> GetPartnerMetricsAsync(BenefitMetricsFilterDto filter, CancellationToken cancellationToken = default);
    Task<IEnumerable<BenefitOfferHistoryItemDto>> GetAdminHistoryAsync(BenefitHistoryFilterDto filter, CancellationToken cancellationToken = default);
    Task<IEnumerable<BenefitOfferHistoryItemDto>> GetPartnerHistoryAsync(BenefitHistoryFilterDto filter, CancellationToken cancellationToken = default);
}