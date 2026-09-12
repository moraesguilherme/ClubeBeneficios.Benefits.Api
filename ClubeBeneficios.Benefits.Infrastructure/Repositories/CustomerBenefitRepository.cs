using System.Data;
using ClubeBeneficios.Benefits.Domain.Dtos.Common;
using ClubeBeneficios.Benefits.Domain.Dtos.Customer;
using ClubeBeneficios.Benefits.Domain.Repositories;
using Dapper;

namespace ClubeBeneficios.Benefits.Infrastructure.Repositories;

public sealed class CustomerBenefitRepository : ICustomerBenefitRepository
{
    private readonly IDbConnection _connection;

    public CustomerBenefitRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<Guid?> GetClientIdByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT TOP (1)
                id
            FROM dbo.clients
            WHERE user_id = @UserId
              AND status NOT IN ('blocked', 'archived')
            ORDER BY created_at DESC;
        """;

        return await _connection.ExecuteScalarAsync<Guid?>(
            new CommandDefinition(
                sql,
                new { UserId = userId },
                commandType: CommandType.Text,
                cancellationToken: cancellationToken));
    }

    public async Task<bool> ClientPetBelongsToClientAsync(
        Guid clientId,
        Guid petId,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT COUNT(1)
            FROM dbo.client_pets
            WHERE id = @PetId
              AND client_id = @ClientId
              AND status NOT IN ('blocked', 'archived');
        """;

        var count = await _connection.ExecuteScalarAsync<int>(
            new CommandDefinition(
                sql,
                new
                {
                    ClientId = clientId,
                    PetId = petId
                },
                commandType: CommandType.Text,
                cancellationToken: cancellationToken));

