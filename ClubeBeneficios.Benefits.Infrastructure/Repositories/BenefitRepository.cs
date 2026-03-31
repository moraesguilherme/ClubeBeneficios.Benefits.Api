using System.Data;
using Dapper;
using ClubeBeneficios.Benefits.Domain.Dtos;
using ClubeBeneficios.Benefits.Domain.Dtos.Filters;
using ClubeBeneficios.Benefits.Domain.Dtos.Requests;
using ClubeBeneficios.Benefits.Domain.Repositories;

namespace ClubeBeneficios.Benefits.Infrastructure.Repositories;

public class BenefitRepository : IBenefitRepository
{
    private readonly IDbConnection _connection;

    public BenefitRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<PagedResultDto<BenefitListItemDto>> GetPagedAsync(BenefitFilterDto filter, CancellationToken cancellationToken = default)
    {
        const string itemsSql = @"
select
    b.id as Id,
    b.partner_id as PartnerId,
    p.trade_name as PartnerName,
    b.title as Title,
    b.benefit_type as BenefitType,
    b.direction as Direction,
    case
        when b.direction = 'partner_to_matilha' then 'Parceiro Ã¢â€ â€™ Matilha'
        when b.direction = 'matilha_to_partner' then 'Matilha Ã¢â€ â€™ Parceiro'
        else b.direction
    end as DirectionLabel,
    b.status as Status,
    b.target_actor_type as TargetActorType,
    case
        when b.target_actor_type = 'client' then 'Cliente Matilha'
        when b.target_actor_type = 'partner_customer' then 'Cliente do parceiro'
        else b.target_actor_type
    end as TargetActorLabel,
    b.eligibility_type as EligibilityType,
    isnull(b.short_description, b.full_description) as EligibilitySummary,
    case
        when b.validity_type = 'continuous' then 'ContÃƒÂ­nuo'
        when b.validity_type = 'date_range' then 'PerÃƒÂ­odo definido'
        else b.validity_type
    end as ValidityLabel,
    concat(
        isnull(cast(b.recurrence_value as varchar(20)), '0'),
        ' / ',
        isnull(b.recurrence_period, '-')
    ) as RecurrenceLabel,
    isnull(ms.requests_count, 0) as RequestsCount,
    isnull(ms.usages_count, 0) as UsagesCount,
    cast(isnull(ms.conversion_rate, 0) as int) as ConversionRate,
    p.level as PartnerLevel,
    b.created_at as CreatedAt,
    b.updated_at as UpdatedAt
from dbo.benefits b
inner join dbo.partners p
    on p.id = b.partner_id
left join dbo.benefit_metrics_snapshot ms
    on ms.benefit_id = b.id
where (@search is null or b.title like '%' + @search + '%' or p.trade_name like '%' + @search + '%')
  and (@partner_id is null or b.partner_id = @partner_id)
  and (@origin is null or p.origin_type = @origin)
  and (@status is null or b.status = @status)
  and (@target_actor_type is null or b.target_actor_type = @target_actor_type)
  and (@eligibility_type is null or b.eligibility_type = @eligibility_type)
  and (@only_pending_approval = 0 or b.status in ('pending_review', 'under_review', 'approved'))
order by b.created_at desc
offset @offset rows fetch next @page_size rows only;
";

        const string totalSql = @"
select count(1)
from dbo.benefits b
inner join dbo.partners p
    on p.id = b.partner_id
where (@search is null or b.title like '%' + @search + '%' or p.trade_name like '%' + @search + '%')
  and (@partner_id is null or b.partner_id = @partner_id)
  and (@origin is null or p.origin_type = @origin)
  and (@status is null or b.status = @status)
  and (@target_actor_type is null or b.target_actor_type = @target_actor_type)
  and (@eligibility_type is null or b.eligibility_type = @eligibility_type)
  and (@only_pending_approval = 0 or b.status in ('pending_review', 'under_review', 'approved'));
";

        var parameters = new
        {
            search = filter.Search,
            partner_id = filter.PartnerId,
            origin = filter.Origin,
            status = filter.Status,
            target_actor_type = filter.TargetActorType,
            eligibility_type = filter.EligibilityType,
            only_pending_approval = filter.OnlyPendingApproval,
            offset = (filter.Page - 1) * filter.PageSize,
            page_size = filter.PageSize
        };

        var items = (await _connection.QueryAsync<BenefitListItemDto>(
            new CommandDefinition(itemsSql, parameters, commandType: CommandType.Text, cancellationToken: cancellationToken))).ToList();

        var total = await _connection.ExecuteScalarAsync<int>(
            new CommandDefinition(totalSql, parameters, commandType: CommandType.Text, cancellationToken: cancellationToken));

        return new PagedResultDto<BenefitListItemDto>
        {
            Items = items,
            Page = filter.Page,
            PageSize = filter.PageSize,
            TotalCount = total
        };
    }

