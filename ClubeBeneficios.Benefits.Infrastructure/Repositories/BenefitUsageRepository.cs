using System.Data;
using Dapper;
using ClubeBeneficios.Benefits.Domain.Dtos;
using ClubeBeneficios.Benefits.Domain.Dtos.Filters;
using ClubeBeneficios.Benefits.Domain.Dtos.Requests;
using ClubeBeneficios.Benefits.Domain.Repositories;

namespace ClubeBeneficios.Benefits.Infrastructure.Repositories;

public class BenefitUsageRepository : IBenefitUsageRepository
{
    private readonly IDbConnection _connection;

    public BenefitUsageRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<Guid> ConfirmAsync(ConfirmBenefitUsageRequest request, Guid? performedByUserId, bool performedByPartner, CancellationToken cancellationToken = default)
    {
        var createdId = Guid.NewGuid();

        const string sql = @"
insert into dbo.benefit_usages
(
    id, benefit_request_id, benefit_id, partner_id,
    used_by_user_id, used_by_partner_customer_id, used_by_type,
    pet_id, usage_status, used_at,
    confirmed_by_partner_user_id, confirmed_by_admin_user_id,
    monetary_value, discount_value,
    snapshot_title, snapshot_partner_name, snapshot_rule_summary,
    created_at, updated_at
)
values
(
    @id, @benefit_request_id, @benefit_id, @partner_id,
    @used_by_user_id, @used_by_partner_customer_id, @used_by_type,
    @pet_id, 'used', @used_at,
    case when @performed_by_partner = 1 then @performed_by_user_id else null end,
    case when @performed_by_partner = 1 then null else @performed_by_user_id end,
    @monetary_value, @discount_value,
    @snapshot_title, @snapshot_partner_name, @snapshot_rule_summary,
    sysutcdatetime(), sysutcdatetime()
);
";

        await _connection.ExecuteAsync(
            new CommandDefinition(
                sql,
                new
                {
                    id = createdId,
                    benefit_request_id = request.BenefitRequestId,
                    benefit_id = request.BenefitId,
                    partner_id = request.PartnerId,
                    used_by_user_id = request.UsedByUserId,
                    used_by_partner_customer_id = request.UsedByPartnerCustomerId,
                    used_by_type = request.UsedByType,
                    pet_id = request.PetId,
                    used_at = request.UsedAt,
                    monetary_value = request.MonetaryValue,
                    discount_value = request.DiscountValue,
                    snapshot_title = request.SnapshotTitle,
                    snapshot_partner_name = request.SnapshotPartnerName,
                    snapshot_rule_summary = request.SnapshotRuleSummary,
                    performed_by_user_id = performedByUserId,
                    performed_by_partner = performedByPartner ? 1 : 0
                },
                commandType: CommandType.Text,
                cancellationToken: cancellationToken));

        return createdId;
    }

    public Task CancelAsync(Guid usageId, CancelBenefitUsageRequest request, Guid? performedByUserId, CancellationToken cancellationToken = default)
    {
        const string sql = @"
update dbo.benefit_usages
set
    usage_status = 'cancelled',
    snapshot_rule_summary = case
        when @cancellation_reason is null then snapshot_rule_summary
        else concat(isnull(snapshot_rule_summary, ''), ' | Cancelado: ', @cancellation_reason)
    end,
    updated_at = sysutcdatetime()
where id = @benefit_usage_id;
";

        return _connection.ExecuteAsync(
            new CommandDefinition(
                sql,
                new
                {
                    benefit_usage_id = usageId,
                    cancellation_reason = request.CancellationReason,
                    performed_by_user_id = performedByUserId
                },
                commandType: CommandType.Text,
                cancellationToken: cancellationToken));
    }