        return count > 0;
    }

    public async Task<PagedResultDto<CustomerBenefitListItemDto>> GetAvailableAsync(
        int page,
        int pageSize,
        string? search,
        CancellationToken cancellationToken = default)
    {
        var offset = (page - 1) * pageSize;

        const string sql = """
            SELECT
                COUNT(1)
            FROM dbo.benefits b
            INNER JOIN dbo.partners p ON p.id = b.partner_id
            WHERE b.status = 'active'
              AND b.direction = 'partner_to_matilha'
              AND b.target_actor_type = 'client'
              AND (
                    b.validity_type = 'continuous'
                    OR b.ends_at IS NULL
                    OR b.ends_at >= SYSUTCDATETIME()
                  )
              AND (
                    @Search IS NULL
                    OR b.title LIKE '%' + @Search + '%'
                    OR b.short_description LIKE '%' + @Search + '%'
                    OR p.trade_name LIKE '%' + @Search + '%'
                  );

            SELECT
                b.id AS Id,
                b.partner_id AS PartnerId,

                b.title AS Title,
                b.short_description AS ShortDescription,
                b.full_description AS FullDescription,

                p.trade_name AS PartnerName,
                p.segment AS PartnerSegment,
                p.category AS PartnerCategory,

                b.benefit_type AS BenefitType,
                b.eligibility_type AS EligibilityType,
                CASE
                    WHEN b.eligibility_type = 'open' THEN 'Disponível para clientes Matilha.'
                    WHEN b.eligibility_type = 'level' THEN 'Disponível conforme nível do cliente.'
                    WHEN b.eligibility_type = 'behavior' THEN 'Disponível conforme critérios comportamentais.'
                    WHEN b.eligibility_type = 'hybrid' THEN 'Disponível conforme critérios combinados.'
                    WHEN b.eligibility_type = 'code' THEN 'Disponível mediante código.'
                    ELSE b.eligibility_type
                END AS EligibilitySummary,

                b.usage_scope AS UsageScope,
                CASE
                    WHEN b.usage_scope = 'customer_pet' THEN 'Por cão'
                    ELSE 'Por cliente'
                END AS UsageScopeLabel,

                b.recurrence_type AS RecurrenceType,
                CASE
                    WHEN b.recurrence_type = 'once_per_customer' THEN '1x por cliente'
                    WHEN b.recurrence_type = 'first_use_only' THEN 'Somente primeira utilização'
                    WHEN b.recurrence_type = 'unlimited_within_rule' THEN 'Ilimitado dentro da regra'
                    WHEN b.recurrence_type = 'limited_per_period' THEN CONCAT(
                        COALESCE(CAST(b.recurrence_value AS varchar(10)), '1'),
                        'x por ',
                        CASE b.recurrence_period
                            WHEN 'day' THEN 'dia'
                            WHEN 'week' THEN 'semana'
                            WHEN 'month' THEN 'mês'
                            WHEN 'quarter' THEN 'trimestre'
                            WHEN 'semester' THEN 'semestre'
                            WHEN 'year' THEN 'ano'
                            ELSE COALESCE(b.recurrence_period, 'período')
                        END
                    )
                    ELSE b.recurrence_type
                END AS RecurrenceLabel,

                b.validity_type AS ValidityType,
                CASE
                    WHEN b.validity_type = 'continuous' THEN 'Contínuo'
                    WHEN b.ends_at IS NOT NULL THEN CONCAT('Até ', CONVERT(varchar(10), b.ends_at, 103))
                    WHEN b.starts_at IS NOT NULL THEN CONCAT('A partir de ', CONVERT(varchar(10), b.starts_at, 103))
                    ELSE b.validity_type
                END AS ValidityLabel,

                b.requires_partner_availability AS RequiresPartnerAvailability,
                b.requires_matilha_acceptance_rules AS RequiresMatilhaAcceptanceRules,
                b.requires_manual_release AS RequiresManualRelease,

                b.starts_at AS StartsAt,
                b.ends_at AS EndsAt
            FROM dbo.benefits b
            INNER JOIN dbo.partners p ON p.id = b.partner_id
            WHERE b.status = 'active'
              AND b.direction = 'partner_to_matilha'
              AND b.target_actor_type = 'client'
              AND (
                    b.validity_type = 'continuous'
                    OR b.ends_at IS NULL
                    OR b.ends_at >= SYSUTCDATETIME()
                  )
              AND (
                    @Search IS NULL
                    OR b.title LIKE '%' + @Search + '%'
                    OR b.short_description LIKE '%' + @Search + '%'
                    OR p.trade_name LIKE '%' + @Search + '%'
                  )
            ORDER BY b.highlight_in_showcase DESC, b.created_at DESC
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
        """;

        var parameters = new
        {
            Search = string.IsNullOrWhiteSpace(search) ? null : search.Trim(),
            Offset = offset,
            PageSize = pageSize
        };

        using var grid = await _connection.QueryMultipleAsync(
            new CommandDefinition(
                sql,
                parameters,
                commandType: CommandType.Text,
                cancellationToken: cancellationToken));

        var totalCount = await grid.ReadSingleAsync<int>();
        var items = (await grid.ReadAsync<CustomerBenefitListItemDto>()).ToList();

        return new PagedResultDto<CustomerBenefitListItemDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task<CustomerBenefitDetailDto?> GetByIdAsync(
        Guid benefitId,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT TOP (1)
                b.id AS Id,
                b.partner_id AS PartnerId,

                b.title AS Title,
                b.short_description AS ShortDescription,
                b.full_description AS FullDescription,

                p.trade_name AS PartnerName,
                p.segment AS PartnerSegment,
                p.category AS PartnerCategory,

                b.benefit_type AS BenefitType,
                b.eligibility_type AS EligibilityType,
                CASE
                    WHEN b.eligibility_type = 'open' THEN 'Disponível para clientes Matilha.'
                    WHEN b.eligibility_type = 'level' THEN 'Disponível conforme nível do cliente.'
                    WHEN b.eligibility_type = 'behavior' THEN 'Disponível conforme critérios comportamentais.'
                    WHEN b.eligibility_type = 'hybrid' THEN 'Disponível conforme critérios combinados.'
                    WHEN b.eligibility_type = 'code' THEN 'Disponível mediante código.'
                    ELSE b.eligibility_type
                END AS EligibilitySummary,

                b.usage_scope AS UsageScope,
                CASE
                    WHEN b.usage_scope = 'customer_pet' THEN 'Por cão'
                    ELSE 'Por cliente'
                END AS UsageScopeLabel,

                b.recurrence_type AS RecurrenceType,
                CASE
                    WHEN b.recurrence_type = 'once_per_customer' THEN '1x por cliente'
                    WHEN b.recurrence_type = 'first_use_only' THEN 'Somente primeira utilização'
                    WHEN b.recurrence_type = 'unlimited_within_rule' THEN 'Ilimitado dentro da regra'
                    WHEN b.recurrence_type = 'limited_per_period' THEN CONCAT(
                        COALESCE(CAST(b.recurrence_value AS varchar(10)), '1'),
                        'x por ',
                        CASE b.recurrence_period
                            WHEN 'day' THEN 'dia'
                            WHEN 'week' THEN 'semana'
                            WHEN 'month' THEN 'mês'
                            WHEN 'quarter' THEN 'trimestre'
                            WHEN 'semester' THEN 'semestre'
                            WHEN 'year' THEN 'ano'
                            ELSE COALESCE(b.recurrence_period, 'período')
                        END
                    )
                    ELSE b.recurrence_type
                END AS RecurrenceLabel,

                b.validity_type AS ValidityType,
                CASE
                    WHEN b.validity_type = 'continuous' THEN 'Contínuo'
                    WHEN b.ends_at IS NOT NULL THEN CONCAT('Até ', CONVERT(varchar(10), b.ends_at, 103))
                    WHEN b.starts_at IS NOT NULL THEN CONCAT('A partir de ', CONVERT(varchar(10), b.starts_at, 103))
                    ELSE b.validity_type
                END AS ValidityLabel,

                b.requires_partner_availability AS RequiresPartnerAvailability,
                b.requires_matilha_acceptance_rules AS RequiresMatilhaAcceptanceRules,
                b.requires_manual_release AS RequiresManualRelease,

                b.allow_first_use_only AS AllowFirstUseOnly,
                b.auto_activate_when_approved AS AutoActivateWhenApproved,
                b.stacking_rule AS StackingRule,

                b.starts_at AS StartsAt,
                b.ends_at AS EndsAt
            FROM dbo.benefits b
            INNER JOIN dbo.partners p ON p.id = b.partner_id
            WHERE b.id = @BenefitId
              AND b.status = 'active'
              AND b.direction = 'partner_to_matilha'
              AND b.target_actor_type = 'client'
              AND (
                    b.validity_type = 'continuous'
                    OR b.ends_at IS NULL
                    OR b.ends_at >= SYSUTCDATETIME()
                  );
        """;

        return await _connection.QueryFirstOrDefaultAsync<CustomerBenefitDetailDto>(
            new CommandDefinition(
                sql,
                new { BenefitId = benefitId },
                commandType: CommandType.Text,
                cancellationToken: cancellationToken));
    }

    public async Task<PagedResultDto<CustomerBenefitRequestListItemDto>> GetRequestsByClientAsync(
    Guid clientId,
    int page,
    int pageSize,
    string? status,
    CancellationToken cancellationToken = default)
    {
        page = page <= 0 ? 1 : page;
        pageSize = pageSize <= 0 ? 12 : pageSize;

        var offset = (page - 1) * pageSize;

        const string sql = """
                                SELECT
                                    COUNT(1)
                                FROM dbo.benefit_requests br
                                INNER JOIN dbo.benefits b ON b.id = br.benefit_id
                                INNER JOIN dbo.partners p ON p.id = br.partner_id
                                LEFT JOIN dbo.client_pets cp ON cp.id = br.requester_client_pet_id
                                WHERE br.requester_type = 'client'
                                  AND br.requester_client_id = @ClientId
                                  AND (
                                        @Status IS NULL
                                        OR br.request_status = @Status
                                        OR br.approval_status = @Status
                                      );

                                SELECT
                                    br.id AS Id,

                                    br.benefit_id AS BenefitId,
                                    b.title AS BenefitTitle,
                                    b.benefit_type AS BenefitType,

                                    br.partner_id AS PartnerId,
                                    p.trade_name AS PartnerName,

                                    br.request_status AS RequestStatus,
                                    br.review_required AS ReviewRequired,
                                    br.approval_status AS ApprovalStatus,
                                    br.approval_reason AS ApprovalReason,

                                    br.pet_source_type AS PetSourceType,
                                    br.requester_client_pet_id AS PetId,
                                    cp.name AS PetName,

                                    br.requested_at AS RequestedAt,
                                    br.scheduled_for AS ScheduledFor,
                                    br.expires_at AS ExpiresAt,
                                    br.updated_at AS UpdatedAt,

                                    bu.id AS UsageId,
                                    bu.usage_status AS UsageStatus,
                                    bu.used_at AS UsedAt
                                FROM dbo.benefit_requests br
                                INNER JOIN dbo.benefits b ON b.id = br.benefit_id
                                INNER JOIN dbo.partners p ON p.id = br.partner_id
                                LEFT JOIN dbo.client_pets cp ON cp.id = br.requester_client_pet_id
                                LEFT JOIN dbo.benefit_usages bu ON bu.benefit_request_id = br.id
                                WHERE br.requester_type = 'client'
                                  AND br.requester_client_id = @ClientId
                                  AND (
                                        @Status IS NULL
                                        OR br.request_status = @Status
                                        OR br.approval_status = @Status
                                      )
                                ORDER BY br.requested_at DESC
                                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
                            """;

        var parameters = new
        {
            ClientId = clientId,
            Status = string.IsNullOrWhiteSpace(status) ? null : status.Trim(),
            Offset = offset,
            PageSize = pageSize
        };

        using var grid = await _connection.QueryMultipleAsync(
            new CommandDefinition(
                sql,
                parameters,
                commandType: CommandType.Text,
                cancellationToken: cancellationToken));

        var totalCount = await grid.ReadSingleAsync<int>();
        var items = (await grid.ReadAsync<CustomerBenefitRequestListItemDto>()).ToList();

        return new PagedResultDto<CustomerBenefitRequestListItemDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task<PagedResultDto<CustomerBenefitUsageListItemDto>> GetUsagesByClientAsync(
        Guid clientId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        page = page <= 0 ? 1 : page;
        pageSize = pageSize <= 0 ? 12 : pageSize;

        var offset = (page - 1) * pageSize;

        const string sql = """
                                SELECT
                                    COUNT(1)
                                FROM dbo.benefit_usages bu
                                INNER JOIN dbo.benefits b ON b.id = bu.benefit_id
                                INNER JOIN dbo.partners p ON p.id = bu.partner_id
                                LEFT JOIN dbo.client_pets cp ON cp.id = bu.client_pet_id
                                WHERE bu.used_by_type = 'client'
                                  AND bu.used_by_client_id = @ClientId;

                                SELECT
                                    bu.id AS Id,

                                    bu.benefit_request_id AS BenefitRequestId,
                                    bu.benefit_id AS BenefitId,

                                    COALESCE(bu.snapshot_title, b.title) AS BenefitTitle,
                                    b.benefit_type AS BenefitType,

                                    bu.partner_id AS PartnerId,
                                    COALESCE(bu.snapshot_partner_name, p.trade_name) AS PartnerName,

                                    bu.usage_status AS UsageStatus,
                                    bu.used_at AS UsedAt,

                                    bu.client_pet_id AS PetId,
                                    cp.name AS PetName,

                                    bu.monetary_value AS MonetaryValue,
                                    bu.discount_value AS DiscountValue,

                                    bu.snapshot_rule_summary AS SnapshotRuleSummary,

                                    bu.created_at AS CreatedAt,
                                    bu.updated_at AS UpdatedAt
                                FROM dbo.benefit_usages bu
                                INNER JOIN dbo.benefits b ON b.id = bu.benefit_id
                                INNER JOIN dbo.partners p ON p.id = bu.partner_id
                                LEFT JOIN dbo.client_pets cp ON cp.id = bu.client_pet_id
                                WHERE bu.used_by_type = 'client'
                                  AND bu.used_by_client_id = @ClientId
                                ORDER BY COALESCE(bu.used_at, bu.created_at) DESC
                                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
                            """;

        var parameters = new
        {
            ClientId = clientId,
            Offset = offset,
            PageSize = pageSize
        };

        using var grid = await _connection.QueryMultipleAsync(
            new CommandDefinition(
                sql,
                parameters,
                commandType: CommandType.Text,
                cancellationToken: cancellationToken));

        var totalCount = await grid.ReadSingleAsync<int>();
        var items = (await grid.ReadAsync<CustomerBenefitUsageListItemDto>()).ToList();

        return new PagedResultDto<CustomerBenefitUsageListItemDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task<CustomerBenefitRequestDetailDto?> GetRequestByIdForClientAsync(
        Guid clientId,
        Guid requestId,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
                                SELECT TOP (1)
                                    br.id AS Id,

                                    br.benefit_id AS BenefitId,
                                    b.title AS BenefitTitle,
                                    b.benefit_type AS BenefitType,
                                    COALESCE(b.full_description, b.short_description) AS BenefitDescription,

                                    br.partner_id AS PartnerId,
                                    p.trade_name AS PartnerName,
                                    p.segment AS PartnerSegment,
                                    p.category AS PartnerCategory,

                                    br.request_status AS RequestStatus,
                                    br.review_required AS ReviewRequired,
                                    br.approval_status AS ApprovalStatus,
                                    br.approval_reason AS ApprovalReason,

                                    br.pet_source_type AS PetSourceType,
                                    br.requester_client_pet_id AS PetId,
                                    cp.name AS PetName,

                                    br.requested_at AS RequestedAt,
                                    br.scheduled_for AS ScheduledFor,
                                    br.expires_at AS ExpiresAt,
                                    br.updated_at AS UpdatedAt,

                                    bu.id AS UsageId,
                                    bu.usage_status AS UsageStatus,
                                    bu.used_at AS UsedAt,

                                    br.review_notes AS ReviewNotes,
                                    br.reviewed_at AS ReviewedAt,

                                    latest_review.review_status AS LatestReviewStatus,
                                    latest_review.review_point AS LatestReviewPoint,
                                    latest_review.review_recommendation AS LatestReviewRecommendation,
                                    COALESCE(latest_review.reviewed_at, latest_review.created_at) AS LatestReviewedAt,

                                    b.eligibility_type AS EligibilityType,
                                    CASE
                                        WHEN b.eligibility_type = 'open' THEN 'Disponível para clientes Matilha.'
                                        WHEN b.eligibility_type = 'level' THEN 'Disponível conforme nível do cliente.'
                                        WHEN b.eligibility_type = 'behavior' THEN 'Disponível conforme critérios comportamentais.'
                                        WHEN b.eligibility_type = 'hybrid' THEN 'Disponível conforme critérios combinados.'
                                        WHEN b.eligibility_type = 'code' THEN 'Disponível mediante código.'
                                        ELSE b.eligibility_type
                                    END AS EligibilitySummary,

                                    b.usage_scope AS UsageScope,
                                    CASE
                                        WHEN b.usage_scope = 'customer_pet' THEN 'Por cão'
                                        ELSE 'Por cliente'
                                    END AS UsageScopeLabel,

                                    b.recurrence_type AS RecurrenceType,
                                    CASE
                                        WHEN b.recurrence_type = 'once_per_customer' THEN '1x por cliente'
                                        WHEN b.recurrence_type = 'first_use_only' THEN 'Somente primeira utilização'
                                        WHEN b.recurrence_type = 'unlimited_within_rule' THEN 'Ilimitado dentro da regra'
                                        WHEN b.recurrence_type = 'limited_per_period' THEN CONCAT(
                                            COALESCE(CAST(b.recurrence_value AS varchar(10)), '1'),
                                            'x por ',
                                            CASE b.recurrence_period
                                                WHEN 'day' THEN 'dia'
                                                WHEN 'week' THEN 'semana'
                                                WHEN 'month' THEN 'mês'
                                                WHEN 'quarter' THEN 'trimestre'
                                                WHEN 'semester' THEN 'semestre'
                                                WHEN 'year' THEN 'ano'
                                                ELSE COALESCE(b.recurrence_period, 'período')
                                            END
                                        )
                                        ELSE b.recurrence_type
                                    END AS RecurrenceLabel,

                                    b.validity_type AS ValidityType,
                                    CASE
                                        WHEN b.validity_type = 'continuous' THEN 'Contínuo'
                                        WHEN b.ends_at IS NOT NULL THEN CONCAT('Até ', CONVERT(varchar(10), b.ends_at, 103))
                                        WHEN b.starts_at IS NOT NULL THEN CONCAT('A partir de ', CONVERT(varchar(10), b.starts_at, 103))
                                        ELSE b.validity_type
                                    END AS ValidityLabel
                                FROM dbo.benefit_requests br
                                INNER JOIN dbo.benefits b ON b.id = br.benefit_id
                                INNER JOIN dbo.partners p ON p.id = br.partner_id
                                LEFT JOIN dbo.client_pets cp ON cp.id = br.requester_client_pet_id
                                LEFT JOIN dbo.benefit_usages bu ON bu.benefit_request_id = br.id
                                OUTER APPLY
                                (
                                    SELECT TOP (1)
                                        r.review_status,
                                        r.review_point,
                                        r.review_recommendation,
                                        r.reviewed_at,
                                        r.created_at
                                    FROM dbo.benefit_request_reviews r
                                    WHERE r.benefit_request_id = br.id
                                    ORDER BY r.created_at DESC
                                ) latest_review
                                WHERE br.id = @RequestId
                                  AND br.requester_type = 'client'
                                  AND br.requester_client_id = @ClientId;
                            """;

        return await _connection.QueryFirstOrDefaultAsync<CustomerBenefitRequestDetailDto>(
            new CommandDefinition(
                sql,
                new
                {
                    ClientId = clientId,
                    RequestId = requestId
                },
                commandType: CommandType.Text,
                cancellationToken: cancellationToken));
    }

    public async Task<CustomerBenefitUsageDetailDto?> GetUsageByIdForClientAsync(
        Guid clientId,
        Guid usageId,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
                                SELECT TOP (1)
                                    bu.id AS Id,

                                    bu.benefit_request_id AS BenefitRequestId,
                                    bu.benefit_id AS BenefitId,

                                    COALESCE(bu.snapshot_title, b.title) AS BenefitTitle,
                                    b.benefit_type AS BenefitType,
                                    COALESCE(b.full_description, b.short_description) AS BenefitDescription,

                                    bu.partner_id AS PartnerId,
                                    COALESCE(bu.snapshot_partner_name, p.trade_name) AS PartnerName,
                                    p.segment AS PartnerSegment,
                                    p.category AS PartnerCategory,

                                    bu.usage_status AS UsageStatus,
                                    bu.used_at AS UsedAt,

                                    bu.client_pet_id AS PetId,
                                    cp.name AS PetName,

                                    bu.monetary_value AS MonetaryValue,
                                    bu.discount_value AS DiscountValue,

                                    bu.snapshot_rule_summary AS SnapshotRuleSummary,

                                    bu.created_at AS CreatedAt,
                                    bu.updated_at AS UpdatedAt,

                                    b.usage_scope AS UsageScope,
                                    CASE
                                        WHEN b.usage_scope = 'customer_pet' THEN 'Por cão'
                                        ELSE 'Por cliente'
                                    END AS UsageScopeLabel,

                                    br.request_status AS RequestStatus,
                                    br.requested_at AS RequestedAt,

                                    partner_user.name AS ConfirmedByPartnerName,
                                    admin_user.name AS ConfirmedByAdminName
                                FROM dbo.benefit_usages bu
                                INNER JOIN dbo.benefits b ON b.id = bu.benefit_id
                                INNER JOIN dbo.partners p ON p.id = bu.partner_id
                                LEFT JOIN dbo.benefit_requests br ON br.id = bu.benefit_request_id
                                LEFT JOIN dbo.client_pets cp ON cp.id = bu.client_pet_id
                                LEFT JOIN dbo.users partner_user ON partner_user.id = bu.confirmed_by_partner_user_id
                                LEFT JOIN dbo.users admin_user ON admin_user.id = bu.confirmed_by_admin_user_id
                                WHERE bu.id = @UsageId
                                  AND bu.used_by_type = 'client'
                                  AND bu.used_by_client_id = @ClientId;
                            """;

        return await _connection.QueryFirstOrDefaultAsync<CustomerBenefitUsageDetailDto>(
            new CommandDefinition(
                sql,
                new
                {
                    ClientId = clientId,
                    UsageId = usageId
                },
                commandType: CommandType.Text,
                cancellationToken: cancellationToken));
    }
}