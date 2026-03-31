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
        const string sql = @"
select
    count(1) as TotalBenefits,
    sum(case when b.status = 'active' then 1 else 0 end) as ActiveBenefits,
    sum(case when b.status in ('pending_review', 'under_review', 'approved') then 1 else 0 end) as PendingBenefits,
    sum(case when b.status = 'rejected' then 1 else 0 end) as RejectedBenefits,
    (
        select count(1)
        from dbo.benefit_requests br
        where (@partner_id is null or br.partner_id = @partner_id)
          and (@start_date is null or br.requested_at >= @start_date)
          and (@end_date is null or br.requested_at < dateadd(day, 1, @end_date))
    ) as TotalRequests,
    (
        select count(1)
        from dbo.benefit_requests br
        where (@partner_id is null or br.partner_id = @partner_id)
          and br.request_status = 'approved'
          and (@start_date is null or br.requested_at >= @start_date)
          and (@end_date is null or br.requested_at < dateadd(day, 1, @end_date))
    ) as ApprovedRequests,
    (
        select count(1)
        from dbo.benefit_usages bu
        where (@partner_id is null or bu.partner_id = @partner_id)
          and (@start_date is null or bu.used_at >= @start_date)
          and (@end_date is null or bu.used_at < dateadd(day, 1, @end_date))
    ) as TotalUsages,
    (
        select count(1)
        from dbo.benefit_usages bu
        where (@partner_id is null or bu.partner_id = @partner_id)
          and bu.usage_status = 'used'
          and (@start_date is null or bu.used_at >= @start_date)
          and (@end_date is null or bu.used_at < dateadd(day, 1, @end_date))
    ) as ConfirmedUsages,
    (
        select isnull(sum(isnull(bu.discount_value, 0)), 0)
        from dbo.benefit_usages bu
        where (@partner_id is null or bu.partner_id = @partner_id)
          and (@start_date is null or bu.used_at >= @start_date)
          and (@end_date is null or bu.used_at < dateadd(day, 1, @end_date))
    ) as TotalDiscountValue,
    (
        select isnull(sum(isnull(bu.monetary_value, 0)), 0)
        from dbo.benefit_usages bu
        where (@partner_id is null or bu.partner_id = @partner_id)
          and (@start_date is null or bu.used_at >= @start_date)
          and (@end_date is null or bu.used_at < dateadd(day, 1, @end_date))
    ) as TotalRevenueImpacted
from dbo.benefits b
where (@partner_id is null or b.partner_id = @partner_id)
  and (@start_date is null or b.created_at >= @start_date)
  and (@end_date is null or b.created_at < dateadd(day, 1, @end_date));
";

        var command = new CommandDefinition(
            sql,
            new
            {
                partner_id = filter.PartnerId,
                start_date = filter.StartDate,
                end_date = filter.EndDate
            },
            commandType: CommandType.Text,
            cancellationToken: cancellationToken);

        return await _connection.QueryFirstOrDefaultAsync<BenefitDashboardSummaryDto>(command)
            ?? new BenefitDashboardSummaryDto();
    }

    public async Task<IEnumerable<BenefitMetricItemDto>> GetMetricsAsync(BenefitMetricsFilterDto filter, CancellationToken cancellationToken = default)
    {
        const string sql = @"
select
    bms.benefit_id as BenefitId,
    'requests_count' as MetricCode,
    'Total de solicitaÃ§Ãµes' as MetricName,
    cast(isnull(bms.requests_count, 0) as decimal(18,2)) as MetricValue,
    cast(bms.refreshed_at as date) as ReferenceDate,
    bms.refreshed_at as CreatedAt
from dbo.benefit_metrics_snapshot bms
where bms.benefit_id = @benefit_id
union all
select
    bms.benefit_id,
    'approved_requests_count',
    'SolicitaÃ§Ãµes aprovadas',
    cast(isnull(bms.approved_requests_count, 0) as decimal(18,2)),
    cast(bms.refreshed_at as date),
    bms.refreshed_at
from dbo.benefit_metrics_snapshot bms
where bms.benefit_id = @benefit_id
union all
select
    bms.benefit_id,
    'usages_count',
    'UtilizaÃ§Ãµes',
    cast(isnull(bms.usages_count, 0) as decimal(18,2)),
    cast(bms.refreshed_at as date),
    bms.refreshed_at
from dbo.benefit_metrics_snapshot bms
where bms.benefit_id = @benefit_id
union all
select
    bms.benefit_id,
    'conversion_rate',
    'Taxa de conversÃ£o',
    cast(isnull(bms.conversion_rate, 0) as decimal(18,2)),
    cast(bms.refreshed_at as date),
    bms.refreshed_at
from dbo.benefit_metrics_snapshot bms
where bms.benefit_id = @benefit_id;
";

        var command = new CommandDefinition(
            sql,
            new { benefit_id = filter.BenefitId },
            commandType: CommandType.Text,
            cancellationToken: cancellationToken);

        return await _connection.QueryAsync<BenefitMetricItemDto>(command);
    }

    public async Task<IEnumerable<BenefitHistoryItemDto>> GetHistoryAsync(BenefitHistoryFilterDto filter, CancellationToken cancellationToken = default)
    {
        const string sql = @"
select
    bsh.id as Id,
    bsh.benefit_id as BenefitId,
    'status_change' as EventType,
    bsh.from_status as FromStatus,
    bsh.to_status as ToStatus,
    bsh.reason as Notes,
    bsh.changed_by_user_id as ChangedByUserId,
    u.name as ChangedByName,
    bsh.changed_at as CreatedAt
from dbo.benefit_status_history bsh
left join dbo.users u
    on u.id = bsh.changed_by_user_id
inner join dbo.benefits b
    on b.id = bsh.benefit_id
where bsh.benefit_id = @benefit_id
  and (@partner_id is null or b.partner_id = @partner_id)
order by bsh.changed_at desc, bsh.id desc;
";

        var command = new CommandDefinition(
            sql,
            new
            {
                benefit_id = filter.BenefitId,
                partner_id = filter.PartnerId
            },
            commandType: CommandType.Text,
            cancellationToken: cancellationToken);

        return await _connection.QueryAsync<BenefitHistoryItemDto>(command);
    }
}