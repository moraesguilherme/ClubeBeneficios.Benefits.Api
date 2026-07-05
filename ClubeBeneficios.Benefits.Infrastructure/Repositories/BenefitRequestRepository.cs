using System.Data;
using Dapper;
using ClubeBeneficios.Benefits.Domain.Dtos;
using ClubeBeneficios.Benefits.Domain.Dtos.Requests;
using ClubeBeneficios.Benefits.Domain.Repositories;

namespace ClubeBeneficios.Benefits.Infrastructure.Repositories;

public class BenefitRequestRepository : IBenefitRequestRepository
{
    private readonly IDbConnection _connection;

    public BenefitRequestRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<Guid> CreateAsync(
    CreateBenefitRequestDto request,
    Guid? performedByUserId,
    CancellationToken cancellationToken = default)
    {
        var parameters = new DynamicParameters();

        parameters.Add("@BenefitId", request.BenefitId);
        parameters.Add("@RequesterType", request.RequesterType);
        parameters.Add("@RequesterUserId", request.RequesterUserId);
        parameters.Add("@RequesterClientId", request.RequesterClientId);
        parameters.Add("@RequesterPartnerCustomerId", request.RequesterPartnerCustomerId);
        parameters.Add("@RequestedByUserId", request.RequestedByUserId ?? performedByUserId);
        parameters.Add("@AccessCodeId", request.AccessCodeId);
        parameters.Add("@PetSourceType", request.PetSourceType);
        parameters.Add("@RequesterClientPetId", request.RequesterClientPetId);
        parameters.Add("@RequesterPartnerCustomerPetId", request.RequesterPartnerCustomerPetId);
        parameters.Add("@ScheduledFor", request.ScheduledFor);
        parameters.Add("@ExpiresAt", request.ExpiresAt);
        parameters.Add("@ReviewRequired", request.ReviewRequired);

        var createdId = await _connection.QuerySingleAsync<Guid>(
            new CommandDefinition(
                "dbo.usp_benefit_requests_create",
                parameters,
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken));

        return createdId;
    }

    public async Task SubmitHealthAsync(
        Guid requestId,
        SubmitBenefitRequestHealthRequest request,
        CancellationToken cancellationToken = default)
    {
        var parameters = new DynamicParameters();

        parameters.Add("@BenefitRequestId", requestId);

        parameters.Add("@IncludeVaccinationCard", request.VaccinationCard is not null);
        parameters.Add("@VaccinationCardSourceType", request.VaccinationCard?.SourceType ?? "uploaded");
        parameters.Add("@VaccinationCardClientDocumentId", request.VaccinationCard?.ClientDocumentId);
        parameters.Add("@VaccinationCardPartnerCustomerDocumentId", request.VaccinationCard?.PartnerCustomerDocumentId);
        parameters.Add("@VaccinationCardFileUrl", request.VaccinationCard?.FileUrl);
        parameters.Add("@VaccinationCardFileName", request.VaccinationCard?.FileName);
        parameters.Add("@VaccinationCardSubmissionStatus", request.VaccinationCard?.SubmissionStatus ?? "submitted");
        parameters.Add("@VaccinationCardNotes", request.VaccinationCard?.Notes);

        parameters.Add("@IncludeDewormer", request.Dewormer is not null);
        parameters.Add("@DewormerSourceType", request.Dewormer?.SourceType ?? "uploaded");
        parameters.Add("@DewormerClientPetHealthRecordId", request.Dewormer?.ClientPetHealthRecordId);
        parameters.Add("@DewormerPartnerCustomerPetHealthRecordId", request.Dewormer?.PartnerCustomerPetHealthRecordId);
        parameters.Add("@DewormerApplicationType", request.Dewormer?.ApplicationType);
        parameters.Add("@DewormerBrandName", request.Dewormer?.BrandName);
        parameters.Add("@DewormerAppliedAt", request.Dewormer?.AppliedAt);
        parameters.Add("@DewormerExpiresAt", request.Dewormer?.ExpiresAt);
        parameters.Add("@DewormerSubmissionStatus", request.Dewormer?.SubmissionStatus ?? "submitted");
        parameters.Add("@DewormerNotes", request.Dewormer?.Notes);

        parameters.Add("@IncludeFleaTick", request.FleaTick is not null);
        parameters.Add("@FleaTickSourceType", request.FleaTick?.SourceType ?? "uploaded");
        parameters.Add("@FleaTickClientPetHealthRecordId", request.FleaTick?.ClientPetHealthRecordId);
        parameters.Add("@FleaTickPartnerCustomerPetHealthRecordId", request.FleaTick?.PartnerCustomerPetHealthRecordId);
        parameters.Add("@FleaTickApplicationType", request.FleaTick?.ApplicationType);
        parameters.Add("@FleaTickBrandName", request.FleaTick?.BrandName);
        parameters.Add("@FleaTickAppliedAt", request.FleaTick?.AppliedAt);
        parameters.Add("@FleaTickExpiresAt", request.FleaTick?.ExpiresAt);
        parameters.Add("@FleaTickSubmissionStatus", request.FleaTick?.SubmissionStatus ?? "submitted");
        parameters.Add("@FleaTickNotes", request.FleaTick?.Notes);

        await _connection.ExecuteAsync(
            new CommandDefinition(
                "dbo.usp_benefit_request_health_submit",
                parameters,
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken));
    }

