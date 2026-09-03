using System.Data;
using Dapper;
using ClubeBeneficios.Benefits.Domain.Dtos.PublicCatalog;
using ClubeBeneficios.Benefits.Domain.Repositories;
using System.Security.Cryptography;
using System.Text;

namespace ClubeBeneficios.Benefits.Infrastructure.Repositories;

public class PublicPartnerCatalogRepository : IPublicPartnerCatalogRepository
{
    private readonly IDbConnection _connection;

    public PublicPartnerCatalogRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<PublicPartnerCatalogDto?> GetBySlugAsync(
        string slug,
        CancellationToken cancellationToken = default)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@Slug", slug);

        var command = new CommandDefinition(
            "dbo.usp_public_partner_catalog_get_by_slug",
            parameters,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        using var multi = await _connection.QueryMultipleAsync(command);

        var catalog = await multi.ReadFirstOrDefaultAsync<PublicPartnerCatalogLinkDto>();

        if (catalog is null || !catalog.Found)
            return null;

        var benefits = (await multi.ReadAsync<PublicPartnerCatalogBenefitDto>()).ToArray();

        return new PublicPartnerCatalogDto
        {
            Catalog = catalog,
            Benefits = benefits
        };
    }

    public async Task<PublicPartnerBenefitRequestCreatedDto> CreateRequestAsync(
        string slug,
        Guid benefitId,
        CreatePublicPartnerBenefitRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var parameters = new DynamicParameters();

        parameters.Add("@Slug", slug);
        parameters.Add("@BenefitId", benefitId);

        parameters.Add("@CustomerName", Normalize(request.CustomerName));
        parameters.Add("@CustomerEmail", Normalize(request.CustomerEmail));
        parameters.Add("@CustomerPhone", OnlyDigits(request.CustomerPhone));
        parameters.Add("@CustomerDocument", OnlyDigits(request.CustomerDocument));

        parameters.Add("@PetName", Normalize(request.PetName));
        parameters.Add("@PetBreed", Normalize(request.PetBreed));
        parameters.Add("@PetSex", Normalize(request.PetSex));
        parameters.Add("@PetAgeMonths", request.PetAgeMonths);
        parameters.Add("@PetSize", Normalize(request.PetSize));
        parameters.Add("@PetIsNeutered", request.PetIsNeutered);

        parameters.Add("@AcceptedTerms", request.AcceptedTerms);
        parameters.Add("@AcceptedPrivacyPolicy", request.AcceptedPrivacyPolicy);
        parameters.Add("@CustomerNotes", Normalize(request.CustomerNotes));

        var command = new CommandDefinition(
            "dbo.usp_public_partner_benefit_request_create",
            parameters,
            commandType: CommandType.StoredProcedure,
            commandTimeout: 60);

        return await _connection.QuerySingleAsync<PublicPartnerBenefitRequestCreatedDto>(command);
    }

