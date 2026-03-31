using ClubeBeneficios.Benefits.Domain.Exceptions;
using ClubeBeneficios.Benefits.Domain.Dtos;
using ClubeBeneficios.Benefits.Domain.Dtos.Requests;
using ClubeBeneficios.Benefits.Domain.Repositories;
using ClubeBeneficios.Benefits.Domain.Security;
using ClubeBeneficios.Benefits.Domain.Services;

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

    public Task<BenefitDashboardSummaryDto> GetAdminDashboardSummaryAsync(BenefitDashboardSummaryFilterDto filter, CancellationToken cancellationToken = default)
        => _repository.GetDashboardSummaryAsync(filter, cancellationToken);

    public Task<IEnumerable<BenefitMetricItemDto>> GetAdminMetricsAsync(BenefitMetricsFilterDto filter, CancellationToken cancellationToken = default)
        => _repository.GetMetricsAsync(filter, cancellationToken);

    public Task<IEnumerable<BenefitHistoryItemDto>> GetAdminHistoryAsync(BenefitHistoryFilterDto filter, CancellationToken cancellationToken = default)
        => _repository.GetHistoryAsync(filter, cancellationToken);

    public Task<BenefitDashboardSummaryDto> GetPartnerDashboardSummaryAsync(BenefitDashboardSummaryFilterDto filter, CancellationToken cancellationToken = default)
    {
        EnsurePartnerContext();
        filter.PartnerId = _currentUser.PartnerId;
        return _repository.GetDashboardSummaryAsync(filter, cancellationToken);
    }

    public Task<IEnumerable<BenefitMetricItemDto>> GetPartnerMetricsAsync(BenefitMetricsFilterDto filter, CancellationToken cancellationToken = default)
    {
        EnsurePartnerContext();
        filter.PartnerId = _currentUser.PartnerId;
        return _repository.GetMetricsAsync(filter, cancellationToken);
    }

    public Task<IEnumerable<BenefitHistoryItemDto>> GetPartnerHistoryAsync(BenefitHistoryFilterDto filter, CancellationToken cancellationToken = default)
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