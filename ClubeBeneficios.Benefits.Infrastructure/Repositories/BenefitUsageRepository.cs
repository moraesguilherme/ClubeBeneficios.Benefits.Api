using System.Data;
using Dapper;
using ClubeBeneficios.Benefits.Domain.Repositories;
using ClubeBeneficios.Benefits.Domain.Dtos.Common;
using ClubeBeneficios.Benefits.Domain.Dtos.Benefits;
using ClubeBeneficios.Benefits.Domain.Dtos.BenefitUsages;
using ClubeBeneficios.Benefits.Domain.Dtos.Requests.BenefitUsages;

namespace ClubeBeneficios.Benefits.Infrastructure.Repositories;

public class BenefitUsageRepository : IBenefitUsageRepository
{
    private readonly IDbConnection _connection;

    public BenefitUsageRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<Guid> ConfirmAsync(
        ConfirmBenefitUsageRequest request,
        Guid? performedByUserId,
        bool performedByPartner,
        CancellationToken cancellationToken = default)
    {
        if (performedByPartner)
        {
            if (request.PartnerId is null)
            {
                throw new InvalidOperationException("Não foi possível identificar o parceiro responsável pela confirmação.");
            }

            const string partnerValidationSql = @"
                select count(1)
                from dbo.benefits
                where id = @benefit_id
                  and partner_id = @partner_id;
            ";

            var belongsToPartner = await _connection.ExecuteScalarAsync<int>(
                new CommandDefinition(
                    partnerValidationSql,
                    new
                    {
                        benefit_id = request.BenefitId,
                        partner_id = request.PartnerId
                    },
                    commandType: CommandType.Text,
                    cancellationToken: cancellationToken));

            if (belongsToPartner == 0)
            {
                throw new InvalidOperationException("O benefício informado não pertence ao parceiro autenticado.");
            }
        }

        var parameters = new DynamicParameters();

        parameters.Add("@BenefitId", request.BenefitId);
        parameters.Add("@BenefitRequestId", request.BenefitRequestId);

        parameters.Add("@UsedByType", request.UsedByType);
        parameters.Add("@UsedByUserId", request.UsedByUserId);
        parameters.Add("@UsedByClientId", request.UsedByClientId);
        parameters.Add("@UsedByPartnerCustomerId", request.UsedByPartnerCustomerId);

        parameters.Add("@PetSourceType", request.PetSourceType);
        parameters.Add("@ClientPetId", request.ClientPetId);
        parameters.Add("@PartnerCustomerPetId", request.PartnerCustomerPetId);

        parameters.Add("@RecordedByUserId", performedByUserId);
        parameters.Add("@ConfirmedByPartnerUserId", performedByPartner ? performedByUserId : null);
        parameters.Add("@ConfirmedByAdminUserId", performedByPartner ? null : performedByUserId);

        parameters.Add("@MonetaryValue", request.MonetaryValue);
        parameters.Add("@DiscountValue", request.DiscountValue);
        parameters.Add("@RuleSummary", request.RuleSummary);

        var result = await _connection.QueryFirstAsync<ConfirmBenefitUsageResult>(
            new CommandDefinition(
                "dbo.usp_benefit_usages_confirm",
                parameters,
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken));

        return result.BenefitUsageId;
    }