    private static string? Normalize(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    private static string? OnlyDigits(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        var digits = new string(value.Where(char.IsDigit).ToArray());

        return string.IsNullOrWhiteSpace(digits) ? null : digits;
    }

    public async Task<Guid> InsertPartnerCustomerDocumentAsync(
        Guid partnerCustomerId,
        Guid? partnerCustomerPetId,
        string documentType,
        string fileUrl,
        string? fileName,
        string? mimeType,
        CancellationToken cancellationToken = default)
    {
        var documentId = Guid.NewGuid();

        const string sql = @"
        INSERT INTO dbo.partner_customer_documents
        (
            id,
            partner_customer_id,
            partner_customer_pet_id,
            document_type,
            file_url,
            file_name,
            mime_type,
            status,
            expires_at,
            verified_at,
            verified_by_user_id,
            rejection_reason,
            created_at,
            updated_at
        )
        VALUES
        (
            @Id,
            @PartnerCustomerId,
            @PartnerCustomerPetId,
            @DocumentType,
            @FileUrl,
            @FileName,
            @MimeType,
            'pending',
            NULL,
            NULL,
            NULL,
            NULL,
            SYSUTCDATETIME(),
            SYSUTCDATETIME()
        );";

        var parameters = new DynamicParameters();
        parameters.Add("@Id", documentId);
        parameters.Add("@PartnerCustomerId", partnerCustomerId);
        parameters.Add("@PartnerCustomerPetId", partnerCustomerPetId);
        parameters.Add("@DocumentType", documentType);
        parameters.Add("@FileUrl", fileUrl);
        parameters.Add("@FileName", fileName);
        parameters.Add("@MimeType", mimeType);

        var command = new CommandDefinition(
            sql,
            parameters);

        await _connection.ExecuteAsync(command);

        return documentId;
    }

    public async Task LinkVaccinationCardToBenefitRequestAsync(
    Guid benefitRequestId,
    Guid partnerCustomerDocumentId,
    string fileUrl,
    string? fileName,
    CancellationToken cancellationToken = default)
    {
        const string sql = @"
        IF EXISTS
        (
            SELECT 1
            FROM dbo.benefit_request_documents
            WHERE benefit_request_id = @BenefitRequestId
              AND document_type = 'vaccination_card'
        )
        BEGIN
            UPDATE dbo.benefit_request_documents
            SET
                source_type = 'uploaded',
                client_document_id = NULL,
                partner_customer_document_id = NULL,
                file_url = @FileUrl,
                file_name = @FileName,
                submission_status = 'submitted',
                notes = 'Carteirinha enviada pelo formulário público.',
                updated_at = SYSUTCDATETIME()
            WHERE benefit_request_id = @BenefitRequestId
              AND document_type = 'vaccination_card';
        END
        ELSE
        BEGIN
            INSERT INTO dbo.benefit_request_documents
            (
                benefit_request_id,
                document_type,
                source_type,
                client_document_id,
                partner_customer_document_id,
                file_url,
                file_name,
                submission_status,
                notes,
                created_at,
                updated_at
            )
            VALUES
            (
                @BenefitRequestId,
                'vaccination_card',
                'uploaded',
                NULL,
                NULL,
                @FileUrl,
                @FileName,
                'submitted',
                'Carteirinha enviada pelo formulário público.',
                SYSUTCDATETIME(),
                SYSUTCDATETIME()
            );
        END";

        var parameters = new DynamicParameters();
        parameters.Add("@BenefitRequestId", benefitRequestId);
        parameters.Add("@FileUrl", fileUrl);
        parameters.Add("@FileName", fileName);

        var command = new CommandDefinition(
            sql,
            parameters);

        await _connection.ExecuteAsync(command);
    }

    public async Task EnqueueRequestNotificationAsync(
    Guid requestId,
    string eventType,
    string? reason = null,
    CancellationToken cancellationToken = default)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@BenefitRequestId", requestId);
        parameters.Add("@EventType", eventType);
        parameters.Add("@ReviewPoint", null);
        parameters.Add("@ReviewRecommendation", Normalize(reason));
        parameters.Add("@EventReferenceId", requestId);

        var command = new CommandDefinition(
            "dbo.usp_benefit_request_notification_enqueue",
            parameters,
            commandType: CommandType.StoredProcedure,
            commandTimeout: 60);

        await _connection.ExecuteAsync(command);
    }

    public async Task UpsertBenefitRequestPreventiveAsync(
    Guid benefitRequestId,
    string preventiveType,
    string? brandName,
    DateTime? appliedAt,
    DateTime? expiresAt,
    CancellationToken cancellationToken = default)
    {
        const string sql = @"
        IF EXISTS
        (
            SELECT 1
            FROM dbo.benefit_request_preventives
            WHERE benefit_request_id = @BenefitRequestId
              AND preventive_type = @PreventiveType
        )
        BEGIN
            UPDATE dbo.benefit_request_preventives
            SET
                source_type = 'uploaded',
                client_pet_health_record_id = NULL,
                partner_customer_pet_health_record_id = NULL,
                application_type = 'unknown',
                brand_name = @BrandName,
                applied_at = @AppliedAt,
                expires_at = @ExpiresAt,
                submission_status = 'submitted',
                notes = 'Informado pelo formulário público.',
                updated_at = SYSUTCDATETIME()
            WHERE benefit_request_id = @BenefitRequestId
              AND preventive_type = @PreventiveType;
        END
        ELSE
        BEGIN
            INSERT INTO dbo.benefit_request_preventives
            (
                benefit_request_id,
                preventive_type,
                source_type,
                client_pet_health_record_id,
                partner_customer_pet_health_record_id,
                application_type,
                brand_name,
                applied_at,
                expires_at,
                submission_status,
                notes,
                created_at,
                updated_at
            )
            VALUES
            (
                @BenefitRequestId,
                @PreventiveType,
                'uploaded',
                NULL,
                NULL,
                'unknown',
                @BrandName,
                @AppliedAt,
                @ExpiresAt,
                'submitted',
                'Informado pelo formulário público.',
                SYSUTCDATETIME(),
                SYSUTCDATETIME()
            );
        END";

        var parameters = new DynamicParameters();
        parameters.Add("@BenefitRequestId", benefitRequestId);
        parameters.Add("@PreventiveType", preventiveType);
        parameters.Add("@BrandName", Normalize(brandName));
        parameters.Add("@AppliedAt", appliedAt);
        parameters.Add("@ExpiresAt", expiresAt);

        var command = new CommandDefinition(
            sql,
            parameters);

        await _connection.ExecuteAsync(command);
    }