    public async Task<PagedResultDto<BenefitApprovalQueueItemDto>> GetApprovalQueueAsync(BenefitFilterDto filter, CancellationToken cancellationToken = default)
    {
        const string itemsSql = @"
select
    b.id as Id,
    b.partner_id as PartnerId,
    p.trade_name as PartnerName,
    b.title as Title,
    b.direction as Direction,
    b.status as Status,
    case
        when b.status in ('pending_review', 'under_review') then b.status
        when b.status = 'approved' and isnull(b.auto_activate_when_approved, 0) = 0 then 'approved_waiting_activation'
        else b.status
    end as ApprovalStatus,
    isnull(b.short_description, b.full_description) as EligibilitySummary,
    concat(
        isnull(cast(b.recurrence_value as varchar(20)), '0'),
        ' / ',
        isnull(b.recurrence_period, '-')
    ) as RecurrenceLabel,
    case
        when b.validity_type = 'continuous' then 'ContÃƒÂ­nuo'
        when b.validity_type = 'date_range' then 'PerÃƒÂ­odo definido'
        else b.validity_type
    end as ValidityLabel,
    b.created_at as CreatedAt,
    b.updated_at as UpdatedAt
from dbo.benefits b
inner join dbo.partners p
    on p.id = b.partner_id
where b.status in ('pending_review', 'under_review', 'approved')
  and (@search is null or b.title like '%' + @search + '%' or p.trade_name like '%' + @search + '%')
  and (@partner_id is null or b.partner_id = @partner_id)
  and (@origin is null or p.origin_type = @origin)
  and (@status is null or b.status = @status)
order by b.created_at desc
offset @offset rows fetch next @page_size rows only;
";

        const string totalSql = @"
select count(1)
from dbo.benefits b
inner join dbo.partners p
    on p.id = b.partner_id
where b.status in ('pending_review', 'under_review', 'approved')
  and (@search is null or b.title like '%' + @search + '%' or p.trade_name like '%' + @search + '%')
  and (@partner_id is null or b.partner_id = @partner_id)
  and (@origin is null or p.origin_type = @origin)
  and (@status is null or b.status = @status);
";

        var parameters = new
        {
            search = filter.Search,
            partner_id = filter.PartnerId,
            origin = filter.Origin,
            status = filter.Status,
            offset = (filter.Page - 1) * filter.PageSize,
            page_size = filter.PageSize
        };

        var items = (await _connection.QueryAsync<BenefitApprovalQueueItemDto>(
            new CommandDefinition(itemsSql, parameters, commandType: CommandType.Text, cancellationToken: cancellationToken))).ToList();

        var total = await _connection.ExecuteScalarAsync<int>(
            new CommandDefinition(totalSql, parameters, commandType: CommandType.Text, cancellationToken: cancellationToken));

        return new PagedResultDto<BenefitApprovalQueueItemDto>
        {
            Items = items,
            Page = filter.Page,
            PageSize = filter.PageSize,
            TotalCount = total
        };
    }