    public Task<BenefitUsageDetailDto?> GetByIdAsync(Guid usageId, CancellationToken cancellationToken = default)
    {
        const string sql = @"
select top 1
    bu.id as Id,
    bu.benefit_id as BenefitId,
    bu.benefit_request_id as BenefitRequestId,
    bu.partner_id as PartnerId,
    bu.used_by_user_id as UsedByUserId,
    bu.used_by_partner_customer_id as UsedByPartnerCustomerId,
    bu.used_by_type as UsedByType,
    bu.pet_id as PetId,
    bu.usage_status as UsageStatus,
    bu.used_at as UsedAt,
    bu.confirmed_by_partner_user_id as ConfirmedByPartnerUserId,
    bu.confirmed_by_admin_user_id as ConfirmedByAdminUserId,
    bu.monetary_value as MonetaryValue,
    bu.discount_value as DiscountValue,
    bu.snapshot_title as SnapshotTitle,
    bu.snapshot_partner_name as SnapshotPartnerName,
    bu.snapshot_rule_summary as SnapshotRuleSummary,
    bu.created_at as CreatedAt,
    bu.updated_at as UpdatedAt
from dbo.benefit_usages bu
where bu.id = @benefit_usage_id;
";

        return _connection.QueryFirstOrDefaultAsync<BenefitUsageDetailDto>(
            new CommandDefinition(sql, new { benefit_usage_id = usageId }, commandType: CommandType.Text, cancellationToken: cancellationToken));
    }