    public async Task<PublicBenefitRequestCorrectionDto?> GetCorrectionByTokenAsync(
    string token,
    CancellationToken cancellationToken = default)
    {
        var tokenHash = HashToken(token);

        const string sql = @"
        SELECT TOP 1
            r.id AS request_id,
            r.benefit_id,
            r.partner_id,
            r.requester_partner_customer_id AS partner_customer_id,
            r.requester_partner_customer_pet_id AS partner_customer_pet_id,

            b.title AS benefit_title,
            p.trade_name AS partner_name,

            pc.full_name AS requester_name,
            pc.email AS requester_email,
            pc.phone AS requester_phone,

            pcp.name AS pet_name,
            pcp.breed AS pet_breed,
            pcp.sex AS pet_sex,
            pcp.age_months AS pet_age_months,
            pcp.size AS pet_size,
            pcp.is_neutered AS pet_is_neutered,

            r.request_status,
            CASE
                WHEN r.request_status = 'under_review' THEN 'Ajuste solicitado'
                WHEN r.request_status = 'pending_review' THEN 'Pendente de análise'
                WHEN r.request_status = 'approved' THEN 'Aprovada'
                WHEN r.request_status IN ('declined', 'rejected') THEN 'Reprovada'
                ELSE r.request_status
            END AS request_status_label,

            latest_review.review_point,
            latest_review.review_recommendation,

            COALESCE(brd.file_url, pcd.file_url) AS vaccination_card_file_url,
            COALESCE(brd.file_name, pcd.file_name) AS vaccination_card_file_name,

            dw.brand_name AS dewormer_brand_name,
            dw.applied_at AS dewormer_applied_at,
            dw.expires_at AS dewormer_expires_at,

            ft.brand_name AS flea_tick_brand_name,
            ft.applied_at AS flea_tick_applied_at,
            ft.expires_at AS flea_tick_expires_at
        FROM dbo.benefit_request_correction_tokens t
        INNER JOIN dbo.benefit_requests r
            ON r.id = t.benefit_request_id
        INNER JOIN dbo.benefits b
            ON b.id = r.benefit_id
        INNER JOIN dbo.partners p
            ON p.id = r.partner_id
        INNER JOIN dbo.partner_customers pc
            ON pc.id = r.requester_partner_customer_id
        LEFT JOIN dbo.partner_customer_pets pcp
            ON pcp.id = r.requester_partner_customer_pet_id

        OUTER APPLY
        (
            SELECT TOP 1
                rr.review_point,
                rr.review_recommendation
            FROM dbo.benefit_request_reviews rr
            WHERE rr.benefit_request_id = r.id
            ORDER BY rr.reviewed_at DESC, rr.created_at DESC
        ) latest_review

        LEFT JOIN dbo.benefit_request_documents brd
            ON brd.benefit_request_id = r.id
           AND brd.document_type = 'vaccination_card'

        LEFT JOIN dbo.partner_customer_documents pcd
            ON pcd.id = brd.partner_customer_document_id

        LEFT JOIN dbo.benefit_request_preventives dw
            ON dw.benefit_request_id = r.id
           AND dw.preventive_type = 'dewormer'

        LEFT JOIN dbo.benefit_request_preventives ft
            ON ft.benefit_request_id = r.id
           AND ft.preventive_type = 'flea_tick'

        WHERE t.token_hash = @TokenHash
          AND t.status = 'active'
          AND t.expires_at > SYSUTCDATETIME()
          AND r.request_status = 'under_review';";

        var parameters = new DynamicParameters();
        parameters.Add("@TokenHash", tokenHash);

        var command = new CommandDefinition(
            sql,
            parameters,
            cancellationToken: cancellationToken);

        return await _connection.QueryFirstOrDefaultAsync<PublicBenefitRequestCorrectionDto>(command);
    }

