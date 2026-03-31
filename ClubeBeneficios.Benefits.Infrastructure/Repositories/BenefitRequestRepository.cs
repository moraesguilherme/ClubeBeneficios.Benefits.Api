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

    public async Task<Guid> CreateAsync(CreateBenefitRequestRequest request, Guid? performedByUserId, CancellationToken cancellationToken = default)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@benefit_id", request.BenefitId);
        parameters.Add("@partner_id", request.PartnerId);
        parameters.Add("@requester_user_id", request.RequesterUserId);
        parameters.Add("@requester_partner_customer_id", request.RequesterPartnerCustomerId);
        parameters.Add("@requester_type", request.RequesterType);
        parameters.Add("@pet_id", request.PetId);
        parameters.Add("@access_code_id", request.AccessCodeId);
        parameters.Add("@scheduled_for", request.ScheduledFor);
        parameters.Add("@review_notes", request.ReviewNotes);
        parameters.Add("@performed_by_user_id", performedByUserId);

        var command = new CommandDefinition(
            "dbo.usp_benefit_requests_create",
            parameters,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        return await _connection.ExecuteScalarAsync<Guid>(command);
    }

    public async Task ChangeStatusAsync(Guid requestId, ChangeBenefitRequestStatusRequest request, Guid? performedByUserId, CancellationToken cancellationToken = default)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@benefit_request_id", requestId);
        parameters.Add("@request_status", request.RequestStatus);
        parameters.Add("@review_notes", request.ReviewNotes);
        parameters.Add("@scheduled_for", request.ScheduledFor);
        parameters.Add("@performed_by_user_id", performedByUserId);

        var command = new CommandDefinition(
            "dbo.usp_benefit_requests_change_status",
            parameters,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        await _connection.ExecuteAsync(command);
    }

    public async Task<BenefitRequestDetailDto?> GetByIdAsync(Guid requestId, CancellationToken cancellationToken = default)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@benefit_request_id", requestId);

        var command = new CommandDefinition(
            "dbo.usp_benefit_requests_get_by_id",
            parameters,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        return await _connection.QueryFirstOrDefaultAsync<BenefitRequestDetailDto>(command);
    }

    public async Task<PagedResultDto<BenefitRequestListItemDto>> SearchAsync(BenefitRequestFilterDto filter, CancellationToken cancellationToken = default)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@search", filter.Search);
        parameters.Add("@benefit_id", filter.BenefitId);
        parameters.Add("@partner_id", filter.PartnerId);
        parameters.Add("@request_status", filter.RequestStatus);
        parameters.Add("@requester_type", filter.RequesterType);
        parameters.Add("@requested_from", filter.RequestedFrom);
        parameters.Add("@requested_to", filter.RequestedTo);
        parameters.Add("@page", filter.Page);
        parameters.Add("@page_size", filter.PageSize);

        var command = new CommandDefinition(
            "dbo.usp_benefit_requests_search",
            parameters,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        using var multi = await _connection.QueryMultipleAsync(command);
        var items = (await multi.ReadAsync<BenefitRequestListItemDto>()).ToList();
        var totalCount = await multi.ReadFirstOrDefaultAsync<int>();

        return new PagedResultDto<BenefitRequestListItemDto>
        {
            Items = items,
            Page = filter.Page,
            PageSize = filter.PageSize,
            TotalCount = totalCount
        };
    }
}