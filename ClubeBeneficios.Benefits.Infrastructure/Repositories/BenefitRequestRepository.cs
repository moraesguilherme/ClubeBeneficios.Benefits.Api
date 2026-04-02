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
        var createdId = Guid.NewGuid();

        const string sql = @"
                            insert into dbo.benefit_requests
                            (
                                id, benefit_id, partner_id, requester_user_id, requester_partner_customer_id,
                                requester_type, pet_id, access_code_id, request_status,
                                requested_at, reviewed_at, reviewed_by_user_id, review_notes,
                                scheduled_for, expires_at, created_at, updated_at
                            )
                            values
                            (
                                @id, @benefit_id, @partner_id, @requester_user_id, @requester_partner_customer_id,
                                @requester_type, @pet_id, @access_code_id, 'requested',
                                sysutcdatetime(), null, null, @review_notes,
                                @scheduled_for, null, sysutcdatetime(), sysutcdatetime()
                            );
                            ";

        await _connection.ExecuteAsync(
            new CommandDefinition(
                sql,
                new
                {
                    id = createdId,
                    benefit_id = request.BenefitId,
                    partner_id = request.PartnerId,
                    requester_user_id = request.RequesterUserId,
                    requester_partner_customer_id = request.RequesterPartnerCustomerId,
                    requester_type = request.RequesterType,
                    pet_id = request.PetId,
                    access_code_id = request.AccessCodeId,
                    review_notes = request.ReviewNotes,
                    scheduled_for = request.ScheduledFor
                },
                commandType: CommandType.Text,
                cancellationToken: cancellationToken));

        return createdId;
    }

    public async Task ChangeStatusAsync(Guid requestId, ChangeBenefitRequestStatusRequest request, Guid? performedByUserId, CancellationToken cancellationToken = default)
    {
        const string sql = @"
                            update dbo.benefit_requests
                            set
                                request_status = @request_status,
                                reviewed_at = case when @request_status in ('approved','declined','cancelled','expired') then sysutcdatetime() else reviewed_at end,
                                reviewed_by_user_id = case when @request_status in ('approved','declined','cancelled','expired') then @performed_by_user_id else reviewed_by_user_id end,
                                review_notes = @review_notes,
                                scheduled_for = @scheduled_for,
                                updated_at = sysutcdatetime()
                            where id = @benefit_request_id;
                            ";

        await _connection.ExecuteAsync(
            new CommandDefinition(
                sql,
                new
                {
                    benefit_request_id = requestId,
                    request_status = request.RequestStatus,
                    review_notes = request.ReviewNotes,
                    scheduled_for = request.ScheduledFor,
                    performed_by_user_id = performedByUserId
                },
                commandType: CommandType.Text,
                cancellationToken: cancellationToken));
    }

    public Task<BenefitRequestDetailDto?> GetByIdAsync(Guid requestId, CancellationToken cancellationToken = default)
    {
        const string sql = @"
                            select top 1
                                br.id as Id,
                                br.benefit_id as BenefitId,
                                br.partner_id as PartnerId,
                                br.requester_user_id as RequesterUserId,
                                br.requester_partner_customer_id as RequesterPartnerCustomerId,
                                br.requester_type as RequesterType,
                                br.pet_id as PetId,
                                br.access_code_id as AccessCodeId,
                                br.request_status as RequestStatus,
                                br.requested_at as RequestedAt,
                                br.reviewed_at as ReviewedAt,
                                br.reviewed_by_user_id as ReviewedByUserId,
                                br.review_notes as ReviewNotes,
                                br.scheduled_for as ScheduledFor,
                                br.expires_at as ExpiresAt
                            from dbo.benefit_requests br
                            where br.id = @benefit_request_id;
                            ";

        return _connection.QueryFirstOrDefaultAsync<BenefitRequestDetailDto>(
            new CommandDefinition(sql, new { benefit_request_id = requestId }, commandType: CommandType.Text, cancellationToken: cancellationToken));
    }

    public async Task<PagedResultDto<BenefitRequestListItemDto>> SearchAsync(BenefitRequestFilterDto filter, CancellationToken cancellationToken = default)
    {
        const string itemsSql = @"
                                select
                                    br.id as Id,
                                    br.benefit_id as BenefitId,
                                    br.partner_id as PartnerId,
                                    br.requester_user_id as RequesterUserId,
                                    br.requester_partner_customer_id as RequesterPartnerCustomerId,
                                    br.requester_type as RequesterType,
                                    br.request_status as RequestStatus,
                                    br.requested_at as RequestedAt,
                                    br.reviewed_at as ReviewedAt,
                                    br.scheduled_for as ScheduledFor,
                                    b.title as BenefitTitle,
                                    p.trade_name as PartnerName
                                from dbo.benefit_requests br
                                inner join dbo.benefits b on b.id = br.benefit_id
                                left join dbo.partners p on p.id = br.partner_id
                                where (@search is null or b.title like '%' + @search + '%' or p.trade_name like '%' + @search + '%')
                                  and (@benefit_id is null or br.benefit_id = @benefit_id)
                                  and (@partner_id is null or br.partner_id = @partner_id)
                                  and (@request_status is null or br.request_status = @request_status)
                                  and (@requester_type is null or br.requester_type = @requester_type)
                                  and (@requested_from is null or br.requested_at >= @requested_from)
                                  and (@requested_to is null or br.requested_at < dateadd(day, 1, @requested_to))
                                order by br.requested_at desc
                                offset @offset rows fetch next @page_size rows only;
                                ";

        const string totalSql = @"
                                select count(1)
                                from dbo.benefit_requests br
                                inner join dbo.benefits b on b.id = br.benefit_id
                                left join dbo.partners p on p.id = br.partner_id
                                where (@search is null or b.title like '%' + @search + '%' or p.trade_name like '%' + @search + '%')
                                  and (@benefit_id is null or br.benefit_id = @benefit_id)
                                  and (@partner_id is null or br.partner_id = @partner_id)
                                  and (@request_status is null or br.request_status = @request_status)
                                  and (@requester_type is null or br.requester_type = @requester_type)
                                  and (@requested_from is null or br.requested_at >= @requested_from)
                                  and (@requested_to is null or br.requested_at < dateadd(day, 1, @requested_to));
                                ";

        var parameters = new
        {
            search = filter.Search,
            benefit_id = filter.BenefitId,
            partner_id = filter.PartnerId,
            request_status = filter.RequestStatus,
            requester_type = filter.RequesterType,
            requested_from = filter.RequestedFrom,
            requested_to = filter.RequestedTo,
            offset = (filter.Page - 1) * filter.PageSize,
            page_size = filter.PageSize
        };

        var items = (await _connection.QueryAsync<BenefitRequestListItemDto>(
            new CommandDefinition(itemsSql, parameters, commandType: CommandType.Text, cancellationToken: cancellationToken))).ToList();

        var totalCount = await _connection.ExecuteScalarAsync<int>(
            new CommandDefinition(totalSql, parameters, commandType: CommandType.Text, cancellationToken: cancellationToken));

        return new PagedResultDto<BenefitRequestListItemDto>
        {
            Items = items,
            Page = filter.Page,
            PageSize = filter.PageSize,
            TotalCount = totalCount
        };
    }
}