    public async Task<PublicBenefitRequestCorrectionSubmittedDto> SubmitCorrectionAsync(
        string token,
        string? customerNotes,
        CancellationToken cancellationToken = default)
    {
        var tokenHash = HashToken(token);

        const string sql = @"
        DECLARE
            @TokenId UNIQUEIDENTIFIER,
            @BenefitRequestId UNIQUEIDENTIFIER;

        SELECT TOP 1
            @TokenId = t.id,
            @BenefitRequestId = t.benefit_request_id
        FROM dbo.benefit_request_correction_tokens t
        INNER JOIN dbo.benefit_requests r
            ON r.id = t.benefit_request_id
        WHERE t.token_hash = @TokenHash
          AND t.status = 'active'
          AND t.expires_at > SYSUTCDATETIME()
          AND r.request_status = 'under_review';

        IF @TokenId IS NULL OR @BenefitRequestId IS NULL
        BEGIN
            THROW 51000, 'Link de ajuste inválido, expirado ou já utilizado.', 1;
        END;

        UPDATE dbo.benefit_requests
        SET
            request_status = 'pending_review',
            approval_status = 'pending_review',
            approval_requested_at = SYSUTCDATETIME(),
            review_required = 1,
            review_notes = @CustomerNotes,
            updated_at = SYSUTCDATETIME()
        WHERE id = @BenefitRequestId;

        UPDATE dbo.benefit_request_correction_tokens
        SET
            status = 'used',
            used_at = SYSUTCDATETIME(),
            updated_at = SYSUTCDATETIME()
        WHERE id = @TokenId;

        INSERT INTO dbo.benefit_request_timeline_events
        (
            id,
            benefit_request_id,
            event_type,
            event_status,
            event_point,
            event_description,
            actor_user_id,
            occurred_at,
            created_at
        )
        VALUES
        (
            NEWID(),
            @BenefitRequestId,
            'health_submitted',
            'pending_review',
            'health_documents',
            COALESCE(NULLIF(LTRIM(RTRIM(@CustomerNotes)), ''), 'Solicitante reenviou os ajustes solicitados.'),
            NULL,
            SYSUTCDATETIME(),
            SYSUTCDATETIME()
        );

        EXEC dbo.usp_benefit_request_notification_enqueue
            @BenefitRequestId = @BenefitRequestId,
            @EventType = 'benefits.request.submitted.admin',
            @ReviewPoint = 'health_documents',
            @ReviewRecommendation = 'Solicitante reenviou os ajustes solicitados.',
            @EventReferenceId = @TokenId;

        SELECT
            @BenefitRequestId AS request_id,
            'pending_review' AS request_status,
            'pending_review' AS approval_status,
            'Ajustes reenviados com sucesso. A solicitação voltou para análise da Matilha.' AS message;";

        var parameters = new DynamicParameters();
        parameters.Add("@TokenHash", tokenHash);
        parameters.Add("@CustomerNotes", Normalize(customerNotes));

        var command = new CommandDefinition(
            sql,
            parameters,
            commandTimeout: 60,
            cancellationToken: cancellationToken);

        return await _connection.QuerySingleAsync<PublicBenefitRequestCorrectionSubmittedDto>(command);
    }

    private static string HashToken(string token)
    {
        var normalizedToken = string.IsNullOrWhiteSpace(token)
            ? string.Empty
            : token.Trim();

        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(normalizedToken));

        return Convert.ToHexString(bytes).ToLowerInvariant();
    }
}