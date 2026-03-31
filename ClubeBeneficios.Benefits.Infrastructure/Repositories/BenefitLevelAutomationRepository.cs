using System.Data;
using Dapper;
using ClubeBeneficios.Benefits.Domain.Dtos;
using ClubeBeneficios.Benefits.Domain.Dtos.Requests;
using ClubeBeneficios.Benefits.Domain.Repositories;

namespace ClubeBeneficios.Benefits.Infrastructure.Repositories;

public class BenefitLevelAutomationRepository : IBenefitLevelAutomationRepository
{
    private readonly IDbConnection _connection;

    public BenefitLevelAutomationRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<IEnumerable<PartnerLevelAutomationResultDto>> RecalculatePartnerLevelsAsync(RecalculatePartnerLevelsRequest request, CancellationToken cancellationToken = default)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@partner_id", request.PartnerId);
        parameters.Add("@reference_date", request.ReferenceDate);

        var command = new CommandDefinition(
            "dbo.usp_partner_levels_recalculate",
            parameters,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        return await _connection.QueryAsync<PartnerLevelAutomationResultDto>(command);
    }

    public async Task<IEnumerable<ClientLevelAutomationResultDto>> RecalculateClientLevelsAsync(RecalculateClientLevelsRequest request, CancellationToken cancellationToken = default)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@user_id", request.UserId);
        parameters.Add("@reference_date", request.ReferenceDate);

        var command = new CommandDefinition(
            "dbo.usp_client_levels_recalculate",
            parameters,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        return await _connection.QueryAsync<ClientLevelAutomationResultDto>(command);
    }
}