    public async Task AddReviewAsync(
        Guid requestId,
        AddBenefitRequestReviewRequest request,
        Guid? performedByUserId,
        CancellationToken cancellationToken = default)
    {
        var reviewedByUserId = request.ReviewedByUserId ?? performedByUserId;

        if (reviewedByUserId is null)
        {
            throw new InvalidOperationException("Não foi possível identificar o usuário responsável pela análise.");
        }

        if (string.IsNullOrWhiteSpace(request.ReviewStatus))
        {
            throw new ArgumentException("ReviewStatus é obrigatório.", nameof(request));
        }

        var parameters = new DynamicParameters();

        parameters.Add("@BenefitRequestId", requestId);
        parameters.Add("@ReviewStatus", request.ReviewStatus);
        parameters.Add("@ReviewPoint", request.ReviewPoint);
        parameters.Add("@ReviewRecommendation", request.ReviewRecommendation);
        parameters.Add("@ReviewedByUserId", reviewedByUserId);

        await _connection.ExecuteAsync(
            new CommandDefinition(
                "dbo.usp_benefit_request_add_review",
                parameters,
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken));
    }

    public async Task ChangeStatusAsync(
        Guid requestId,
        ChangeBenefitRequestStatusRequest request,
        Guid? performedByUserId,
        CancellationToken cancellationToken = default)
    {
        var parameters = new DynamicParameters();

        parameters.Add("@BenefitRequestId", requestId);
        parameters.Add("@NewStatus", request.RequestStatus);
        parameters.Add("@ReviewNotes", request.ReviewNotes);
        parameters.Add("@ReviewedByUserId", performedByUserId);
        parameters.Add("@ApprovalStatus", null);
        parameters.Add("@ApprovalReason", request.ReviewNotes);

        await _connection.ExecuteAsync(
            new CommandDefinition(
                "dbo.usp_benefit_requests_change_status",
                parameters,
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken));
    }

    public async Task<BenefitRequestDetailDto?> GetByIdAsync(
    Guid requestId,
    CancellationToken cancellationToken = default)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@BenefitRequestId", requestId);

        using var grid = await _connection.QueryMultipleAsync(
            new CommandDefinition(
                "dbo.usp_benefit_request_get_by_id",
                parameters,
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken));

        var row = await grid.ReadFirstOrDefaultAsync<BenefitRequestDetailRow>();

        if (row is null)
        {
            return null;
        }

