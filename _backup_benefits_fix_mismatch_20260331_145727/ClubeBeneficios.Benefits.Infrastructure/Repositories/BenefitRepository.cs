using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
    b.direction_label AS DirectionLabel,
    b.status AS Status,
    b.target_actor_type AS TargetActorType,
    b.target_actor_label AS TargetActorLabel,
    b.eligibility_type AS EligibilityType,
    b.eligibility_summary AS EligibilitySummary,
    b.recurrence_type AS RecurrenceType,
    b.recurrence_period AS RecurrencePeriod,
    b.recurrence_label AS RecurrenceLabel,
    b.validity_type AS ValidityType,
    b.validity_label AS ValidityLabel,
    b.highlight_in_showcase AS HighlightInShowcase,
    b.auto_activate_when_approved AS AutoActivateWhenApproved,
    b.requests_count AS RequestsCount,
    b.usages_count AS UsagesCount,
    b.conversion_rate AS ConversionRate,
    b.partner_level AS PartnerLevel,
    b.created_at AS CreatedAt,
    b.updated_at AS UpdatedAt
FROM dbo.vw_benefits_admin_list b
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
    b.direction_label AS DirectionLabel,
    b.status AS Status,
    b.approval_stage AS ApprovalStage,
    b.target_actor_type AS TargetActorType,
    b.target_actor_label AS TargetActorLabel,
    b.eligibility_summary AS EligibilitySummary,
    b.recurrence_label AS RecurrenceLabel,
    b.validity_label AS ValidityLabel,
    b.auto_activate_when_approved AS AutoActivateWhenApproved,
    b.requires_manual_release AS RequiresManualRelease,
    b.highlight_in_showcase AS HighlightInShowcase,
    b.stacking_rule AS StackingRule,
    b.approval_notes AS ApprovalNotes,
    b.rejection_reason AS RejectionReason,
    b.requests_count AS RequestsCount,
    b.usages_count AS UsagesCount,
    b.conversion_rate AS ConversionRate,
    b.partner_level AS PartnerLevel,
    b.created_at AS CreatedAt,
    b.updated_at AS UpdatedAt
FROM dbo.vw_benefits_pending_list b
{where}
ORDER BY b.created_at DESC
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
SELECT DISTINCT status AS Value, status AS Label
FROM dbo.benefits
WHERE status IS NOT NULL
ORDER BY status;

SELECT DISTINCT direction AS Value, direction AS Label
FROM dbo.benefits
WHERE direction IS NOT NULL
ORDER BY direction;

SELECT DISTINCT target_actor_type AS Value, target_actor_type AS Label
FROM dbo.benefits
WHERE target_actor_type IS NOT NULL
ORDER BY target_actor_type;

SELECT DISTINCT eligibility_type AS Value, eligibility_type AS Label
FROM dbo.benefits
WHERE eligibility_type IS NOT NULL
ORDER BY eligibility_type;

SELECT p.id AS Value, p.name AS Label
FROM dbo.partners p
ORDER BY p.name;";

        using var connection = await OpenConnectionAsync(cancellationToken);
        using var multi = await connection.QueryMultipleAsync(new CommandDefinition(sql, cancellationToken: cancellationToken));

        var statusOptions = (await multi.ReadAsync<LookupOptionDto>()).ToList();
        var directionOptions = (await multi.ReadAsync<LookupOptionDto>()).ToList();
        var targetActorOptions = (await multi.ReadAsync<LookupOptionDto>()).ToList();
        var eligibilityOptions = (await multi.ReadAsync<LookupOptionDto>()).ToList();
        var partnerOptions = (await multi.ReadAsync<LookupOptionDto>()).ToList();

        return new BenefitFilterOptionsDto
        {
            Statuses = statusOptions,
            Directions = directionOptions,
            TargetActorTypes = targetActorOptions,
            EligibilityTypes = eligibilityOptions,
            Partners = partnerOptions
        };
    }

    public async Task<BenefitDetailsDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        const string sql = @"
SELECT TOP (1)
    b.id AS Id,
    b.partner_id AS PartnerId,
    p.name AS PartnerName,
    b.title AS Title,
    b.benefit_type AS BenefitType,
    b.direction AS Direction,
    b.status AS Status,
    b.target_actor_type AS TargetActorType,
    b.short_description AS ShortDescription,
    b.full_description AS FullDescription,
    b.internal_notes AS InternalNotes,
    b.eligibility_type AS EligibilityType,
    b.eligibility_summary AS EligibilitySummary,
    b.recurrence_type AS RecurrenceType,
    b.recurrence_value AS RecurrenceValue,
    b.recurrence_period AS RecurrencePeriod,
    b.validity_type AS ValidityType,
    b.starts_at AS StartsAt,
    b.ends_at AS EndsAt,
    b.auto_activate_when_approved AS AutoActivateWhenApproved,
    b.requires_manual_release AS RequiresManualRelease,
    b.highlight_in_showcase AS HighlightInShowcase,
    b.stacking_rule AS StackingRule,
    b.created_at AS CreatedAt,
    b.updated_at AS UpdatedAt