    public async Task<BenefitSummaryDto> GetSummaryAsync(Guid? partnerId, CancellationToken cancellationToken = default)
    {
        const string sql = @"
select
    sum(case when b.status = 'active' then 1 else 0 end) as ActiveCount,
    sum(case when b.status = 'pending_review' then 1 else 0 end) as PendingCount,
    sum(case when b.status = 'under_review' then 1 else 0 end) as InReviewCount,
    sum(case when isnull(ms.conversion_rate, 0) < 30 then 1 else 0 end) as LowConversionCount,
    count(distinct case when p.status in ('active','approved') then p.id end) as ActivePartnersCount,
    isnull(sum(isnull(ms.requests_count, 0)), 0) as TotalRequests,
    isnull(sum(isnull(ms.usages_count, 0)), 0) as TotalUsages,
    cast(case
        when isnull(sum(isnull(ms.requests_count, 0)), 0) = 0 then 0
        else (100.0 * isnull(sum(isnull(ms.usages_count, 0)), 0) / nullif(sum(isnull(ms.requests_count, 0)), 0))
    end as int) as ConversionRate
from dbo.benefits b
inner join dbo.partners p
    on p.id = b.partner_id
left join dbo.benefit_metrics_snapshot ms
    on ms.benefit_id = b.id
where (@partner_id is null or b.partner_id = @partner_id);
";

        return await _connection.QueryFirstOrDefaultAsync<BenefitSummaryDto>(
            new CommandDefinition(sql, new { partner_id = partnerId }, commandType: CommandType.Text, cancellationToken: cancellationToken))
            ?? new BenefitSummaryDto();
    }

    public async Task<BenefitFilterOptionsDto> GetFilterOptionsAsync(Guid? partnerId, CancellationToken cancellationToken = default)
    {
        const string sql = @"
select distinct
    isnull(p.origin_type, '') as Value,
    isnull(p.origin_type, '') as Label
from dbo.benefits b
inner join dbo.partners p on p.id = b.partner_id
where (@partner_id is null or b.partner_id = @partner_id)
  and p.origin_type is not null;

select distinct
    b.status as Value,
    b.status as Label
from dbo.benefits b
where (@partner_id is null or b.partner_id = @partner_id)
  and b.status is not null;

select distinct
    b.target_actor_type as Value,
    b.target_actor_type as Label
from dbo.benefits b
where (@partner_id is null or b.partner_id = @partner_id)
  and b.target_actor_type is not null;

select distinct
    b.eligibility_type as Value,
    b.eligibility_type as Label
from dbo.benefits b
where (@partner_id is null or b.partner_id = @partner_id)
  and b.eligibility_type is not null;

select distinct
    cast(p.id as varchar(36)) as Value,
    p.trade_name as Label
from dbo.benefits b
inner join dbo.partners p on p.id = b.partner_id
where (@partner_id is null or b.partner_id = @partner_id);
";

        using var multi = await _connection.QueryMultipleAsync(
            new CommandDefinition(sql, new { partner_id = partnerId }, commandType: CommandType.Text, cancellationToken: cancellationToken));

        return new BenefitFilterOptionsDto
        {
            Origins = (await multi.ReadAsync<LookupItemDto>()).ToList(),
            Statuses = (await multi.ReadAsync<LookupItemDto>()).ToList(),
            Audiences = (await multi.ReadAsync<LookupItemDto>()).ToList(),
            EligibilityTypes = (await multi.ReadAsync<LookupItemDto>()).ToList(),
            Partners = (await multi.ReadAsync<LookupItemDto>()).ToList()
        };
    }

    public async Task<BenefitDetailsDto?> GetByIdAsync(Guid id, Guid? partnerId, bool enforcePartnerOwnership, CancellationToken cancellationToken = default)
    {
        const string sql = @"
select top 1
    b.*,
    p.trade_name as PartnerName
from dbo.benefits b
inner join dbo.partners p
    on p.id = b.partner_id
where b.id = @id
  and (@enforce_partner_ownership = 0 or b.partner_id = @partner_id);
";

        return await _connection.QueryFirstOrDefaultAsync<BenefitDetailsDto>(
            new CommandDefinition(
                sql,
                new
                {
                    id,
                    partner_id = partnerId,
                    enforce_partner_ownership = enforcePartnerOwnership ? 1 : 0
                },
                commandType: CommandType.Text,
                cancellationToken: cancellationToken));
    }

