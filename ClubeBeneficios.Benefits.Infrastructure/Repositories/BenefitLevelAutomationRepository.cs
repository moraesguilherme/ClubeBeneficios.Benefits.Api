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

    public async Task<IEnumerable<PartnerLevelAutomationResultDto>> RecalculatePartnerLevelsAsync(
        RecalculatePartnerLevelsRequest request,
        Guid? changedByUserId,
        CancellationToken cancellationToken = default)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@PartnerId", request.PartnerId);
        parameters.Add("@ChangedByUserId", changedByUserId);

        var command = new CommandDefinition(
            "dbo.usp_partner_levels_recalculate",
            parameters,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        return await _connection.QueryAsync<PartnerLevelAutomationResultDto>(command);
    }

    public Task<IEnumerable<ClientLevelAutomationResultDto>> RecalculateClientLevelsAsync(RecalculateClientLevelsRequest request, CancellationToken cancellationToken = default)
    {
        IEnumerable<ClientLevelAutomationResultDto> result = Array.Empty<ClientLevelAutomationResultDto>();
        return Task.FromResult(result);
    }
}