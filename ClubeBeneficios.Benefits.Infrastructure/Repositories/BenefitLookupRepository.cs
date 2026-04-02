using System.Data;
using Dapper;
using ClubeBeneficios.Benefits.Domain.Dtos;
using ClubeBeneficios.Benefits.Domain.Repositories;

namespace ClubeBeneficios.Benefits.Infrastructure.Repositories;

public class BenefitLookupRepository : IBenefitLookupRepository
{
    private readonly IDbConnection _connection;

    public BenefitLookupRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<BenefitLookupOptionsDto> GetOptionsAsync(Guid? partnerId = null, CancellationToken cancellationToken = default)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@partner_id", partnerId);

        var statuses = await QueryValuesAsync(@"
                                                select distinct b.status
                                                from dbo.benefits b
                                                where b.status is not null
                                                  and (@partner_id is null or b.partner_id = @partner_id)
                                                order by b.status;", parameters, cancellationToken);

        var directions = await QueryValuesAsync(@"
                                                select distinct b.direction
                                                from dbo.benefits b
                                                where b.direction is not null
                                                  and (@partner_id is null or b.partner_id = @partner_id)
                                                order by b.direction;", parameters, cancellationToken);

        var targetActorTypes = await QueryValuesAsync(@"
                                                        select distinct b.target_actor_type
                                                        from dbo.benefits b
                                                        where b.target_actor_type is not null
                                                          and (@partner_id is null or b.partner_id = @partner_id)
                                                        order by b.target_actor_type;", parameters, cancellationToken);

        var eligibilityTypes = await QueryValuesAsync(@"
                                                        select distinct b.eligibility_type
                                                        from dbo.benefits b
                                                        where b.eligibility_type is not null
                                                          and (@partner_id is null or b.partner_id = @partner_id)
                                                        order by b.eligibility_type;", parameters, cancellationToken);

        var recurrencePeriods = await QueryValuesAsync(@"
                                                        select distinct b.recurrence_period
                                                        from dbo.benefits b
                                                        where b.recurrence_period is not null
                                                          and (@partner_id is null or b.partner_id = @partner_id)
                                                        order by b.recurrence_period;", parameters, cancellationToken);

        return new BenefitLookupOptionsDto
        {
            Statuses = statuses,
            Directions = directions,
            TargetActorTypes = targetActorTypes,
            EligibilityTypes = eligibilityTypes,
            RecurrencePeriods = recurrencePeriods
        };
    }

    private async Task<IEnumerable<string>> QueryValuesAsync(
        string sql,
        DynamicParameters parameters,
        CancellationToken cancellationToken)
    {
        var command = new CommandDefinition(
            sql,
            parameters,
            commandType: CommandType.Text,
            cancellationToken: cancellationToken);

        var values = await _connection.QueryAsync<string>(command);
        return values.Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToArray();
    }
}