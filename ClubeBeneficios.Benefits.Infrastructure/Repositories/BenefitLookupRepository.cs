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

    public async Task<BenefitLookupOptionsDto> GetOptionsAsync(
        Guid? partnerId = null,
        CancellationToken cancellationToken = default)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@partner_id", partnerId);

        var statuses = await QueryValuesAsync(@"
            select distinct b.status
            from dbo.benefits b
            where b.status is not null
              and (@partner_id is null or b.partner_id = @partner_id);",
            parameters,
            cancellationToken);

        var directions = await QueryValuesAsync(@"
            select distinct b.direction
            from dbo.benefits b
            where b.direction is not null
              and (@partner_id is null or b.partner_id = @partner_id);",
            parameters,
            cancellationToken);

        var targetActorTypes = await QueryValuesAsync(@"
            select distinct b.target_actor_type
            from dbo.benefits b
            where b.target_actor_type is not null
              and (@partner_id is null or b.partner_id = @partner_id);",
            parameters,
            cancellationToken);

        var eligibilityTypes = await QueryValuesAsync(@"
            select distinct b.eligibility_type
            from dbo.benefits b
            where b.eligibility_type is not null
              and (@partner_id is null or b.partner_id = @partner_id);",
            parameters,
            cancellationToken);

        var recurrencePeriods = await QueryValuesAsync(@"
            select distinct b.recurrence_period
            from dbo.benefits b
            where b.recurrence_period is not null
              and (@partner_id is null or b.partner_id = @partner_id);",
            parameters,
            cancellationToken);

        var benefitTypes = await QueryValuesAsync(@"
            select distinct b.benefit_type
            from dbo.benefits b
            where b.benefit_type is not null
              and (@partner_id is null or b.partner_id = @partner_id);",
            parameters,
            cancellationToken);

        var clientLevels = await QueryValuesAsync(@"
            select distinct bls.level_code
            from dbo.benefit_level_scopes bls
            where bls.level_type = 'client_level'
              and bls.level_code is not null;",
            new DynamicParameters(),
            cancellationToken);

        var partnerLevels = await QueryValuesAsync(@"
            select distinct bls.level_code
            from dbo.benefit_level_scopes bls
            where bls.level_type = 'partner_level'
              and bls.level_code is not null;",
            new DynamicParameters(),
            cancellationToken);

        return new BenefitLookupOptionsDto
        {
            Statuses = ToLookupItems(
                EnsureDefaultOrder(
                    statuses,
                    "draft", "pending_review", "under_review", "approved",
                    "active", "inactive", "rejected", "expired", "archived")),

            Directions = ToLookupItems(
                EnsureDefaultOrder(
                    directions,
                    "partner_to_matilha", "matilha_to_partner")),

            TargetActorTypes = ToLookupItems(
                EnsureDefaultOrder(
                    targetActorTypes,
                    "client", "partner_customer")),

            EligibilityTypes = ToLookupItems(
                EnsureDefaultOrder(
                    eligibilityTypes,
                    "open", "level", "behavior", "code", "hybrid")),

            RecurrencePeriods = ToLookupItems(
                EnsureDefaultOrder(
                    recurrencePeriods,
                    "day", "week", "month", "quarter", "semester", "year")),

            BenefitTypes = ToLookupItems(
                EnsureDefaultOrder(
                    benefitTypes,
                    "discount", "service", "gift", "daily_rate", "evaluation",
                    "upgrade", "raffle", "event", "experience", "custom")),

            EligibleLevels = new BenefitEligibleLevelsLookupDto
            {
                ClientLevels = ToLookupItems(
                    EnsureDefaultOrder(
                        clientLevels,
                        "bronze", "silver", "gold", "diamond", "platinum")),

                PartnerLevels = ToLookupItems(
                    EnsureDefaultOrder(
                        partnerLevels,
                        "bronze", "silver", "gold", "diamond", "platinum"))
            },

            RecurrenceCards = BuildRecurrenceCards(),
            ValidityCards = BuildValidityCards()
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

        return values
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private static IEnumerable<string> EnsureDefaultOrder(
        IEnumerable<string> values,
        params string[] defaultOrder)
    {
        var existing = values
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var ordered = defaultOrder
            .Where(d => existing.Any(x => string.Equals(x, d, StringComparison.OrdinalIgnoreCase)))
            .ToList();

        var remaining = existing
            .Where(x => !defaultOrder.Any(d => string.Equals(d, x, StringComparison.OrdinalIgnoreCase)))
            .OrderBy(x => x)
            .ToList();

        ordered.AddRange(remaining);
        return ordered;
    }

    private static IEnumerable<LookupItemDto> ToLookupItems(IEnumerable<string> values)
    {
        return values.Select(ToLookupItem).ToArray();
    }

    private static LookupItemDto ToLookupItem(string value)
    {
        return new LookupItemDto
        {
            Value = value,
            Label = GetLabel(value)
        };
    }

    private static string GetLabel(string value)
    {
        return value.ToLowerInvariant() switch
        {
            "draft" => "Rascunho",
            "pending_review" => "Pendente de revisão",
            "under_review" => "Em revisão",
            "approved" => "Aprovado",
            "active" => "Ativo",
            "inactive" => "Inativo",
            "rejected" => "Rejeitado",
            "expired" => "Expirado",
            "archived" => "Arquivado",

            "partner_to_matilha" => "Parceiro → Cliente Matilha",
            "matilha_to_partner" => "Matilha → Cliente do parceiro",

            "client" => "Cliente Matilha",
            "partner_customer" => "Cliente do parceiro",

            "open" => "Aberto",
            "level" => "Por nível",
            "behavior" => "Por comportamento",
            "code" => "Por código",
            "hybrid" => "Híbrido",

            "day" => "Dia",
            "week" => "Semana",
            "month" => "Mês",
            "quarter" => "Trimestre",
            "semester" => "Semestre",
            "year" => "Ano",

            "discount" => "Desconto",
            "service" => "Serviço",
            "gift" => "Brinde",
            "daily_rate" => "Diária",
            "evaluation" => "Avaliação",
            "upgrade" => "Upgrade",
            "raffle" => "Sorteio",
            "event" => "Evento",
            "experience" => "Experiência",
            "custom" => "Personalizado",

            "bronze" => "Bronze",
            "silver" => "Prata",
            "gold" => "Ouro",
            "diamond" => "Diamante",
            "platinum" => "Platinum",

            _ => value
        };
    }

    private static IEnumerable<BenefitLookupCardDto> BuildRecurrenceCards()
    {
        return new[]
        {
            new BenefitLookupCardDto
            {
                Value = "once_per_customer",
                Title = "1 vez por cliente",
                Description = "O benefício pode ser utilizado apenas uma vez por cliente."
            },
            new BenefitLookupCardDto
            {
                Value = "limited_per_period",
                Title = "Recorrência periódica",
                Description = "Permite uso recorrente dentro de uma frequência definida."
            },
            new BenefitLookupCardDto
            {
                Value = "unlimited_within_rule",
                Title = "Uso livre",
                Description = "Permite múltiplas utilizações conforme as regras do benefício."
            },
            new BenefitLookupCardDto
            {
                Value = "first_use_only",
                Title = "Primeiro uso",
                Description = "Disponível apenas na primeira utilização elegível."
            }
        };
    }

    private static IEnumerable<BenefitLookupCardDto> BuildValidityCards()
    {
        return new[]
        {
            new BenefitLookupCardDto
            {
                Value = "continuous",
                Title = "Validade contínua",
                Description = "O benefício permanece ativo sem data fixa para encerramento."
            },
            new BenefitLookupCardDto
            {
                Value = "date_range",
                Title = "Período fixo",
                Description = "O benefício pode ser utilizado apenas entre datas definidas."
            },
            new BenefitLookupCardDto
            {
                Value = "until_stock",
                Title = "Até acabar estoque",
                Description = "O benefício permanece disponível enquanto houver disponibilidade."
            },
            new BenefitLookupCardDto
            {
                Value = "campaign_period",
                Title = "Período de campanha",
                Description = "O benefício segue a janela de validade da campanha associada."
            }
        };
    }
}