using System.Data;
using Dapper;
using ClubeBeneficios.Benefits.Domain.Dtos.PublicCatalog;
using ClubeBeneficios.Benefits.Domain.Repositories;

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
}