    public async Task<Guid> CreateAsync(CreateBenefitRequest request, Guid createdByUserId, Guid? partnerContextId, bool isPartnerScope, CancellationToken cancellationToken = default)
    {
        var createdId = Guid.NewGuid();
        var partnerId = isPartnerScope ? partnerContextId : request.PartnerId;

        const string sql = @"
insert into dbo.benefits
(
    id, partner_id, created_by_user_id, updated_by_user_id, approved_by_user_id, rejected_by_user_id,
    title, benefit_type, direction, target_actor_type, status,
    short_description, full_description, internal_notes,
    eligibility_type, recurrence_type, recurrence_value, recurrence_period,
    validity_type, starts_at, ends_at,
    requires_manual_release, auto_activate_when_approved, highlight_in_showcase,
    allow_first_use_only, requires_active_access_code, requires_partner_availability, requires_matilha_acceptance_rules,
    stacking_rule, approval_notes, rejection_reason,
    approved_at, rejected_at, inactivated_at, created_at, updated_at
)
values
(
    @id, @partner_id, @created_by_user_id, @updated_by_user_id, null, null,
    @title, @benefit_type, @direction, @target_actor_type, @status,
    @short_description, @full_description, @internal_notes,
    @eligibility_type, @recurrence_type, @recurrence_value, @recurrence_period,
    @validity_type, @starts_at, @ends_at,
    @requires_manual_release, @auto_activate_when_approved, @highlight_in_showcase,
    0, 0, 1, 0,
    @stacking_rule, null, null,
    null, null, null, sysutcdatetime(), sysutcdatetime()
);

insert into dbo.benefit_status_history
(
    benefit_id, from_status, to_status, reason, changed_by_user_id, changed_at
)
values
(
    @id, null, @status, 'CriaÃƒÂ§ÃƒÂ£o do benefÃƒÂ­cio via API.', @created_by_user_id, sysutcdatetime()
);
";

        await _connection.ExecuteAsync(
            new CommandDefinition(
                sql,
                new
                {
                    id = createdId,
                    partner_id = partnerId,
                    created_by_user_id = createdByUserId,
                    updated_by_user_id = createdByUserId,
                    title = request.Title,
                    benefit_type = request.BenefitType,
                    direction = request.Direction,
                    target_actor_type = request.TargetActorType,
                    status = "pending_review",
                    short_description = request.ShortDescription,
                    full_description = request.FullDescription,
                    internal_notes = request.InternalNotes,
                    eligibility_type = request.EligibilityType,
                    recurrence_type = request.RecurrenceType,
                    recurrence_value = request.RecurrenceValue,
                    recurrence_period = request.RecurrencePeriod,
                    validity_type = request.ValidityType,
                    starts_at = request.StartsAt,
                    ends_at = request.EndsAt,
                    requires_manual_release = request.RequiresManualRelease,
                    auto_activate_when_approved = request.AutoActivateWhenApproved,
                    highlight_in_showcase = request.HighlightInShowcase,
                    stacking_rule = request.StackingRule
                },
                commandType: CommandType.Text,
                cancellationToken: cancellationToken));

        return createdId;
    }

    public Task UpdateAsync(Guid id, UpdateBenefitRequest request, Guid updatedByUserId, Guid? partnerContextId, bool isPartnerScope, CancellationToken cancellationToken = default)
    {
        const string sql = @"
update b
set
    b.title = @title,
    b.benefit_type = @benefit_type,
    b.target_actor_type = @target_actor_type,
    b.short_description = @short_description,
    b.full_description = @full_description,
    b.internal_notes = @internal_notes,
    b.eligibility_type = @eligibility_type,
    b.recurrence_type = @recurrence_type,
    b.recurrence_value = @recurrence_value,
    b.recurrence_period = @recurrence_period,
    b.validity_type = @validity_type,
    b.starts_at = @starts_at,
    b.ends_at = @ends_at,
    b.requires_manual_release = @requires_manual_release,
    b.auto_activate_when_approved = @auto_activate_when_approved,
    b.highlight_in_showcase = @highlight_in_showcase,
    b.stacking_rule = @stacking_rule,
    b.updated_by_user_id = @updated_by_user_id,
    b.updated_at = sysutcdatetime()
from dbo.benefits b
where b.id = @id
  and (@enforce_partner_ownership = 0 or b.partner_id = @partner_id);
";

        return _connection.ExecuteAsync(
            new CommandDefinition(
                sql,
                new
                {
                    id,
                    partner_id = partnerContextId,
                    enforce_partner_ownership = isPartnerScope ? 1 : 0,
                    title = request.Title,
                    benefit_type = request.BenefitType,
                    target_actor_type = request.TargetActorType,
                    short_description = request.ShortDescription,
                    full_description = request.FullDescription,
                    internal_notes = request.InternalNotes,
                    eligibility_type = request.EligibilityType,
                    recurrence_type = request.RecurrenceType,
                    recurrence_value = request.RecurrenceValue,
                    recurrence_period = request.RecurrencePeriod,
                    validity_type = request.ValidityType,
                    starts_at = request.StartsAt,
                    ends_at = request.EndsAt,
                    requires_manual_release = request.RequiresManualRelease,
                    auto_activate_when_approved = request.AutoActivateWhenApproved,
                    highlight_in_showcase = request.HighlightInShowcase,
                    stacking_rule = request.StackingRule,
                    updated_by_user_id = updatedByUserId
                },
                commandType: CommandType.Text,
                cancellationToken: cancellationToken));
    }

