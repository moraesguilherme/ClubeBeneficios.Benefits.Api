using System.Data;
using System.Text;
using ClubeBeneficios.Benefits.Domain.Dtos;
using ClubeBeneficios.Benefits.Domain.Dtos.Filters;
using ClubeBeneficios.Benefits.Domain.Dtos.Requests;
using ClubeBeneficios.Benefits.Domain.Repositories;
using ClubeBeneficios.Benefits.Infrastructure.Helpers;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace ClubeBeneficios.Benefits.Infrastructure.Repositories;

public class BenefitRepository : IBenefitRepository
{
    private readonly string _connectionString;

    public BenefitRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? configuration.GetConnectionString("ClubeBeneficiosDb")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' ou 'ClubeBeneficiosDb' nÃ£o encontrada.");
    }

    public async Task<PagedResultDto<BenefitListItemDto>> GetPagedAsync(
        BenefitFilterDto filter,
        CancellationToken cancellationToken = default)
    {
        var page = GetIntValue(filter, "Page", 1);
        var pageSize = GetIntValue(filter, "PageSize", 10);
        var offset = (page - 1) * pageSize;

        var where = new StringBuilder(" WHERE 1 = 1 ");
        var parameters = new DynamicParameters();

        AddCommonListFilters(filter, where, parameters);
        parameters.Add("OffsetRows", offset);
        parameters.Add("FetchRows", pageSize);

        var countSql = $@"
                        SELECT COUNT(1)
                        FROM dbo.vw_benefits_admin_list b
                        {where};";

        var itemsSql = $@"
                        SELECT
                            b.id AS Id,
                            b.partner_id AS PartnerId,
                            b.partner_name AS PartnerName,
                            b.title AS Title,
                            b.benefit_type AS BenefitType,
                            b.direction AS Direction,
                            CASE
                                WHEN b.direction = 'partner_to_matilha' THEN 'Parceiro â†’ Cliente Matilha'
                                WHEN b.direction = 'matilha_to_partner' THEN 'Matilha â†’ Cliente do parceiro'
                                ELSE b.direction
                            END AS DirectionLabel,
                            b.status AS Status,
                            b.target_actor_type AS TargetActorType,
                            CASE
                                WHEN b.target_actor_type = 'client' THEN 'Cliente Matilha'
                                WHEN b.target_actor_type = 'partner_customer' THEN 'Cliente do parceiro'
                                ELSE b.target_actor_type
                            END AS TargetActorLabel,
                            b.eligibility_type AS EligibilityType,
                            CASE
                                WHEN b.eligibility_type = 'open' THEN 'Elegibilidade aberta'
                                WHEN b.eligibility_type = 'level' THEN 'Elegibilidade por nÃ­vel'
                                WHEN b.eligibility_type = 'behavior' THEN 'Elegibilidade por comportamento'
                                WHEN b.eligibility_type = 'code' THEN 'Elegibilidade por cÃ³digo'
                                WHEN b.eligibility_type = 'hybrid' THEN 'Elegibilidade hÃ­brida'
                                ELSE b.eligibility_type
                            END AS EligibilitySummary,
                            bt.recurrence_type AS RecurrenceType,
                            bt.recurrence_period AS RecurrencePeriod,
                            CASE
                                WHEN bt.recurrence_type = 'once_per_customer' THEN '1x por cliente'
                                WHEN bt.recurrence_type = 'limited_per_period' THEN CONCAT(
                                    COALESCE(CAST(bt.recurrence_value AS varchar(10)), '1'),
                                    'x por ',
                                    CASE bt.recurrence_period
                                        WHEN 'day' THEN 'dia'
                                        WHEN 'week' THEN 'semana'
                                        WHEN 'month' THEN 'mÃªs'
                                        WHEN 'quarter' THEN 'trimestre'
                                        WHEN 'semester' THEN 'semestre'
                                        WHEN 'year' THEN 'ano'
                                        ELSE COALESCE(bt.recurrence_period, 'perÃ­odo')
                                    END
                                )
                                WHEN bt.recurrence_type = 'unlimited_within_rule' THEN 'Ilimitado dentro da regra'
                                WHEN bt.recurrence_type = 'first_use_only' THEN 'Somente primeira utilizaÃ§Ã£o'
                                ELSE bt.recurrence_type
                            END AS RecurrenceLabel,
                            b.validity_type AS ValidityType,
                            CASE
                                WHEN b.validity_type = 'continuous' THEN 'ContÃ­nuo'
                                WHEN b.validity_type IN ('date_range', 'campaign_period')
                                    AND b.starts_at IS NOT NULL
                                    AND b.ends_at IS NOT NULL
                                    THEN CONCAT(CONVERT(varchar(10), b.starts_at, 103), ' atÃ© ', CONVERT(varchar(10), b.ends_at, 103))
                                WHEN b.validity_type IN ('date_range', 'campaign_period')
                                    AND b.ends_at IS NOT NULL
                                    THEN CONCAT('AtÃ© ', CONVERT(varchar(10), b.ends_at, 103))
                                WHEN b.validity_type IN ('date_range', 'campaign_period')
                                    AND b.starts_at IS NOT NULL
                                    THEN CONCAT('A partir de ', CONVERT(varchar(10), b.starts_at, 103))
                                WHEN b.validity_type = 'until_stock' THEN 'AtÃ© durar o estoque'
                                ELSE b.validity_type
                            END AS ValidityLabel,
                            b.highlight_in_showcase AS HighlightInShowcase,
                            bt.auto_activate_when_approved AS AutoActivateWhenApproved,
                            b.requests_count AS RequestsCount,
                            b.usages_count AS UsagesCount,
                            CAST(ROUND(ISNULL(b.conversion_rate, 0), 0) AS int) AS ConversionRate,
                            b.partner_level AS PartnerLevel,
                            b.created_at AS CreatedAt,
                            b.updated_at AS UpdatedAt
                        FROM dbo.vw_benefits_admin_list b
                        INNER JOIN dbo.benefits bt ON bt.id = b.id
                        {where}
                        ORDER BY b.created_at DESC
                        OFFSET @OffsetRows ROWS FETCH NEXT @FetchRows ROWS ONLY;";

        using var connection = await OpenConnectionAsync(cancellationToken);
        var totalCount = await connection.ExecuteScalarAsync<int>(new CommandDefinition(countSql, parameters, cancellationToken: cancellationToken));
        var items = (await connection.QueryAsync<BenefitListItemDto>(new CommandDefinition(itemsSql, parameters, cancellationToken: cancellationToken))).ToList();

        return new PagedResultDto<BenefitListItemDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task<PagedResultDto<BenefitApprovalItemDto>> GetPendingAsync(
        BenefitFilterDto filter,
        CancellationToken cancellationToken = default)
    {
        var page = GetIntValue(filter, "Page", 1);
        var pageSize = GetIntValue(filter, "PageSize", 10);
        var offset = (page - 1) * pageSize;

        var where = new StringBuilder(" WHERE 1 = 1 ");
        var parameters = new DynamicParameters();

        AddCommonPendingFilters(filter, where, parameters);
        parameters.Add("OffsetRows", offset);
        parameters.Add("FetchRows", pageSize);

        var countSql = $@"
                        SELECT COUNT(1)
                        FROM dbo.vw_benefits_pending_list b
                        {where};";

        var itemsSql = $@"
                        SELECT
                            b.id AS Id,
                            b.partner_id AS PartnerId,
                            b.partner_name AS PartnerName,
                            b.title AS Title,
                            b.benefit_type AS BenefitType,
                            b.direction AS Direction,
                            CASE
                                WHEN b.direction = 'partner_to_matilha' THEN 'Parceiro â†’ Cliente Matilha'
                                WHEN b.direction = 'matilha_to_partner' THEN 'Matilha â†’ Cliente do parceiro'
                                ELSE b.direction
                            END AS DirectionLabel,
                            bt.status AS Status,
                            CASE
                                WHEN bt.status = 'pending_review' THEN 'pending_review'
                                WHEN bt.status = 'under_review' THEN 'under_review'
                                ELSE bt.status
                            END AS ApprovalStage,
                            b.target_actor_type AS TargetActorType,
                            CASE
                                WHEN b.target_actor_type = 'client' THEN 'Cliente Matilha'
                                WHEN b.target_actor_type = 'partner_customer' THEN 'Cliente do parceiro'
                                ELSE b.target_actor_type
                            END AS TargetActorLabel,
                            bt.eligibility_type AS EligibilityType,
                            CASE
                                WHEN bt.eligibility_type = 'open' THEN 'Elegibilidade aberta'
                                WHEN bt.eligibility_type = 'level' THEN 'Elegibilidade por nÃ­vel'
                                WHEN bt.eligibility_type = 'behavior' THEN 'Elegibilidade por comportamento'
                                WHEN bt.eligibility_type = 'code' THEN 'Elegibilidade por cÃ³digo'
                                WHEN bt.eligibility_type = 'hybrid' THEN 'Elegibilidade hÃ­brida'
                                ELSE bt.eligibility_type
                            END AS EligibilitySummary,
                            bt.auto_activate_when_approved AS AutoActivateWhenApproved,
                            bt.requires_manual_release AS RequiresManualRelease,
                            bt.highlight_in_showcase AS HighlightInShowcase,
                            b.review_point AS LastReviewType,
                            b.review_recommendation AS LastReviewNotes,
                            CAST(NULL AS varchar(200)) AS LastReviewedBy,
                            b.reviewed_at AS LastReviewedAt,
                            bt.created_at AS CreatedAt,
                            bt.updated_at AS UpdatedAt
                        FROM dbo.vw_benefits_pending_list b
                        INNER JOIN dbo.benefits bt ON bt.id = b.id
                        {where}
                        ORDER BY bt.created_at DESC
                        OFFSET @OffsetRows ROWS FETCH NEXT @FetchRows ROWS ONLY;";

        using var connection = await OpenConnectionAsync(cancellationToken);
        var totalCount = await connection.ExecuteScalarAsync<int>(new CommandDefinition(countSql, parameters, cancellationToken: cancellationToken));
        var items = (await connection.QueryAsync<BenefitApprovalItemDto>(new CommandDefinition(itemsSql, parameters, cancellationToken: cancellationToken))).ToList();

        return new PagedResultDto<BenefitApprovalItemDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task<BenefitDashboardSummaryDto> GetDashboardSummaryAsync(CancellationToken cancellationToken = default)
    {
        const string sql = @"
                            SELECT
                                COUNT(1) AS TotalBenefits,
                                SUM(CASE WHEN b.status = 'active' THEN 1 ELSE 0 END) AS ActiveBenefits,
                                SUM(CASE WHEN b.status IN ('pending_review','under_review') THEN 1 ELSE 0 END) AS PendingBenefits,
                                SUM(CASE WHEN b.status = 'inactive' THEN 1 ELSE 0 END) AS InactiveBenefits,
                                ISNULL(SUM(ISNULL(b.requests_count, 0)), 0) AS TotalRequests,
                                ISNULL(SUM(ISNULL(b.usages_count, 0)), 0) AS TotalUsages,
                                ISNULL(CAST(AVG(CAST(ISNULL(b.conversion_rate, 0) AS decimal(10,2))) AS int), 0) AS AverageConversionRate
                            FROM dbo.vw_benefits_admin_list b;";

        using var connection = await OpenConnectionAsync(cancellationToken);
        var result = await connection.QueryFirstOrDefaultAsync<BenefitDashboardSummaryDto>(new CommandDefinition(sql, cancellationToken: cancellationToken));
        return result ?? new BenefitDashboardSummaryDto();
    }

    public async Task<BenefitFilterOptionsDto> GetFilterOptionsAsync(CancellationToken cancellationToken = default)
    {
        const string sql = @"
                            SELECT DISTINCT status AS Code, status AS Label
                            FROM dbo.benefits
                            WHERE status IS NOT NULL
                            ORDER BY status;

                            SELECT DISTINCT direction AS Code, direction AS Label
                            FROM dbo.benefits
                            WHERE direction IS NOT NULL
                            ORDER BY direction;

                            SELECT DISTINCT target_actor_type AS Code, target_actor_type AS Label
                            FROM dbo.benefits
                            WHERE target_actor_type IS NOT NULL
                            ORDER BY target_actor_type;

                            SELECT DISTINCT eligibility_type AS Code, eligibility_type AS Label
                            FROM dbo.benefits
                            WHERE eligibility_type IS NOT NULL
                            ORDER BY eligibility_type;

                            SELECT CAST(p.id AS varchar(36)) AS Code, p.trade_name AS Label
                            FROM dbo.partners p
                            ORDER BY p.trade_name;";

        using var connection = await OpenConnectionAsync(cancellationToken);
        using var multi = await connection.QueryMultipleAsync(new CommandDefinition(sql, cancellationToken: cancellationToken));

        var statusOptions = (await multi.ReadAsync<LookupOptionDto>()).ToList();
        var directionOptions = (await multi.ReadAsync<LookupOptionDto>()).ToList();
        var targetActorOptions = (await multi.ReadAsync<LookupOptionDto>()).ToList();
        var eligibilityOptions = (await multi.ReadAsync<LookupOptionDto>()).ToList();
        var partnerOptions = (await multi.ReadAsync<LookupOptionDto>()).ToList();

        return new BenefitFilterOptionsDto
        {
            Origins = directionOptions,
            Statuses = statusOptions,
            Audiences = targetActorOptions,
            EligibilityTypes = eligibilityOptions,
            Partners = partnerOptions,
            Directions = directionOptions,
            TargetActorTypes = targetActorOptions
        };
    }

    public async Task<BenefitDetailsDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        const string sql = @"
                            SELECT TOP (1)
                                b.id AS Id,
                                b.partner_id AS PartnerId,
                                p.trade_name AS PartnerName,
                                b.title AS Title,
                                b.benefit_type AS BenefitType,
                                b.direction AS Direction,
                                b.status AS Status,
                                b.target_actor_type AS TargetActorType,
                                b.short_description AS ShortDescription,
                                b.full_description AS FullDescription,
                                b.internal_notes AS InternalNotes,
                                b.eligibility_type AS EligibilityType,
                                CASE
                                    WHEN b.eligibility_type = 'open' THEN 'Elegibilidade aberta'
                                    WHEN b.eligibility_type = 'level' THEN 'Elegibilidade por nÃ­vel'
                                    WHEN b.eligibility_type = 'behavior' THEN 'Elegibilidade por comportamento'
                                    WHEN b.eligibility_type = 'code' THEN 'Elegibilidade por cÃ³digo'
                                    WHEN b.eligibility_type = 'hybrid' THEN 'Elegibilidade hÃ­brida'
                                    ELSE b.eligibility_type
                                END AS EligibilitySummary,
                                b.recurrence_type AS RecurrenceType,
                                b.recurrence_value AS RecurrenceLimit,
                                b.recurrence_period AS RecurrencePeriod,
                                b.starts_at AS ValidFrom,
                                b.ends_at AS ValidUntil,
                                b.auto_activate_when_approved AS AutoActivateWhenApproved,
                                b.requires_manual_release AS RequiresManualRelease,
                                b.highlight_in_showcase AS HighlightInShowcase,
                                b.stacking_rule AS StackingRule,
                                b.created_at AS CreatedAt,
                                b.updated_at AS UpdatedAt
                            FROM dbo.benefits b
                            LEFT JOIN dbo.partners p ON p.id = b.partner_id
                            WHERE b.id = @Id;

                            SELECT
                                level_type AS LevelType,
                                level_code AS LevelCode
                            FROM dbo.benefit_level_scopes
                            WHERE benefit_id = @Id
                            ORDER BY created_at DESC;

                            SELECT TOP (1)
                                min_frequency_enabled AS MinFrequencyEnabled,
                                min_frequency_value AS MinFrequencyValue,
                                frequency_window_months AS FrequencyWindowMonths,
                                min_ticket_enabled AS MinTicketEnabled,
                                min_ticket_value AS MinTicketValue,
                                ticket_window_months AS TicketWindowMonths,
                                first_use_only AS FirstUseOnly,
                                requires_matilha_approval AS RequiresMatilhaApproval,
                                custom_rule_text AS CustomRuleText
                            FROM dbo.benefit_behavior_rules
                            WHERE benefit_id = @Id
                            ORDER BY created_at DESC;

                            SELECT TOP (1)
                                requires_access_code AS RequiresAccessCode,
                                allow_any_active_partner_code AS AllowAnyActivePartnerCode,
                                specific_access_code_id AS SpecificAccessCodeId,
                                code_validation_mode AS CodeValidationMode
                            FROM dbo.benefit_code_rules
                            WHERE benefit_id = @Id
                            ORDER BY created_at DESC;";

        using var connection = await OpenConnectionAsync(cancellationToken);
        using var multi = await connection.QueryMultipleAsync(new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));

        var dto = await multi.ReadFirstOrDefaultAsync<BenefitDetailsDto>();
        if (dto is null)
            return null;

        dto.LevelScopes = (await multi.ReadAsync<BenefitLevelScopeDto>()).ToList();
        dto.BehaviorRules = await multi.ReadFirstOrDefaultAsync<BenefitBehaviorRulesDto>();
        dto.CodeRules = await multi.ReadFirstOrDefaultAsync<BenefitCodeRulesDto>();
        return dto;
    }

    public async Task<Guid> CreateAsync(CreateBenefitRequest request, CancellationToken cancellationToken = default)
    {
        using var connection = await OpenConnectionAsync(cancellationToken);

        var parameters = BuildCreateParameters(request);

        var id = await connection.ExecuteScalarAsync<Guid>(new CommandDefinition(
            "dbo.usp_benefits_create",
            parameters,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken));

        return id;
    }

    private async Task<string?> GetCurrentStatusAsync(Guid id, CancellationToken cancellationToken = default)
    {
        const string sql = @"
        SELECT TOP (1) status
        FROM dbo.benefits
        WHERE id = @Id;";

        using var connection = await OpenConnectionAsync(cancellationToken);

        return await connection.ExecuteScalarAsync<string?>(new CommandDefinition(
            sql,
            new { Id = id },
            cancellationToken: cancellationToken));
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateBenefitRequest request, CancellationToken cancellationToken = default)
    {
        var currentStatus = await GetCurrentStatusAsync(id, cancellationToken);

        if (string.IsNullOrWhiteSpace(currentStatus))
            return false;

        using var connection = await OpenConnectionAsync(cancellationToken);

        var parameters = BuildUpdateParameters(id, request);

        await connection.ExecuteAsync(new CommandDefinition(
            "dbo.usp_benefits_update",
            parameters,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken));

        var normalizedRequestedStatus = string.IsNullOrWhiteSpace(request.Status)
            ? null
            : BenefitContractMapper.NormalizeStatus(request.Status);

        var normalizedCurrentStatus = string.IsNullOrWhiteSpace(currentStatus)
            ? null
            : BenefitContractMapper.NormalizeStatus(currentStatus);

        if (!string.IsNullOrWhiteSpace(normalizedRequestedStatus) &&
            !string.Equals(normalizedRequestedStatus, normalizedCurrentStatus, StringComparison.OrdinalIgnoreCase))
        {
            var statusParameters = new DynamicParameters();
            statusParameters.Add("BenefitId", id);
            statusParameters.Add("NewStatus", normalizedRequestedStatus);
            statusParameters.Add("Reason", "Status alterado durante edição do benefício.");
            statusParameters.Add("ChangedByUserId", request.UpdatedByUserId);

            await connection.ExecuteAsync(new CommandDefinition(
                "dbo.usp_benefits_change_status",
                statusParameters,
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken));
        }

        return true;
    }

    public async Task<bool> ChangeStatusAsync(Guid id, ChangeBenefitStatusRequest request, CancellationToken cancellationToken = default)
    {
        using var connection = await OpenConnectionAsync(cancellationToken);

        var parameters = new DynamicParameters();
        parameters.Add("BenefitId", id);
        parameters.Add("NewStatus", BenefitContractMapper.NormalizeStatus(GetStringValue(request, "Status") ?? GetStringValue(request, "NewStatus")));
        parameters.Add("Reason", GetStringValue(request, "Reason"));
        parameters.Add("ChangedByUserId", request.ChangedByUserId);

        await connection.ExecuteAsync(new CommandDefinition(
            "dbo.usp_benefits_change_status",
            parameters,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken));

        return true;
    }

    public async Task<bool> AddReviewAsync(
    Guid id,
    AddBenefitReviewRequest request,
    CancellationToken cancellationToken = default)
    {
        using var connection = await OpenConnectionAsync(cancellationToken);

        var parameters = new DynamicParameters();
        parameters.Add("BenefitId", id);
        parameters.Add("ReviewStatus", NormalizeReviewStatus(request.ReviewStatus));
        parameters.Add("ReviewPoint", request.ReviewPoint);
        parameters.Add("ReviewRecommendation", request.ReviewRecommendation);
        parameters.Add("ReviewedByUserId", request.ReviewedByUserId);

        await connection.ExecuteAsync(new CommandDefinition(
            "dbo.usp_benefits_add_review",
            parameters,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken));

        return true;
    }

    private static DynamicParameters BuildCreateParameters(CreateBenefitRequest request)
    {
        var parameters = new DynamicParameters();

        parameters.Add("PartnerId", request.PartnerId);
        parameters.Add("Title", request.Title);
        parameters.Add("BenefitType", request.BenefitType);
        parameters.Add("Direction", BenefitContractMapper.NormalizeDirection(request.Direction));
        parameters.Add("TargetActorType", BenefitContractMapper.NormalizeTargetActorType(request.TargetActorType));
        parameters.Add("ShortDescription", request.ShortDescription);
        parameters.Add("FullDescription", request.FullDescription);
        parameters.Add("InternalNotes", request.InternalNotes);
        parameters.Add("EligibilityType", BenefitContractMapper.NormalizeEligibilityType(request.EligibilityType));
        parameters.Add("AllowFirstUseOnly", request.FirstUseOnly);
        parameters.Add("RequiresActiveAccessCode", request.RequiresAccessCode);
        parameters.Add("RequiresPartnerAvailability", false);
        parameters.Add("RequiresMatilhaAcceptanceRules", request.RequiresMatilhaApproval);
        parameters.Add("LevelType", request.LevelType);
        parameters.Add("LevelCodesCsv", BenefitContractMapper.JoinList(request.AllowedLevels));
        parameters.Add("MinFrequencyEnabled", request.MinFrequencyEnabled);
        parameters.Add("MinFrequencyValue", request.MinFrequencyValue);
        parameters.Add("FrequencyWindowMonths", request.FrequencyWindowMonths);
        parameters.Add("MinTicketEnabled", request.MinTicketEnabled);
        parameters.Add("MinTicketValue", request.MinTicketValue);
        parameters.Add("TicketWindowMonths", request.TicketWindowMonths);
        parameters.Add("BehaviorFirstUseOnly", request.FirstUseOnly);
        parameters.Add("BehaviorRequiresMatilhaApproval", request.RequiresMatilhaApproval);
        parameters.Add("CustomRuleText", request.CustomRuleText);
        parameters.Add("RequiresAccessCode", request.RequiresAccessCode);
        parameters.Add("AllowAnyActivePartnerCode", request.AllowAnyActivePartnerCode);
        parameters.Add("SpecificAccessCodeId", request.SpecificAccessCodeId);
        parameters.Add("CodeValidationMode", string.IsNullOrWhiteSpace(request.CodeValidationMode) ? "partner_code" : request.CodeValidationMode);
        parameters.Add("RecurrenceType", BenefitContractMapper.NormalizeRecurrenceType(request.RecurrenceType));
        parameters.Add("RecurrenceValue", request.RecurrenceValue);
        parameters.Add("RecurrencePeriod", request.RecurrencePeriod);
        parameters.Add("ValidityType", BenefitContractMapper.NormalizeValidityType(request.ValidityType));
        parameters.Add("StartsAt", request.StartsAt);
        parameters.Add("EndsAt", request.EndsAt);
        parameters.Add("AutoActivateWhenApproved", request.AutoActivateWhenApproved);
        parameters.Add("RequiresManualRelease", request.RequiresManualRelease);
        parameters.Add("HighlightInShowcase", request.HighlightInShowcase);
        parameters.Add("StackingRule", request.StackingRule);
        parameters.Add("CreatedByUserId", request.CreatedByUserId);
        parameters.Add("InitialStatus", BenefitContractMapper.NormalizeStatus(request.Status));

        return parameters;
    }

    private static DynamicParameters BuildUpdateParameters(Guid id, UpdateBenefitRequest request)
    {
        var parameters = new DynamicParameters();

        parameters.Add("BenefitId", id);
        parameters.Add("PartnerId", request.PartnerId);
        parameters.Add("Title", request.Title);
        parameters.Add("BenefitType", request.BenefitType);
        parameters.Add("Direction", BenefitContractMapper.NormalizeDirection(request.Direction));
        parameters.Add("TargetActorType", BenefitContractMapper.NormalizeTargetActorType(request.TargetActorType));
        parameters.Add("ShortDescription", request.ShortDescription);
        parameters.Add("FullDescription", request.FullDescription);
        parameters.Add("InternalNotes", request.InternalNotes);
        parameters.Add("Status", string.IsNullOrWhiteSpace(request.Status) ? null : BenefitContractMapper.NormalizeStatus(request.Status));
        parameters.Add("EligibilityType", BenefitContractMapper.NormalizeEligibilityType(request.EligibilityType));
        parameters.Add("AllowFirstUseOnly", request.FirstUseOnly);
        parameters.Add("RequiresActiveAccessCode", request.RequiresAccessCode);
        parameters.Add("RequiresPartnerAvailability", false);
        parameters.Add("RequiresMatilhaAcceptanceRules", request.RequiresMatilhaApproval);
        parameters.Add("LevelType", request.LevelType);
        parameters.Add("LevelCodesCsv", BenefitContractMapper.JoinList(request.AllowedLevels));
        parameters.Add("MinFrequencyEnabled", request.MinFrequencyEnabled);
        parameters.Add("MinFrequencyValue", request.MinFrequencyValue);
        parameters.Add("FrequencyWindowMonths", request.FrequencyWindowMonths);
        parameters.Add("MinTicketEnabled", request.MinTicketEnabled);
        parameters.Add("MinTicketValue", request.MinTicketValue);
        parameters.Add("TicketWindowMonths", request.TicketWindowMonths);
        parameters.Add("BehaviorFirstUseOnly", request.FirstUseOnly);
        parameters.Add("BehaviorRequiresMatilhaApproval", request.RequiresMatilhaApproval);
        parameters.Add("CustomRuleText", request.CustomRuleText);
        parameters.Add("RequiresAccessCode", request.RequiresAccessCode);
        parameters.Add("AllowAnyActivePartnerCode", request.AllowAnyActivePartnerCode);
        parameters.Add("SpecificAccessCodeId", request.SpecificAccessCodeId);
        parameters.Add("CodeValidationMode", string.IsNullOrWhiteSpace(request.CodeValidationMode) ? "partner_code" : request.CodeValidationMode);
        parameters.Add("RecurrenceType", BenefitContractMapper.NormalizeRecurrenceType(request.RecurrenceType));
        parameters.Add("RecurrenceValue", request.RecurrenceValue);
        parameters.Add("RecurrencePeriod", request.RecurrencePeriod);
        parameters.Add("ValidityType", BenefitContractMapper.NormalizeValidityType(request.ValidityType));
        parameters.Add("StartsAt", request.StartsAt);
        parameters.Add("EndsAt", request.EndsAt);
        parameters.Add("AutoActivateWhenApproved", request.AutoActivateWhenApproved);
        parameters.Add("RequiresManualRelease", request.RequiresManualRelease);
        parameters.Add("HighlightInShowcase", request.HighlightInShowcase);
        parameters.Add("StackingRule", request.StackingRule);
        parameters.Add("UpdatedByUserId", request.UpdatedByUserId);

        return parameters;
    }

    private static void AddCommonListFilters(BenefitFilterDto filter, StringBuilder where, DynamicParameters parameters)
    {
        AddLikeFilter(filter, where, parameters, "Search", "b.title");
        AddStringFilter(filter, where, parameters, "Status", "b.status", normalizeStatus: true);
        AddStringFilter(filter, where, parameters, "Origin", "b.direction");
        AddStringFilter(filter, where, parameters, "TargetActorType", "b.target_actor_type");
        AddStringFilter(filter, where, parameters, "EligibilityType", "b.eligibility_type");
        AddGuidFilter(filter, where, parameters, "PartnerId", "b.partner_id");
    }

    private static void AddCommonPendingFilters(BenefitFilterDto filter, StringBuilder where, DynamicParameters parameters)
    {
        AddLikeFilter(filter, where, parameters, "Search", "b.title");
        AddStringFilter(filter, where, parameters, "Status", "b.status", normalizeStatus: true);
        AddStringFilter(filter, where, parameters, "Origin", "b.direction");
        AddGuidFilter(filter, where, parameters, "PartnerId", "b.partner_id");
    }

    private static void AddLikeFilter(object source, StringBuilder where, DynamicParameters parameters, string propertyName, string columnName)
    {
        var value = GetStringValue(source, propertyName);
        if (string.IsNullOrWhiteSpace(value))
            return;

        where.Append($" AND {columnName} LIKE @{propertyName} ");
        parameters.Add(propertyName, $"%{value.Trim()}%");
    }

    private static void AddStringFilter(object source, StringBuilder where, DynamicParameters parameters, string propertyName, string columnName, bool normalizeStatus = false)
    {
        var value = GetStringValue(source, propertyName);
        if (string.IsNullOrWhiteSpace(value))
            return;

        if (normalizeStatus)
            value = BenefitContractMapper.NormalizeStatus(value);

        where.Append($" AND {columnName} = @{propertyName} ");
        parameters.Add(propertyName, value);
    }

    private static void AddGuidFilter(object source, StringBuilder where, DynamicParameters parameters, string propertyName, string columnName)
    {
        var value = GetGuidValue(source, propertyName);
        if (!value.HasValue || value.Value == Guid.Empty)
            return;

        where.Append($" AND {columnName} = @{propertyName} ");
        parameters.Add(propertyName, value.Value);
    }

    private async Task<SqlConnection> OpenConnectionAsync(CancellationToken cancellationToken)
    {
        var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);
        return connection;
    }

    private static string NormalizeReviewStatus(string? value)
    {
        var normalized = (value ?? string.Empty).Trim().ToLowerInvariant();

        return normalized switch
        {
            "approved" => "approved",
            "aprovado" => "approved",
            "rejected" => "rejected",
            "reprovado" => "rejected",
            "changes_requested" => "changes_requested",
            "adjustments_requested" => "changes_requested",
            "ajustes_solicitados" => "changes_requested",
            _ => throw new InvalidOperationException($"ReviewStatus inválido: '{value}'.")
        };
    }

    private static string? GetStringValue(object source, string propertyName)
        => source.GetType().GetProperty(propertyName)?.GetValue(source)?.ToString();

    private static string? GetFirstStringValue(object source, params string[] propertyNames)
    {
        foreach (var propertyName in propertyNames)
        {
            var value = GetStringValue(source, propertyName);
            if (!string.IsNullOrWhiteSpace(value))
                return value;
        }

        return null;
    }

    private static Guid? GetFirstGuidValue(object source, params string[] propertyNames)
    {
        foreach (var propertyName in propertyNames)
        {
            var value = GetGuidValue(source, propertyName);
            if (value.HasValue && value.Value != Guid.Empty)
                return value;
        }

        return null;
    }

    private static Guid? GetGuidValue(object source, string propertyName)
    {
        var value = source.GetType().GetProperty(propertyName)?.GetValue(source);

        if (value is null)
            return null;

        if (value is Guid guid)
            return guid == Guid.Empty ? null : guid;

        var valueType = value.GetType();
        var underlyingType = Nullable.GetUnderlyingType(valueType);
        if (underlyingType == typeof(Guid))
        {
            var boxed = (Guid?)value;
            return boxed.HasValue && boxed.Value != Guid.Empty ? boxed.Value : null;
        }

        if (value is string text && Guid.TryParse(text, out var parsed))
            return parsed == Guid.Empty ? null : parsed;

        return null;
    }

    private static bool GetBoolValue(object source, string propertyName)
    {
        var value = source.GetType().GetProperty(propertyName)?.GetValue(source);

        if (value is null)
            return false;

        if (value is bool boolean)
            return boolean;

        var valueType = value.GetType();
        var underlyingType = Nullable.GetUnderlyingType(valueType);
        if (underlyingType == typeof(bool))
        {
            var boxed = (bool?)value;
            return boxed ?? false;
        }

        if (value is string text && bool.TryParse(text, out var parsed))
            return parsed;

        return false;
    }

    private static int GetIntValue(object source, string propertyName, int fallback)
    {
        var value = source.GetType().GetProperty(propertyName)?.GetValue(source);

        if (value is null)
            return fallback;

        if (value is int integer)
            return integer;

        var valueType = value.GetType();
        var underlyingType = Nullable.GetUnderlyingType(valueType);
        if (underlyingType == typeof(int))
        {
            var boxed = (int?)value;
            return boxed ?? fallback;
        }

        if (value is string text && int.TryParse(text, out var parsed))
            return parsed;

        return fallback;
    }

    private static int? GetNullableIntValue(object source, string propertyName)
    {
        var value = source.GetType().GetProperty(propertyName)?.GetValue(source);

        if (value is null)
            return null;

        if (value is int integer)
            return integer;

        var valueType = value.GetType();
        var underlyingType = Nullable.GetUnderlyingType(valueType);
        if (underlyingType == typeof(int))
            return (int?)value;

        if (value is string text && int.TryParse(text, out var parsed))
            return parsed;

        return null;
    }

    private static decimal? GetNullableDecimalValue(object source, string propertyName)
    {
        var value = source.GetType().GetProperty(propertyName)?.GetValue(source);

        if (value is null)
            return null;

        if (value is decimal dec)
            return dec;

        if (value is double dbl)
            return Convert.ToDecimal(dbl);

        if (value is float flt)
            return Convert.ToDecimal(flt);

        var valueType = value.GetType();
        var underlyingType = Nullable.GetUnderlyingType(valueType);
        if (underlyingType == typeof(decimal))
            return (decimal?)value;

        if (underlyingType == typeof(double))
        {
            var boxed = (double?)value;
            return boxed.HasValue ? Convert.ToDecimal(boxed.Value) : null;
        }

        if (underlyingType == typeof(float))
        {
            var boxed = (float?)value;
            return boxed.HasValue ? Convert.ToDecimal(boxed.Value) : null;
        }

        if (value is string text && decimal.TryParse(text, out var parsed))
            return parsed;

        return null;
    }

    private static DateTime? GetNullableDateTimeValue(object source, string propertyName)
    {
        var value = source.GetType().GetProperty(propertyName)?.GetValue(source);

        if (value is null)
            return null;

        if (value is DateTime dt)
            return dt;

        var valueType = value.GetType();
        var underlyingType = Nullable.GetUnderlyingType(valueType);
        if (underlyingType == typeof(DateTime))
            return (DateTime?)value;

        if (value is string text && DateTime.TryParse(text, out var parsed))
            return parsed;

        return null;
    }

    private static IEnumerable<string> GetStringListValue(object source, string propertyName)
    {
        var value = source.GetType().GetProperty(propertyName)?.GetValue(source);

        if (value is IEnumerable<string> list)
            return list;

        if (value is string text)
            return string.IsNullOrWhiteSpace(text)
                ? Array.Empty<string>()
                : text.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        return Array.Empty<string>();
    }
}