FROM dbo.benefits b
LEFT JOIN dbo.partners p ON p.id = b.partner_id
WHERE b.id = @Id;

SELECT TOP (1)
    id AS Id,
    level_type AS LevelType,
    allowed_levels_csv AS LevelCode
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

        dto.LevelScope = await multi.ReadFirstOrDefaultAsync<BenefitLevelScopeDto>();
        dto.BehaviorRules = await multi.ReadFirstOrDefaultAsync<BenefitBehaviorRulesDto>();
        dto.CodeRules = await multi.ReadFirstOrDefaultAsync<BenefitCodeRulesDto>();
        return dto;
    }

    public async Task<Guid> CreateAsync(CreateBenefitRequest request, CancellationToken cancellationToken = default)
    {
        using var connection = await OpenConnectionAsync(cancellationToken);

        var parameters = BuildCreateOrUpdateParameters(request);
        var id = await connection.ExecuteScalarAsync<Guid>(new CommandDefinition(
            "dbo.usp_benefits_create",
            parameters,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken));

        return id;
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateBenefitRequest request, CancellationToken cancellationToken = default)
    {
        using var connection = await OpenConnectionAsync(cancellationToken);

        var parameters = BuildCreateOrUpdateParameters(request);
        parameters.Add("BenefitId", id);

        await connection.ExecuteAsync(new CommandDefinition(
            "dbo.usp_benefits_update",
            parameters,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken));

        return true;
    }

    public async Task<bool> ChangeStatusAsync(Guid id, ChangeBenefitStatusRequest request, CancellationToken cancellationToken = default)
    {
        using var connection = await OpenConnectionAsync(cancellationToken);

        var parameters = new DynamicParameters();
        parameters.Add("BenefitId", id);
        parameters.Add("Status", BenefitContractMapper.NormalizeStatus(GetStringValue(request, "Status")));
        parameters.Add("Reason", GetStringValue(request, "Reason"));
        parameters.Add("ChangedByUserId", GetGuidValue(request, "ChangedByUserId"));
        parameters.Add("ChangedByName", GetStringValue(request, "ChangedByName"));
        parameters.Add("ChangedByRole", GetStringValue(request, "ChangedByRole"));

        await connection.ExecuteAsync(new CommandDefinition(
            "dbo.usp_benefits_change_status",
            parameters,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken));

        return true;
    }

    public async Task<bool> AddReviewAsync(Guid id, AddBenefitReviewRequest request, CancellationToken cancellationToken = default)
    {
        using var connection = await OpenConnectionAsync(cancellationToken);

        var parameters = new DynamicParameters();
        parameters.Add("BenefitId", id);
        parameters.Add("ReviewType", GetStringValue(request, "ReviewType") ?? "manual_review");
        parameters.Add("Decision", GetStringValue(request, "Decision") ?? "adjustments_requested");
        parameters.Add("Notes", GetStringValue(request, "Notes"));
        parameters.Add("ReviewerUserId", GetGuidValue(request, "ReviewerUserId"));
        parameters.Add("ReviewerName", GetStringValue(request, "ReviewerName"));
        parameters.Add("ReviewerRole", GetStringValue(request, "ReviewerRole"));

        await connection.ExecuteAsync(new CommandDefinition(
            "dbo.usp_benefits_add_review",
            parameters,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken));

        return true;
    }

    private static DynamicParameters BuildCreateOrUpdateParameters(CreateBenefitRequest request)
    {
        var parameters = new DynamicParameters();
        FillSharedParameters(parameters, request);
        return parameters;
    }

    private static DynamicParameters BuildCreateOrUpdateParameters(UpdateBenefitRequest request)
    {
        var parameters = new DynamicParameters();
        FillSharedParameters(parameters, request);
        return parameters;
    }

    private static void FillSharedParameters(DynamicParameters parameters, dynamic request)
    {
        parameters.Add("PartnerId", GetGuidValue(request, "PartnerId"));
        parameters.Add("Title", GetStringValue(request, "Title"));
        parameters.Add("BenefitType", GetStringValue(request, "BenefitType"));
        parameters.Add("Direction", BenefitContractMapper.NormalizeDirection(GetStringValue(request, "Direction")));
        parameters.Add("TargetActorType", BenefitContractMapper.NormalizeTargetActorType(GetStringValue(request, "TargetActorType")));
        parameters.Add("ShortDescription", GetStringValue(request, "ShortDescription"));
        parameters.Add("FullDescription", GetStringValue(request, "FullDescription"));
        parameters.Add("InternalNotes", GetStringValue(request, "InternalNotes"));
        parameters.Add("Status", BenefitContractMapper.NormalizeStatus(GetStringValue(request, "Status")));
        parameters.Add("EligibilityType", BenefitContractMapper.NormalizeEligibilityType(GetStringValue(request, "EligibilityType")));
        parameters.Add("EligibilitySummary", BenefitContractMapper.BuildEligibilitySummary(request));
        parameters.Add("EligibilityChipsCsv", BenefitContractMapper.JoinList(GetStringListValue(request, "EligibilityChips")));
        parameters.Add("LevelType", GetStringValue(request, "LevelType"));
        parameters.Add("AllowedLevelsCsv", BenefitContractMapper.JoinList(GetStringListValue(request, "AllowedLevels")));
        parameters.Add("MinFrequencyEnabled", GetBoolValue(request, "MinFrequencyEnabled"));
        parameters.Add("MinFrequencyValue", GetNullableIntValue(request, "MinFrequencyValue"));
        parameters.Add("FrequencyWindowMonths", GetNullableIntValue(request, "FrequencyWindowMonths"));
        parameters.Add("MinTicketEnabled", GetBoolValue(request, "MinTicketEnabled"));
        parameters.Add("MinTicketValue", GetNullableDecimalValue(request, "MinTicketValue"));
        parameters.Add("TicketWindowMonths", GetNullableIntValue(request, "TicketWindowMonths"));
        parameters.Add("FirstUseOnly", GetBoolValue(request, "FirstUseOnly"));
        parameters.Add("RequiresMatilhaApproval", GetBoolValue(request, "RequiresMatilhaApproval"));
        parameters.Add("CustomRuleText", GetStringValue(request, "CustomRuleText"));
        parameters.Add("RequiresAccessCode", GetBoolValue(request, "RequiresAccessCode"));
        parameters.Add("AllowAnyActivePartnerCode", GetBoolValue(request, "AllowAnyActivePartnerCode"));
        parameters.Add("SpecificAccessCodeId", GetGuidValue(request, "SpecificAccessCodeId"));
        parameters.Add("CodeValidationMode", GetStringValue(request, "CodeValidationMode"));
        parameters.Add("RecurrenceType", BenefitContractMapper.NormalizeRecurrenceType(GetStringValue(request, "RecurrenceType")));
        parameters.Add("RecurrenceValue", GetNullableIntValue(request, "RecurrenceValue"));
        parameters.Add("RecurrencePeriod", GetStringValue(request, "RecurrencePeriod"));
        parameters.Add("ValidityType", BenefitContractMapper.NormalizeValidityType(GetStringValue(request, "ValidityType")));
        parameters.Add("StartsAt", GetNullableDateTimeValue(request, "StartsAt"));
        parameters.Add("EndsAt", GetNullableDateTimeValue(request, "EndsAt"));
        parameters.Add("AutoActivateWhenApproved", GetBoolValue(request, "AutoActivateWhenApproved"));
        parameters.Add("RequiresManualRelease", GetBoolValue(request, "RequiresManualRelease"));
        parameters.Add("HighlightInShowcase", GetBoolValue(request, "HighlightInShowcase"));
        parameters.Add("StackingRule", GetStringValue(request, "StackingRule"));
    }

    private static void AddCommonListFilters(BenefitFilterDto filter, StringBuilder where, DynamicParameters parameters)
    {
        AddLikeFilter(filter, where, parameters, "Search", "b.title");
        AddStringFilter(filter, where, parameters, "Status", "b.status", normalizeStatus: true);
        AddStringFilter(filter, where, parameters, "Direction", "b.direction");
        AddStringFilter(filter, where, parameters, "TargetActorType", "b.target_actor_type");
        AddStringFilter(filter, where, parameters, "EligibilityType", "b.eligibility_type");
        AddGuidFilter(filter, where, parameters, "PartnerId", "b.partner_id");
    }

    private static void AddCommonPendingFilters(BenefitFilterDto filter, StringBuilder where, DynamicParameters parameters)
    {
        AddLikeFilter(filter, where, parameters, "Search", "b.title");
        AddStringFilter(filter, where, parameters, "Status", "b.status", normalizeStatus: true);
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

    private static async Task<SqlConnection> OpenConnectionAsync(CancellationToken cancellationToken)
    {
        var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);
        return connection;
    }

    private static string? GetStringValue(object source, string propertyName)
        => source.GetType().GetProperty(propertyName)?.GetValue(source)?.ToString();

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