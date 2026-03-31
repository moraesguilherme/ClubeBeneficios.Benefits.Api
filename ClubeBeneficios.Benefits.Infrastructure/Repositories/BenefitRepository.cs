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
        var parameters = new DynamicParameters();
        parameters.Add("@search", filter.Search);
        parameters.Add("@partner_id", filter.PartnerId);
        parameters.Add("@origin", filter.Origin);
        parameters.Add("@status", filter.Status);
        parameters.Add("@target_actor_type", filter.TargetActorType);
        parameters.Add("@eligibility_type", filter.EligibilityType);
        parameters.Add("@only_pending_approval", filter.OnlyPendingApproval);
        parameters.Add("@page", filter.Page);
        parameters.Add("@page_size", filter.PageSize);

        using var multi = await _connection.QueryMultipleAsync(new CommandDefinition(
            "dbo.usp_benefits_admin_search",
            parameters,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken));

        var items = (await multi.ReadAsync<BenefitListItemDto>()).ToList();
        var total = await multi.ReadFirstOrDefaultAsync<int>();

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
        var parameters = new DynamicParameters();
        parameters.Add("@search", filter.Search);
        parameters.Add("@partner_id", filter.PartnerId);
        parameters.Add("@origin", filter.Origin);
        parameters.Add("@status", filter.Status);
        parameters.Add("@page", filter.Page);
        parameters.Add("@page_size", filter.PageSize);

        using var multi = await _connection.QueryMultipleAsync(new CommandDefinition(
            "dbo.usp_benefits_pending_search",
            parameters,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken));

        var items = (await multi.ReadAsync<BenefitApprovalQueueItemDto>()).ToList();
        var total = await multi.ReadFirstOrDefaultAsync<int>();

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
        return await _connection.QueryFirstAsync<BenefitSummaryDto>(new CommandDefinition(
            "dbo.usp_benefits_admin_summary",
            new { partner_id = partnerId },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken));
    }

    public async Task<BenefitFilterOptionsDto> GetFilterOptionsAsync(Guid? partnerId, CancellationToken cancellationToken = default)
    {
        using var multi = await _connection.QueryMultipleAsync(new CommandDefinition(
            "dbo.usp_benefits_filter_options",
            new { partner_id = partnerId },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken));

        return new BenefitFilterOptionsDto
        {
            Origins = (await multi.ReadAsync<LookupItemDto>()).ToList(),
            Statuses = (await multi.ReadAsync<LookupItemDto>()).ToList(),
            Audiences = (await multi.ReadAsync<LookupItemDto>()).ToList(),
            EligibilityTypes = (await multi.ReadAsync<LookupItemDto>()).ToList(),
            Partners = (await multi.ReadAsync<LookupItemDto>()).ToList(),
        };
    }

    public async Task<BenefitDetailsDto?> GetByIdAsync(Guid id, Guid? partnerId, bool enforcePartnerOwnership, CancellationToken cancellationToken = default)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@id", id);
        parameters.Add("@partner_id", partnerId);
        parameters.Add("@enforce_partner_ownership", enforcePartnerOwnership);

        using var multi = await _connection.QueryMultipleAsync(new CommandDefinition(
            "dbo.usp_benefits_get_by_id",
            parameters,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken));

        var details = await multi.ReadFirstOrDefaultAsync<BenefitDetailsDto>();
        if (details is null)
        {
            return null;
        }

        var chips = (await multi.ReadAsync<string>()).ToList();
        details.EligibilityChips = chips;
        return details;
    }

    public async Task<Guid> CreateAsync(CreateBenefitRequest request, Guid createdByUserId, Guid? partnerContextId, bool isPartnerScope, CancellationToken cancellationToken = default)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@partner_id", isPartnerScope ? partnerContextId : request.PartnerId);
        parameters.Add("@title", request.Title);
        parameters.Add("@benefit_type", request.BenefitType);
        parameters.Add("@direction", request.Direction);
        parameters.Add("@target_actor_type", request.TargetActorType);
        parameters.Add("@short_description", request.ShortDescription);
        parameters.Add("@full_description", request.FullDescription);
        parameters.Add("@internal_notes", request.InternalNotes);
        parameters.Add("@eligibility_type", request.EligibilityType);
        parameters.Add("@eligibility_summary", request.EligibilitySummary);
        parameters.Add("@eligibility_chips", request.EligibilityChips);
        parameters.Add("@recurrence_type", request.RecurrenceType);
        parameters.Add("@recurrence_value", request.RecurrenceValue);
        parameters.Add("@recurrence_period", request.RecurrencePeriod);
        parameters.Add("@validity_type", request.ValidityType);
        parameters.Add("@starts_at", request.StartsAt);
        parameters.Add("@ends_at", request.EndsAt);
        parameters.Add("@auto_activate_when_approved", request.AutoActivateWhenApproved);
        parameters.Add("@requires_manual_release", request.RequiresManualRelease);
        parameters.Add("@highlight_in_showcase", request.HighlightInShowcase);
        parameters.Add("@stacking_rule", request.StackingRule);
        parameters.Add("@created_by_user_id", createdByUserId);
        parameters.Add("@created_id", dbType: DbType.Guid, direction: ParameterDirection.Output);

        await _connection.ExecuteAsync(new CommandDefinition(
            "dbo.usp_benefits_create",
            parameters,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken));

        return parameters.Get<Guid>("@created_id");
    }

    public Task UpdateAsync(Guid id, UpdateBenefitRequest request, Guid updatedByUserId, Guid? partnerContextId, bool isPartnerScope, CancellationToken cancellationToken = default)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@id", id);
        parameters.Add("@partner_id", partnerContextId);
        parameters.Add("@enforce_partner_ownership", isPartnerScope);
        parameters.Add("@title", request.Title);
        parameters.Add("@benefit_type", request.BenefitType);
        parameters.Add("@target_actor_type", request.TargetActorType);
        parameters.Add("@short_description", request.ShortDescription);
        parameters.Add("@full_description", request.FullDescription);
        parameters.Add("@internal_notes", request.InternalNotes);
        parameters.Add("@eligibility_type", request.EligibilityType);
        parameters.Add("@eligibility_summary", request.EligibilitySummary);
        parameters.Add("@eligibility_chips", request.EligibilityChips);
        parameters.Add("@recurrence_type", request.RecurrenceType);
        parameters.Add("@recurrence_value", request.RecurrenceValue);
        parameters.Add("@recurrence_period", request.RecurrencePeriod);
        parameters.Add("@validity_type", request.ValidityType);
        parameters.Add("@starts_at", request.StartsAt);
        parameters.Add("@ends_at", request.EndsAt);
        parameters.Add("@auto_activate_when_approved", request.AutoActivateWhenApproved);
        parameters.Add("@requires_manual_release", request.RequiresManualRelease);
        parameters.Add("@highlight_in_showcase", request.HighlightInShowcase);
        parameters.Add("@stacking_rule", request.StackingRule);
        parameters.Add("@updated_by_user_id", updatedByUserId);

        return _connection.ExecuteAsync(new CommandDefinition(
            "dbo.usp_benefits_update",
            parameters,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken));
    }

    public Task ChangeStatusAsync(Guid id, ChangeBenefitStatusRequest request, Guid changedByUserId, Guid? partnerContextId, bool isPartnerScope, CancellationToken cancellationToken = default)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@id", id);
        parameters.Add("@partner_id", partnerContextId);
        parameters.Add("@enforce_partner_ownership", isPartnerScope);
        parameters.Add("@status", request.Status);
        parameters.Add("@reason", request.Reason);
        parameters.Add("@changed_by_user_id", changedByUserId);

        return _connection.ExecuteAsync(new CommandDefinition(
            "dbo.usp_benefits_change_status",
            parameters,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken));
    }

    public Task AddReviewAsync(Guid id, ReviewBenefitRequest request, Guid reviewedByUserId, CancellationToken cancellationToken = default)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@benefit_id", id);
        parameters.Add("@review_status", request.ReviewStatus);
        parameters.Add("@review_point", request.ReviewPoint);
        parameters.Add("@review_recommendation", request.ReviewRecommendation);
        parameters.Add("@reviewed_by_user_id", reviewedByUserId);

        return _connection.ExecuteAsync(new CommandDefinition(
            "dbo.usp_benefits_add_review",
            parameters,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken));
    }
}