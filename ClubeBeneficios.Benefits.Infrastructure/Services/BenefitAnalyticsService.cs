using ClubeBeneficios.Benefits.Domain.Exceptions;
using ClubeBeneficios.Benefits.Domain.Repositories;
using ClubeBeneficios.Benefits.Domain.Security;
using ClubeBeneficios.Benefits.Domain.Services;
using ClubeBeneficios.Benefits.Domain.Dtos.Benefits;
using ClubeBeneficios.Benefits.Domain.Dtos.Requests.Filters;

namespace ClubeBeneficios.Benefits.Infrastructure.Services;

public class BenefitAnalyticsService : IBenefitAnalyticsService
{
    private readonly IBenefitAnalyticsRepository _repository;
    private readonly ICurrentUser _currentUser;

    public BenefitAnalyticsService(
        IBenefitAnalyticsRepository repository,
        ICurrentUser currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public Task<BenefitOfferDashboardSummaryDto> GetAdminDashboardSummaryAsync(BenefitDashboardSummaryFilterDto filter, CancellationToken cancellationToken = default)
        => _repository.GetDashboardSummaryAsync(filter, cancellationToken);

    public Task<IEnumerable<BenefitOfferMetricItemDto>> GetAdminMetricsAsync(BenefitMetricsFilterDto filter, CancellationToken cancellationToken = default)
        => _repository.GetMetricsAsync(filter, cancellationToken);

    public Task<IEnumerable<BenefitOfferHistoryItemDto>> GetAdminHistoryAsync(BenefitHistoryFilterDto filter, CancellationToken cancellationToken = default)
        => _repository.GetHistoryAsync(filter, cancellationToken);

    public Task<BenefitOfferDashboardSummaryDto> GetPartnerDashboardSummaryAsync(BenefitDashboardSummaryFilterDto filter, CancellationToken cancellationToken = default)
    {
        EnsurePartnerContext();
        filter.PartnerId = _currentUser.PartnerId;
        return _repository.GetDashboardSummaryAsync(filter, cancellationToken);
    }

    public Task<IEnumerable<BenefitOfferMetricItemDto>> GetPartnerMetricsAsync(BenefitMetricsFilterDto filter, CancellationToken cancellationToken = default)
    {
        EnsurePartnerContext();
        filter.PartnerId = _currentUser.PartnerId;
        return _repository.GetMetricsAsync(filter, cancellationToken);
    }

    public Task<IEnumerable<BenefitOfferHistoryItemDto>> GetPartnerHistoryAsync(BenefitHistoryFilterDto filter, CancellationToken cancellationToken = default)
    {
        EnsurePartnerContext();
        filter.PartnerId = _currentUser.PartnerId;
        return _repository.GetHistoryAsync(filter, cancellationToken);
    }

    private void EnsurePartnerContext()
    {
        if (!_currentUser.IsAuthenticated || !_currentUser.PartnerId.HasValue)
        {
            throw new InvalidOperationException("NÃƒÆ’Ã‚Â£o foi possÃƒÆ’Ã‚Â­vel identificar o parceiro autenticado.");
        }
    }
}