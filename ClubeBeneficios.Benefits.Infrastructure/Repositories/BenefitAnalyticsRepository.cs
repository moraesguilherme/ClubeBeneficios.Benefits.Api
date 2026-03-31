using System.Data;
using Dapper;
using ClubeBeneficios.Benefits.Domain.Dtos;
using ClubeBeneficios.Benefits.Domain.Dtos.Requests;
using ClubeBeneficios.Benefits.Domain.Repositories;

namespace ClubeBeneficios.Benefits.Infrastructure.Repositories;

public class BenefitAnalyticsRepository : IBenefitAnalyticsRepository
{
    private readonly IDbConnection _connection;

    public BenefitAnalyticsRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<BenefitDashboardSummaryDto> GetDashboardSummaryAsync(BenefitDashboardSummaryFilterDto filter, CancellationToken cancellationToken = default)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@partner_id", filter.PartnerId);
        parameters.Add("@start_date", filter.StartDate);
        parameters.Add("@end_date", filter.EndDate);

        var command = new CommandDefinition(
            "dbo.usp_benefits_admin_summary",
            parameters,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        return await _connection.QueryFirstOrDefaultAsync<BenefitDashboardSummaryDto>(command)
            ?? new BenefitDashboardSummaryDto();
    }

    public async Task<IEnumerable<BenefitMetricItemDto>> GetMetricsAsync(BenefitMetricsFilterDto filter, CancellationToken cancellationToken = default)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@benefit_id", filter.BenefitId);
        parameters.Add("@partner_id", filter.PartnerId);
        parameters.Add("@start_date", filter.StartDate);
        parameters.Add("@end_date", filter.EndDate);

        var command = new CommandDefinition(
            "dbo.usp_benefit_metrics_get",
            parameters,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        return await _connection.QueryAsync<BenefitMetricItemDto>(command);
    }

    public async Task<IEnumerable<BenefitHistoryItemDto>> GetHistoryAsync(BenefitHistoryFilterDto filter, CancellationToken cancellationToken = default)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@benefit_id", filter.BenefitId);
        parameters.Add("@partner_id", filter.PartnerId);

        var command = new CommandDefinition(
            "dbo.usp_benefits_history_get",
            parameters,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        return await _connection.QueryAsync<BenefitHistoryItemDto>(command);
    }
}