    public async Task CancelAsync(
        Guid usageId,
        CancelBenefitUsageRequest request,
        Guid? performedByUserId,
        CancellationToken cancellationToken = default)
    {
        const string sql = @"
            update dbo.benefit_usages
            set
                usage_status = 'cancelled',
                snapshot_rule_summary =
                    case
                        when @cancellation_reason is null or ltrim(rtrim(@cancellation_reason)) = ''
                            then snapshot_rule_summary
                        else concat(isnull(snapshot_rule_summary, ''), ' | Cancelado: ', @cancellation_reason)
                    end,
                updated_at = sysutcdatetime()
            where id = @benefit_usage_id
              and usage_status in ('confirmed', 'used');
        ";

        var affectedRows = await _connection.ExecuteAsync(
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

        if (affectedRows == 0)
        {
            throw new InvalidOperationException("Utilização não encontrada ou não pode ser cancelada no status atual.");
        }
    }

    public Task<BenefitUsageDetailDto?> GetByIdAsync(
        Guid usageId,
        CancellationToken cancellationToken = default)
    {
        const string sql = @"
            select top 1
                bu.id as Id,
                bu.benefit_id as BenefitId,
                bu.benefit_request_id as BenefitRequestId,
                bu.partner_id as PartnerId,

                p.trade_name as PartnerName,

                bu.used_by_user_id as UsedByUserId,
                bu.used_by_client_id as UsedByClientId,
                bu.used_by_partner_customer_id as UsedByPartnerCustomerId,
                bu.used_by_type as UsedByType,

                c.full_name as ClientName,
                pc.full_name as PartnerCustomerName,

                case
                    when bu.used_by_type = 'client' then c.full_name
                    when bu.used_by_type = 'partner_customer' then pc.full_name
                    else null
                end as UsedByName,

                bu.pet_source_type as PetSourceType,
                bu.client_pet_id as ClientPetId,
                bu.partner_customer_pet_id as PartnerCustomerPetId,

                case
                    when bu.pet_source_type = 'client_pet' then cp.name
                    when bu.pet_source_type = 'partner_customer_pet' then pcp.name
                    else null
                end as PetName,

                bu.usage_status as UsageStatus,
                bu.used_at as UsedAt,

                bu.recorded_by_user_id as RecordedByUserId,
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
            left join dbo.partners p
                on p.id = bu.partner_id
            left join dbo.clients c
                on c.id = bu.used_by_client_id
            left join dbo.partner_customers pc
                on pc.id = bu.used_by_partner_customer_id
            left join dbo.client_pets cp
                on cp.id = bu.client_pet_id
            left join dbo.partner_customer_pets pcp
                on pcp.id = bu.partner_customer_pet_id
            where bu.id = @benefit_usage_id;
        ";

        return _connection.QueryFirstOrDefaultAsync<BenefitUsageDetailDto>(
            new CommandDefinition(
                sql,
                new { benefit_usage_id = usageId },
                commandType: CommandType.Text,
                cancellationToken: cancellationToken));
    }

    public async Task<PagedResultDto<BenefitUsageListItemDto>> SearchAsync(
        BenefitUsageFilterDto filter,
        CancellationToken cancellationToken = default)
    {
        var page = filter.Page <= 0 ? 1 : filter.Page;
        var pageSize = filter.PageSize <= 0 ? 20 : filter.PageSize;
        var offset = (page - 1) * pageSize;

        const string sql = @"
            select
                bu.id as Id,
                bu.benefit_id as BenefitId,
                bu.benefit_request_id as BenefitRequestId,
                bu.partner_id as PartnerId,

                p.trade_name as PartnerName,

                bu.used_by_user_id as UsedByUserId,
                bu.used_by_client_id as UsedByClientId,
                bu.used_by_partner_customer_id as UsedByPartnerCustomerId,
                bu.used_by_type as UsedByType,

                c.full_name as ClientName,
                pc.full_name as PartnerCustomerName,

                case
                    when bu.used_by_type = 'client' then c.full_name
                    when bu.used_by_type = 'partner_customer' then pc.full_name
                    else null
                end as UsedByName,

                bu.pet_source_type as PetSourceType,
                bu.client_pet_id as ClientPetId,
                bu.partner_customer_pet_id as PartnerCustomerPetId,

                case
                    when bu.pet_source_type = 'client_pet' then cp.name
                    when bu.pet_source_type = 'partner_customer_pet' then pcp.name
                    else null
                end as PetName,

                bu.usage_status as UsageStatus,
                bu.used_at as UsedAt,

                bu.recorded_by_user_id as RecordedByUserId,
                bu.confirmed_by_partner_user_id as ConfirmedByPartnerUserId,
                bu.confirmed_by_admin_user_id as ConfirmedByAdminUserId,

                bu.monetary_value as MonetaryValue,
                bu.discount_value as DiscountValue,

                bu.snapshot_title as SnapshotTitle,
                bu.snapshot_partner_name as SnapshotPartnerName,
                bu.snapshot_rule_summary as SnapshotRuleSummary,

                bu.created_at as CreatedAt,
                bu.updated_at as UpdatedAt,

                count(1) over() as TotalCount,
                row_number() over(order by bu.used_at desc, bu.created_at desc) as Rn
            from dbo.benefit_usages bu
            left join dbo.partners p
                on p.id = bu.partner_id
            left join dbo.clients c
                on c.id = bu.used_by_client_id
            left join dbo.partner_customers pc
                on pc.id = bu.used_by_partner_customer_id
            left join dbo.client_pets cp
                on cp.id = bu.client_pet_id
            left join dbo.partner_customer_pets pcp
                on pcp.id = bu.partner_customer_pet_id
            where
                (
                    @search is null
                    or bu.snapshot_title like '%' + @search + '%'
                    or bu.snapshot_partner_name like '%' + @search + '%'
                    or c.full_name like '%' + @search + '%'
                    or pc.full_name like '%' + @search + '%'
                    or cp.name like '%' + @search + '%'
                    or pcp.name like '%' + @search + '%'
                )
                and (@benefit_id is null or bu.benefit_id = @benefit_id)
                and (@benefit_request_id is null or bu.benefit_request_id = @benefit_request_id)
                and (@partner_id is null or bu.partner_id = @partner_id)
                and (@usage_status is null or bu.usage_status = @usage_status)
                and (@used_by_type is null or bu.used_by_type = @used_by_type)
                and (@used_from is null or bu.used_at >= @used_from)
                and (@used_to is null or bu.used_at < dateadd(day, 1, @used_to))
            order by bu.used_at desc, bu.created_at desc
            offset @offset rows fetch next @page_size rows only;
        ";

        var parameters = new
        {
            search = string.IsNullOrWhiteSpace(filter.Search) ? null : filter.Search,
            benefit_id = filter.BenefitId,
            benefit_request_id = filter.BenefitRequestId,
            partner_id = filter.PartnerId,
            usage_status = filter.UsageStatus,
            used_by_type = filter.UsedByType,
            used_from = filter.UsedFrom,
            used_to = filter.UsedTo,
            offset,
            page_size = pageSize
        };

        var items = (await _connection.QueryAsync<BenefitUsageListItemDto>(
            new CommandDefinition(
                sql,
                parameters,
                commandType: CommandType.Text,
                cancellationToken: cancellationToken))).ToList();

        var totalCount = items.FirstOrDefault()?.TotalCount ?? 0;

        return new PagedResultDto<BenefitUsageListItemDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task<BenefitOfferEligibilityValidationResultDto?> ValidateAsync(
        ValidateBenefitUsageRequest request,
        CancellationToken cancellationToken = default)
    {
        const string benefitSql = @"
            select
                b.id as Id,
                b.status as Status,
                b.recurrence_value as RecurrenceValue,
                b.recurrence_period as RecurrencePeriod,
                b.recurrence_type as RecurrenceType,
                b.starts_at as StartsAt,
                b.ends_at as EndsAt
            from dbo.benefits b
            where b.id = @benefit_id
              and (@partner_id is null or b.partner_id = @partner_id);
        ";

        var benefit = await _connection.QueryFirstOrDefaultAsync<BenefitValidationRow>(
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
            return new BenefitOfferEligibilityValidationResultDto
            {
                IsAllowed = false,
                BlockReason = "Benefício não encontrado.",
                RuleSummary = "Sem benefício correspondente."
            };
        }

        if (benefit.Status != "active" && benefit.Status != "approved")
        {
            return new BenefitOfferEligibilityValidationResultDto
            {
                IsAllowed = false,
                BlockReason = "Benefício não está disponível para uso.",
                RuleSummary = "Status do benefício não permite utilização."
            };
        }

        var referenceDate = request.ValidationReferenceDate;

        if (benefit.StartsAt is not null && referenceDate < benefit.StartsAt.Value)
        {
            return new BenefitOfferEligibilityValidationResultDto
            {
                IsAllowed = false,
                BlockReason = "Benefício ainda não iniciou.",
                RuleSummary = "Uso antes da data inicial."
            };
        }

        if (benefit.EndsAt is not null && referenceDate > benefit.EndsAt.Value)
        {
            return new BenefitOfferEligibilityValidationResultDto
            {
                IsAllowed = false,
                BlockReason = "Benefício expirado.",
                RuleSummary = "Uso após a data final."
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
              and (
                    (@user_id is not null and user_id = @user_id)
                    or
                    (@partner_customer_id is not null and partner_customer_id = @partner_customer_id)
                  )
            order by updated_at desc, created_at desc;
        ";

        var usageLock = await _connection.QueryFirstOrDefaultAsync<BenefitOfferEligibilityValidationResultDto>(
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

        if (usageLock is not null &&
            usageLock.AllowedUses.HasValue &&
            usageLock.UsedCount.HasValue &&
            usageLock.UsedCount.Value >= usageLock.AllowedUses.Value)
        {
            usageLock.IsAllowed = false;
            usageLock.BlockReason = "Limite de utilização atingido.";
            usageLock.RuleSummary = "Controle por janela de recorrência.";
            return usageLock;
        }

        return new BenefitOfferEligibilityValidationResultDto
        {
            IsAllowed = true,
            BlockReason = null,
            NextAvailableAt = usageLock?.NextAvailableAt,
            AllowedUses = usageLock?.AllowedUses,
            UsedCount = usageLock?.UsedCount,
            RuleSummary = "Elegível para uso com base no banco atual."
        };
    }

    private sealed class ConfirmBenefitUsageResult
    {
        public Guid BenefitUsageId { get; set; }
    }

    private sealed class BenefitValidationRow
    {
        public Guid Id { get; set; }
        public string? Status { get; set; }
        public int? RecurrenceValue { get; set; }
        public string? RecurrencePeriod { get; set; }
        public string? RecurrenceType { get; set; }
        public DateTime? StartsAt { get; set; }
        public DateTime? EndsAt { get; set; }
    }
}