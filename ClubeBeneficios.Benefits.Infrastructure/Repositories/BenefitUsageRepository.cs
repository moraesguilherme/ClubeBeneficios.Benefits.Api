using System.Data;
using Dapper;
using ClubeBeneficios.Benefits.Domain.Dtos;
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
        var parameters = new DynamicParameters();
        parameters.Add("@benefit_id", request.BenefitId);
        parameters.Add("@benefit_request_id", request.BenefitRequestId);
        parameters.Add("@partner_id", request.PartnerId);
        parameters.Add("@used_by_user_id", request.UsedByUserId);
        parameters.Add("@used_by_partner_customer_id", request.UsedByPartnerCustomerId);
        parameters.Add("@used_by_type", request.UsedByType);
        parameters.Add("@pet_id", request.PetId);
        parameters.Add("@used_at", request.UsedAt);
        parameters.Add("@monetary_value", request.MonetaryValue);
        parameters.Add("@discount_value", request.DiscountValue);
        parameters.Add("@snapshot_title", request.SnapshotTitle);
        parameters.Add("@snapshot_partner_name", request.SnapshotPartnerName);
        parameters.Add("@snapshot_rule_summary", request.SnapshotRuleSummary);
        parameters.Add("@performed_by_user_id", performedByUserId);
        parameters.Add("@performed_by_partner", performedByPartner);

        var command = new CommandDefinition(
            "dbo.usp_benefit_usages_confirm",
            parameters,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        return await _connection.ExecuteScalarAsync<Guid>(command);
    }

    public async Task CancelAsync(Guid usageId, CancelBenefitUsageRequest request, Guid? performedByUserId, CancellationToken cancellationToken = default)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@benefit_usage_id", usageId);
        parameters.Add("@cancellation_reason", request.CancellationReason);
        parameters.Add("@performed_by_user_id", performedByUserId);

        var command = new CommandDefinition(
            "dbo.usp_benefit_usages_cancel",
            parameters,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        await _connection.ExecuteAsync(command);
    }

    public async Task<BenefitUsageDetailDto?> GetByIdAsync(Guid usageId, CancellationToken cancellationToken = default)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@benefit_usage_id", usageId);

        var command = new CommandDefinition(
            "dbo.usp_benefit_usages_get_by_id",
            parameters,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        return await _connection.QueryFirstOrDefaultAsync<BenefitUsageDetailDto>(command);
    }

    public async Task<PagedResultDto<BenefitUsageListItemDto>> SearchAsync(BenefitUsageFilterDto filter, CancellationToken cancellationToken = default)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@search", filter.Search);
        parameters.Add("@benefit_id", filter.BenefitId);
        parameters.Add("@benefit_request_id", filter.BenefitRequestId);
        parameters.Add("@partner_id", filter.PartnerId);
        parameters.Add("@usage_status", filter.UsageStatus);
        parameters.Add("@used_by_type", filter.UsedByType);
        parameters.Add("@used_from", filter.UsedFrom);
        parameters.Add("@used_to", filter.UsedTo);
        parameters.Add("@page", filter.Page);
        parameters.Add("@page_size", filter.PageSize);

        var command = new CommandDefinition(
            "dbo.usp_benefit_usages_search",
            parameters,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        using var multi = await _connection.QueryMultipleAsync(command);
        var items = (await multi.ReadAsync<BenefitUsageListItemDto>()).ToList();
        var totalCount = await multi.ReadFirstOrDefaultAsync<int>();

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
        var parameters = new DynamicParameters();
        parameters.Add("@benefit_id", request.BenefitId);
        parameters.Add("@partner_id", request.PartnerId);
        parameters.Add("@user_id", request.UserId);
        parameters.Add("@partner_customer_id", request.PartnerCustomerId);
        parameters.Add("@actor_type", request.ActorType);
        parameters.Add("@validation_reference_date", request.ValidationReferenceDate);

        var command = new CommandDefinition(
            "dbo.usp_benefit_usage_validate",
            parameters,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        return await _connection.QueryFirstOrDefaultAsync<BenefitEligibilityValidationResultDto>(command);
    }
}