    public async Task<PagedResultDto<BenefitUsageListItemDto>> SearchAsync(BenefitUsageFilterDto filter, CancellationToken cancellationToken = default)
    {
        const string itemsSql = @"
select
    bu.id as Id,
    bu.benefit_id as BenefitId,
    bu.benefit_request_id as BenefitRequestId,
    bu.partner_id as PartnerId,
    bu.usage_status as UsageStatus,
    bu.used_by_type as UsedByType,
    bu.used_at as UsedAt,
    bu.monetary_value as MonetaryValue,
    bu.discount_value as DiscountValue,
    bu.snapshot_title as SnapshotTitle,
    bu.snapshot_partner_name as SnapshotPartnerName
from dbo.benefit_usages bu
where (@search is null or bu.snapshot_title like '%' + @search + '%' or bu.snapshot_partner_name like '%' + @search + '%')
  and (@benefit_id is null or bu.benefit_id = @benefit_id)
  and (@benefit_request_id is null or bu.benefit_request_id = @benefit_request_id)
  and (@partner_id is null or bu.partner_id = @partner_id)
  and (@usage_status is null or bu.usage_status = @usage_status)
  and (@used_by_type is null or bu.used_by_type = @used_by_type)
  and (@used_from is null or bu.used_at >= @used_from)
  and (@used_to is null or bu.used_at < dateadd(day, 1, @used_to))
order by bu.used_at desc
offset @offset rows fetch next @page_size rows only;
";

        const string totalSql = @"
select count(1)
from dbo.benefit_usages bu
where (@search is null or bu.snapshot_title like '%' + @search + '%' or bu.snapshot_partner_name like '%' + @search + '%')
  and (@benefit_id is null or bu.benefit_id = @benefit_id)
  and (@benefit_request_id is null or bu.benefit_request_id = @benefit_request_id)
  and (@partner_id is null or bu.partner_id = @partner_id)
  and (@usage_status is null or bu.usage_status = @usage_status)
  and (@used_by_type is null or bu.used_by_type = @used_by_type)
  and (@used_from is null or bu.used_at >= @used_from)
  and (@used_to is null or bu.used_at < dateadd(day, 1, @used_to));
";

        var parameters = new
        {
            search = filter.Search,
            benefit_id = filter.BenefitId,
            benefit_request_id = filter.BenefitRequestId,
            partner_id = filter.PartnerId,
            usage_status = filter.UsageStatus,
            used_by_type = filter.UsedByType,
            used_from = filter.UsedFrom,
            used_to = filter.UsedTo,
            offset = (filter.Page - 1) * filter.PageSize,
            page_size = filter.PageSize
        };

        var items = (await _connection.QueryAsync<BenefitUsageListItemDto>(
            new CommandDefinition(itemsSql, parameters, commandType: CommandType.Text, cancellationToken: cancellationToken))).ToList();

        var totalCount = await _connection.ExecuteScalarAsync<int>(
            new CommandDefinition(totalSql, parameters, commandType: CommandType.Text, cancellationToken: cancellationToken));

        return new PagedResultDto<BenefitUsageListItemDto>
        {
            Items = items,
            Page = filter.Page,
            PageSize = filter.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<BenefitEligibilityValidationResultDto?> ValidateAsync(ValidateBenefitUsageRequest request, CancellationToken cancellationToken = default)
    {
        const string benefitSql = @"
select
    b.id,
    b.status,
    b.recurrence_value,
    b.recurrence_period,
    b.recurrence_type,
    b.starts_at,
    b.ends_at
from dbo.benefits b
where b.id = @benefit_id
  and (@partner_id is null or b.partner_id = @partner_id);
";

        var benefit = await _connection.QueryFirstOrDefaultAsync<dynamic>(
            new CommandDefinition(
                benefitSql,
                new
                {
                    benefit_id = request.BenefitId,
                    partner_id = request.PartnerId
                },
                commandType: CommandType.Text,
                cancellationToken: cancellationToken));

        if (benefit is null)
        {
            return new BenefitEligibilityValidationResultDto
            {
                IsAllowed = false,
                BlockReason = "BenefÃ­cio nÃ£o encontrado.",
                RuleSummary = "Sem benefÃ­cio correspondente."
            };
        }

        string status = benefit.status;
        if (status != "active" && status != "approved")
        {
            return new BenefitEligibilityValidationResultDto
            {
                IsAllowed = false,
                BlockReason = "BenefÃ­cio nÃ£o estÃ¡ disponÃ­vel para uso.",
                RuleSummary = "Status do benefÃ­cio nÃ£o permite utilizaÃ§Ã£o."
            };
        }

        if (benefit.starts_at is not null && request.ValidationReferenceDate < benefit.starts_at)
        {
            return new BenefitEligibilityValidationResultDto
            {
                IsAllowed = false,
                BlockReason = "BenefÃ­cio ainda nÃ£o iniciou.",
                RuleSummary = "Uso antes da data inicial."
            };
        }

        if (benefit.ends_at is not null && request.ValidationReferenceDate > benefit.ends_at)
        {
            return new BenefitEligibilityValidationResultDto
            {
                IsAllowed = false,
                BlockReason = "BenefÃ­cio expirado.",
                RuleSummary = "Uso apÃ³s a data final."
            };
        }

        const string lockSql = @"
select top 1
    allowed_uses as AllowedUses,
    used_count as UsedCount,
    next_available_at as NextAvailableAt
from dbo.benefit_usage_locks
where benefit_id = @benefit_id
  and actor_type = @actor_type
  and ((@user_id is not null and user_id = @user_id) or (@partner_customer_id is not null and partner_customer_id = @partner_customer_id))
order by updated_at desc, created_at desc;
";

        var usageLock = await _connection.QueryFirstOrDefaultAsync<BenefitEligibilityValidationResultDto>(
            new CommandDefinition(
                lockSql,
                new
                {
                    benefit_id = request.BenefitId,
                    actor_type = request.ActorType,
                    user_id = request.UserId,
                    partner_customer_id = request.PartnerCustomerId
                },
                commandType: CommandType.Text,
                cancellationToken: cancellationToken));

        if (usageLock is not null && usageLock.AllowedUses.HasValue && usageLock.UsedCount.HasValue && usageLock.UsedCount.Value >= usageLock.AllowedUses.Value)
        {
            usageLock.IsAllowed = false;
            usageLock.BlockReason = "Limite de utilizaÃ§Ã£o atingido.";
            usageLock.RuleSummary = "Controle por janela de recorrÃªncia.";
            return usageLock;
        }

        return new BenefitEligibilityValidationResultDto
        {
            IsAllowed = true,
            BlockReason = null,
            NextAvailableAt = usageLock?.NextAvailableAt,
            AllowedUses = usageLock?.AllowedUses,
            UsedCount = usageLock?.UsedCount,
            RuleSummary = "ElegÃ­vel para uso com base no banco atual."
        };
    }
}