    public Task ChangeStatusAsync(Guid id, ChangeBenefitStatusRequest request, Guid changedByUserId, Guid? partnerContextId, bool isPartnerScope, CancellationToken cancellationToken = default)
    {
        const string sql = @"
declare @previous_status varchar(50);

select @previous_status = b.status
from dbo.benefits b
where b.id = @id
  and (@enforce_partner_ownership = 0 or b.partner_id = @partner_id);

update b
set
    b.status = @status,
    b.updated_by_user_id = @changed_by_user_id,
    b.updated_at = sysutcdatetime(),
    b.approved_by_user_id = case when @status in ('approved','active') then @changed_by_user_id else b.approved_by_user_id end,
    b.rejected_by_user_id = case when @status = 'rejected' then @changed_by_user_id else b.rejected_by_user_id end,
    b.approved_at = case when @status in ('approved','active') then sysutcdatetime() else b.approved_at end,
    b.rejected_at = case when @status = 'rejected' then sysutcdatetime() else b.rejected_at end,
    b.inactivated_at = case when @status = 'inactive' then sysutcdatetime() else b.inactivated_at end,
    b.approval_notes = case when @status in ('approved','active') then @reason else b.approval_notes end,
    b.rejection_reason = case when @status = 'rejected' then @reason else b.rejection_reason end
from dbo.benefits b
where b.id = @id
  and (@enforce_partner_ownership = 0 or b.partner_id = @partner_id);

insert into dbo.benefit_status_history
(
    benefit_id, from_status, to_status, reason, changed_by_user_id, changed_at
)
values
(
    @id, @previous_status, @status, @reason, @changed_by_user_id, sysutcdatetime()
);
";

        return _connection.ExecuteAsync(
            new CommandDefinition(
                sql,
                new
                {
                    id,
                    partner_id = partnerContextId,
                    enforce_partner_ownership = isPartnerScope ? 1 : 0,
                    status = request.Status,
                    reason = request.Reason,
                    changed_by_user_id = changedByUserId
                },
                commandType: CommandType.Text,
                cancellationToken: cancellationToken));
    }

    public Task AddReviewAsync(Guid id, ReviewBenefitRequest request, Guid reviewedByUserId, CancellationToken cancellationToken = default)
    {
        const string sql = @"
insert into dbo.benefit_reviews
(
    id, benefit_id, review_status, review_point, review_recommendation,
    reviewed_by_user_id, reviewed_at, created_at
)
values
(
    newid(), @benefit_id, @review_status, @review_point, @review_recommendation,
    @reviewed_by_user_id, sysutcdatetime(), sysutcdatetime()
);
";

        return _connection.ExecuteAsync(
            new CommandDefinition(
                sql,
                new
                {
                    benefit_id = id,
                    review_status = request.ReviewStatus,
                    review_point = request.ReviewPoint,
                    review_recommendation = request.ReviewRecommendation,
                    reviewed_by_user_id = reviewedByUserId
                },
                commandType: CommandType.Text,
                cancellationToken: cancellationToken));
    }
}