        var reviews = (await grid.ReadAsync<BenefitRequestReviewDto>()).ToList();
        var timelineEvents = (await grid.ReadAsync<BenefitRequestTimelineEventDto>()).ToList();

        var detail = row.ToDetailDto();
        detail.Reviews = reviews;
        detail.TimelineEvents = timelineEvents;

        return detail;
    }

    public async Task<PagedResultDto<BenefitRequestListItemDto>> SearchAsync(
    BenefitRequestFilterDto filter,
    CancellationToken cancellationToken = default)
    {
        var page = filter.Page <= 0 ? 1 : filter.Page;
        var pageSize = filter.PageSize <= 0 ? 20 : filter.PageSize;

        var parameters = new DynamicParameters();

        parameters.Add("@Search", string.IsNullOrWhiteSpace(filter.Search) ? null : filter.Search);
        parameters.Add("@BenefitId", filter.BenefitId);
        parameters.Add("@PartnerId", filter.PartnerId);
        parameters.Add("@RequestStatus", filter.RequestStatus);
        parameters.Add("@RequesterType", filter.RequesterType);
        parameters.Add("@RequestedFrom", filter.RequestedFrom);
        parameters.Add("@RequestedTo", filter.RequestedTo);
        parameters.Add("@Page", page);
        parameters.Add("@PageSize", pageSize);

        var items = (await _connection.QueryAsync<BenefitRequestListItemDto>(
            new CommandDefinition(
                "dbo.usp_benefit_requests_admin_search",
                parameters,
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken))).ToList();

        var totalCount = items.FirstOrDefault()?.TotalCount ?? 0;

        return new PagedResultDto<BenefitRequestListItemDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task<PagedResultDto<BenefitRequestListItemDto>> SearchPendingReviewAsync(
        BenefitRequestFilterDto filter,
        CancellationToken cancellationToken = default)
    {
        var page = filter.Page <= 0 ? 1 : filter.Page;
        var pageSize = filter.PageSize <= 0 ? 20 : filter.PageSize;
        var offset = (page - 1) * pageSize;

        const string itemsSql = @"
            select
                v.*,
                count(1) over() as total_count,
                row_number() over(order by v.requested_at desc, v.created_at desc) as rn
            from dbo.vw_benefit_request_approval_queue v
            where
                (@search is null
                    or v.benefit_title like '%' + @search + '%'
                    or v.partner_name like '%' + @search + '%'
                    or v.requester_name like '%' + @search + '%'
                    or v.pet_name like '%' + @search + '%')
                and (@benefit_id is null or v.benefit_id = @benefit_id)
                and (@partner_id is null or v.partner_id = @partner_id)
                and (@request_status is null or v.request_status = @request_status)
                and (@requester_type is null or v.requester_type = @requester_type)
                and (@requested_from is null or v.requested_at >= @requested_from)
                and (@requested_to is null or v.requested_at < dateadd(day, 1, @requested_to))
            order by v.requested_at desc, v.created_at desc
            offset @offset rows fetch next @page_size rows only;
        ";

        var parameters = new
        {
            search = string.IsNullOrWhiteSpace(filter.Search) ? null : filter.Search,
            benefit_id = filter.BenefitId,
            partner_id = filter.PartnerId,
            request_status = filter.RequestStatus,
            requester_type = filter.RequesterType,
            requested_from = filter.RequestedFrom,
            requested_to = filter.RequestedTo,
            offset,
            page_size = pageSize
        };

        var items = (await _connection.QueryAsync<BenefitRequestListItemDto>(
            new CommandDefinition(
                itemsSql,
                parameters,
                commandType: CommandType.Text,
                cancellationToken: cancellationToken))).ToList();

        var totalCount = items.FirstOrDefault()?.TotalCount ?? 0;

        return new PagedResultDto<BenefitRequestListItemDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task<BenefitRequestApprovalSummaryDto> GetApprovalSummaryAsync(
        CancellationToken cancellationToken = default)
    {
        const string sql = @"
            select
                count(1) as TotalQueueItems,
                sum(case when approval_status = 'pending_review' then 1 else 0 end) as PendingReviewCount,
                sum(case when approval_status = 'under_review' then 1 else 0 end) as UnderReviewCount,
                sum(case when approval_status = 'approved' then 1 else 0 end) as ApprovedCount,
                sum(case when approval_status = 'rejected' then 1 else 0 end) as RejectedCount,
                sum(case when approval_status = 'expired' then 1 else 0 end) as ExpiredCount,
                max(requested_at) as LatestRequestedAt
            from dbo.vw_benefit_request_approval_queue;
        ";

        var summary = await _connection.QueryFirstOrDefaultAsync<BenefitRequestApprovalSummaryDto>(
            new CommandDefinition(
                sql,
                commandType: CommandType.Text,
                cancellationToken: cancellationToken));

        return summary ?? new BenefitRequestApprovalSummaryDto();
    }

    public async Task<BenefitUsageConfirmationPairResultDto> CreateUsageConfirmationPairAsync(
    Guid requestId,
    string clientTokenHash,
    string partnerTokenHash,
    string clientConfirmationUrl,
    string partnerConfirmationUrl,
    DateTime confirmationExpiresAt,
    string partnerRecipientEmail,
    string? partnerRecipientName,
    Guid? createdByUserId,
    CancellationToken cancellationToken = default)
    {
        var parameters = new DynamicParameters();

        parameters.Add("@BenefitRequestId", requestId);
        parameters.Add("@ClientTokenHash", clientTokenHash);
        parameters.Add("@PartnerTokenHash", partnerTokenHash);
        parameters.Add("@ClientConfirmationUrl", clientConfirmationUrl);
        parameters.Add("@PartnerConfirmationUrl", partnerConfirmationUrl);
        parameters.Add("@ConfirmationExpiresAt", confirmationExpiresAt);
        parameters.Add("@PartnerRecipientEmail", partnerRecipientEmail);
        parameters.Add("@PartnerRecipientName", partnerRecipientName);
        parameters.Add("@CreatedByUserId", createdByUserId);

        var result = await _connection.QuerySingleAsync<BenefitUsageConfirmationPairResultDto>(
            new CommandDefinition(
                "dbo.usp_benefit_usage_confirmation_create_pair",
                parameters,
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken));

        return result;
    }

    public async Task<BenefitUsageConfirmationTokenDto?> GetUsageConfirmationByTokenHashAsync(
        string tokenHash,
        CancellationToken cancellationToken = default)
    {
        var parameters = new DynamicParameters();

        parameters.Add("@TokenHash", tokenHash);

        return await _connection.QueryFirstOrDefaultAsync<BenefitUsageConfirmationTokenDto>(
            new CommandDefinition(
                "dbo.usp_benefit_usage_confirmation_get_by_token_hash",
                parameters,
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken));
    }

    public async Task<BenefitUsageConfirmationConfirmResultDto> ConfirmUsageConfirmationAsync(
        string tokenHash,
        CancellationToken cancellationToken = default)
    {
        var parameters = new DynamicParameters();

        parameters.Add("@TokenHash", tokenHash);

        var result = await _connection.QuerySingleAsync<BenefitUsageConfirmationConfirmResultDto>(
            new CommandDefinition(
                "dbo.usp_benefit_usage_confirmation_confirm",
                parameters,
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken));

        return result;
    }

    private sealed class BenefitRequestDetailRow : BenefitRequestDetailDto
    {
        public Guid? VaccinationCardId { get; set; }
        public string? VaccinationCardSourceType { get; set; }
        public Guid? VaccinationCardClientDocumentId { get; set; }
        public Guid? VaccinationCardPartnerCustomerDocumentId { get; set; }
        public string? VaccinationCardFileUrl { get; set; }
        public string? VaccinationCardFileName { get; set; }
        public string? VaccinationCardSubmissionStatus { get; set; }
        public string? VaccinationCardNotes { get; set; }
        public DateTime? VaccinationCardReviewedAt { get; set; }
        public Guid? VaccinationCardReviewedByUserId { get; set; }

        public Guid? DewormerId { get; set; }
        public string? DewormerSourceType { get; set; }
        public Guid? DewormerClientPetHealthRecordId { get; set; }
        public Guid? DewormerPartnerCustomerPetHealthRecordId { get; set; }
        public string? DewormerApplicationType { get; set; }
        public string? DewormerBrandName { get; set; }
        public DateTime? DewormerAppliedAt { get; set; }
        public DateTime? DewormerExpiresAt { get; set; }
        public string? DewormerSubmissionStatus { get; set; }
        public string? DewormerNotes { get; set; }
        public DateTime? DewormerReviewedAt { get; set; }
        public Guid? DewormerReviewedByUserId { get; set; }

        public Guid? FleaTickId { get; set; }
        public string? FleaTickSourceType { get; set; }
        public Guid? FleaTickClientPetHealthRecordId { get; set; }
        public Guid? FleaTickPartnerCustomerPetHealthRecordId { get; set; }
        public string? FleaTickApplicationType { get; set; }
        public string? FleaTickBrandName { get; set; }
        public DateTime? FleaTickAppliedAt { get; set; }
        public DateTime? FleaTickExpiresAt { get; set; }
        public string? FleaTickSubmissionStatus { get; set; }
        public string? FleaTickNotes { get; set; }
        public DateTime? FleaTickReviewedAt { get; set; }
        public Guid? FleaTickReviewedByUserId { get; set; }

        public BenefitRequestDetailDto ToDetailDto()
        {
            return new BenefitRequestDetailDto
            {
                Id = Id,

                BenefitId = BenefitId,
                BenefitTitle = BenefitTitle,
                BenefitStatus = BenefitStatus,
                BenefitType = BenefitType,
                BenefitDirection = BenefitDirection,
                TargetActorType = TargetActorType,
                EligibilityType = EligibilityType,

                OperationalOwner = OperationalOwner,
                ProviderLabel = ProviderLabel,

                PartnerId = PartnerId,
                PartnerName = PartnerName,
                PartnerSegment = PartnerSegment,
                PartnerCategory = PartnerCategory,

                RequesterType = RequesterType,
                RequesterUserId = RequesterUserId,
                RequesterClientId = RequesterClientId,
                RequesterPartnerCustomerId = RequesterPartnerCustomerId,
                RequestedByUserId = RequestedByUserId,

                ClientName = ClientName,
                ClientDocument = ClientDocument,
                ClientEmail = ClientEmail,
                ClientPhone = ClientPhone,
                ClientStatus = ClientStatus,

                PartnerCustomerName = PartnerCustomerName,
                PartnerCustomerDocument = PartnerCustomerDocument,
                PartnerCustomerEmail = PartnerCustomerEmail,
                PartnerCustomerPhone = PartnerCustomerPhone,
                PartnerCustomerStatus = PartnerCustomerStatus,
                PartnerCustomerRegistrationStage = PartnerCustomerRegistrationStage,

                RequesterName = RequesterName,
                RequesterEmail = RequesterEmail,
                RequesterPhone = RequesterPhone,

                PetSourceType = PetSourceType,
                RequesterClientPetId = RequesterClientPetId,
                RequesterPartnerCustomerPetId = RequesterPartnerCustomerPetId,

                PetName = PetName,
                PetSpecies = PetSpecies,
                PetBreed = PetBreed,
                PetSex = PetSex,
                PetAgeMonths = PetAgeMonths,
                PetWeightKg = PetWeightKg,
                PetSize = PetSize,
                PetIsNeutered = PetIsNeutered,
                PetBehaviorStatus = PetBehaviorStatus,
                PetStatus = PetStatus,

                AccessCodeId = AccessCodeId,
                AccessCode = AccessCode,

                RequestStatus = RequestStatus,
                ReviewRequired = ReviewRequired,
                ApprovalStatus = ApprovalStatus,
                UsageId = UsageId,
                UsageStatus = UsageStatus,
                UsedAt = UsedAt,
                MonetaryValue = MonetaryValue,
                DiscountValue = DiscountValue,
                ApprovalRequestedAt = ApprovalRequestedAt,
                ApprovalDecidedAt = ApprovalDecidedAt,
                ApprovalDecidedByUserId = ApprovalDecidedByUserId,
                ApprovalReason = ApprovalReason,

                ReviewedAt = ReviewedAt,
                ReviewedByUserId = ReviewedByUserId,
                ReviewNotes = ReviewNotes,

                LatestReviewStatus = LatestReviewStatus,
                LatestReviewPoint = LatestReviewPoint,
                LatestReviewRecommendation = LatestReviewRecommendation,
                LatestReviewedByUserId = LatestReviewedByUserId,
                LatestReviewedAt = LatestReviewedAt,

                RequestHealthReviewStatus = RequestHealthReviewStatus,

                VaccinationCard = VaccinationCardId is null
                    ? null
                    : new BenefitRequestDocumentDto
                    {
                        Id = VaccinationCardId,
                        SourceType = VaccinationCardSourceType,
                        ClientDocumentId = VaccinationCardClientDocumentId,
                        PartnerCustomerDocumentId = VaccinationCardPartnerCustomerDocumentId,
                        FileUrl = VaccinationCardFileUrl,
                        FileName = VaccinationCardFileName,
                        SubmissionStatus = VaccinationCardSubmissionStatus,
                        Notes = VaccinationCardNotes,
                        ReviewedAt = VaccinationCardReviewedAt,
                        ReviewedByUserId = VaccinationCardReviewedByUserId
                    },

                Dewormer = DewormerId is null
                    ? null
                    : new BenefitRequestPreventiveDto
                    {
                        Id = DewormerId,
                        SourceType = DewormerSourceType,
                        ClientPetHealthRecordId = DewormerClientPetHealthRecordId,
                        PartnerCustomerPetHealthRecordId = DewormerPartnerCustomerPetHealthRecordId,
                        ApplicationType = DewormerApplicationType,
                        BrandName = DewormerBrandName,
                        AppliedAt = DewormerAppliedAt,
                        ExpiresAt = DewormerExpiresAt,
                        SubmissionStatus = DewormerSubmissionStatus,
                        Notes = DewormerNotes,
                        ReviewedAt = DewormerReviewedAt,
                        ReviewedByUserId = DewormerReviewedByUserId
                    },

                FleaTick = FleaTickId is null
                    ? null
                    : new BenefitRequestPreventiveDto
                    {
                        Id = FleaTickId,
                        SourceType = FleaTickSourceType,
                        ClientPetHealthRecordId = FleaTickClientPetHealthRecordId,
                        PartnerCustomerPetHealthRecordId = FleaTickPartnerCustomerPetHealthRecordId,
                        ApplicationType = FleaTickApplicationType,
                        BrandName = FleaTickBrandName,
                        AppliedAt = FleaTickAppliedAt,
                        ExpiresAt = FleaTickExpiresAt,
                        SubmissionStatus = FleaTickSubmissionStatus,
                        Notes = FleaTickNotes,
                        ReviewedAt = FleaTickReviewedAt,
                        ReviewedByUserId = FleaTickReviewedByUserId
                    },

                RequestedAt = RequestedAt,
                ScheduledFor = ScheduledFor,
                ExpiresAt = ExpiresAt,
                CreatedAt = CreatedAt,
                UpdatedAt = UpdatedAt
            };
        }
    }
}