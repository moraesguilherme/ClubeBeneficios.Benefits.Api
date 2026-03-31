using System;
using System.Collections.Generic;
using System.Linq;
using ClubeBeneficios.Benefits.Domain.Constants;
using ClubeBeneficios.Benefits.Domain.Dtos;
using ClubeBeneficios.Benefits.Domain.Dtos.Requests;

namespace ClubeBeneficios.Benefits.Infrastructure.Helpers;

internal static class BenefitContractMapper
{
    public static string NormalizeStatus(string? status)
    {
        if (string.IsNullOrWhiteSpace(status))
            return BenefitDomainConstants.Status.PendingReview;

        var normalized = status.Trim().ToLowerInvariant();

        return normalized switch
        {
            "pendente" => BenefitDomainConstants.Status.PendingReview,
            "pending" => BenefitDomainConstants.Status.PendingReview,
            "pending_review" => BenefitDomainConstants.Status.PendingReview,
            "em revisao" => BenefitDomainConstants.Status.UnderReview,
            "em revisÃ£o" => BenefitDomainConstants.Status.UnderReview,
            "under_review" => BenefitDomainConstants.Status.UnderReview,
            "ativo" => BenefitDomainConstants.Status.Active,
            "active" => BenefitDomainConstants.Status.Active,
            "inativo" => BenefitDomainConstants.Status.Inactive,
            "inactive" => BenefitDomainConstants.Status.Inactive,
            "aprovado" => BenefitDomainConstants.Status.Approved,
            "approved" => BenefitDomainConstants.Status.Approved,
            "reprovado" => BenefitDomainConstants.Status.Rejected,
            "rejected" => BenefitDomainConstants.Status.Rejected,
            "rascunho" => BenefitDomainConstants.Status.Draft,
            "draft" => BenefitDomainConstants.Status.Draft,
            _ => normalized
        };
    }

    public static string NormalizeEligibilityType(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return BenefitDomainConstants.EligibilityType.Open;

        var normalized = value.Trim().ToLowerInvariant();

        return normalized switch
        {
            "aberto" => BenefitDomainConstants.EligibilityType.Open,
            "open" => BenefitDomainConstants.EligibilityType.Open,
            "nivel" => BenefitDomainConstants.EligibilityType.Level,
            "nÃ­vel" => BenefitDomainConstants.EligibilityType.Level,
            "level" => BenefitDomainConstants.EligibilityType.Level,
            "comportamento" => BenefitDomainConstants.EligibilityType.Behavior,
            "behavior" => BenefitDomainConstants.EligibilityType.Behavior,
            "codigo" => BenefitDomainConstants.EligibilityType.Code,
            "cÃ³digo" => BenefitDomainConstants.EligibilityType.Code,
            "code" => BenefitDomainConstants.EligibilityType.Code,
            "hibrido" => BenefitDomainConstants.EligibilityType.Hybrid,
            "hÃ­brido" => BenefitDomainConstants.EligibilityType.Hybrid,
            "hybrid" => BenefitDomainConstants.EligibilityType.Hybrid,
            "custom" => BenefitDomainConstants.EligibilityType.Hybrid,
            "customizado" => BenefitDomainConstants.EligibilityType.Hybrid,
            _ => normalized
        };
    }

    public static string NormalizeRecurrenceType(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return BenefitDomainConstants.RecurrenceType.Unlimited;

        var normalized = value.Trim().ToLowerInvariant();

        return normalized switch
        {
            "periodic" => BenefitDomainConstants.RecurrenceType.Periodic,
            "periodico" => BenefitDomainConstants.RecurrenceType.Periodic,
            "periÃ³dico" => BenefitDomainConstants.RecurrenceType.Periodic,
            "unlimited" => BenefitDomainConstants.RecurrenceType.Unlimited,
            "ilimitado" => BenefitDomainConstants.RecurrenceType.Unlimited,
            "once_per_customer" => BenefitDomainConstants.RecurrenceType.OncePerCustomer,
            "uma_vez_por_cliente" => BenefitDomainConstants.RecurrenceType.OncePerCustomer,
            "first_use_only" => BenefitDomainConstants.RecurrenceType.OncePerCustomer,
            _ => normalized
        };
    }

    public static string NormalizeValidityType(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return BenefitDomainConstants.ValidityType.Continuous;

        var normalized = value.Trim().ToLowerInvariant();

        return normalized switch
        {
            "continuous" => BenefitDomainConstants.ValidityType.Continuous,
            "continuo" => BenefitDomainConstants.ValidityType.Continuous,
            "contÃ­nuo" => BenefitDomainConstants.ValidityType.Continuous,
            "fixed_period" => BenefitDomainConstants.ValidityType.FixedPeriod,
            "fixed" => BenefitDomainConstants.ValidityType.FixedPeriod,
            "periodo_fixo" => BenefitDomainConstants.ValidityType.FixedPeriod,
            "perÃ­odo_fixo" => BenefitDomainConstants.ValidityType.FixedPeriod,
            _ => normalized
        };
    }

    public static string NormalizeDirection(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return BenefitDomainConstants.Direction.PartnerToMatilha;

        var normalized = value.Trim().ToLowerInvariant();

        return normalized switch
        {
            "partner_to_matilha" => BenefitDomainConstants.Direction.PartnerToMatilha,
            "matilha_to_partner" => BenefitDomainConstants.Direction.MatilhaToPartner,
            _ => normalized
        };
    }

    public static string NormalizeTargetActorType(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return BenefitDomainConstants.TargetActorType.Client;

        var normalized = value.Trim().ToLowerInvariant();

        return normalized switch
        {
            "client" => BenefitDomainConstants.TargetActorType.Client,
            "cliente" => BenefitDomainConstants.TargetActorType.Client,
            "partner_customer" => BenefitDomainConstants.TargetActorType.PartnerCustomer,
            "cliente_parceiro" => BenefitDomainConstants.TargetActorType.PartnerCustomer,
            _ => normalized
        };
    }

    public static string JoinList(IEnumerable<string>? values)
        => values is null
            ? string.Empty
            : string.Join(',', values.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim().ToLowerInvariant()).Distinct());

    public static string? BuildEligibilitySummary(CreateBenefitRequest request)
    {
        return BuildEligibilitySummary(
            request.EligibilityType,
            request.AllowedLevels,
            request.MinFrequencyEnabled,
            request.MinFrequencyValue,
            request.MinTicketEnabled,
            request.MinTicketValue,
            request.FirstUseOnly,
            request.RequiresAccessCode,
            request.CustomRuleText,
            request.EligibilitySummary);
    }

    public static string? BuildEligibilitySummary(UpdateBenefitRequest request)
    {
        return BuildEligibilitySummary(
            request.EligibilityType,
            request.AllowedLevels,
            request.MinFrequencyEnabled,
            request.MinFrequencyValue,
            request.MinTicketEnabled,
            request.MinTicketValue,
            request.FirstUseOnly,
            request.RequiresAccessCode,
            request.CustomRuleText,
            request.EligibilitySummary);
    }

    private static string? BuildEligibilitySummary(
        string? eligibilityType,
        IEnumerable<string>? allowedLevels,
        bool minFrequencyEnabled,
        int? minFrequencyValue,
        bool minTicketEnabled,
        decimal? minTicketValue,
        bool firstUseOnly,
        bool requiresAccessCode,
        string? customRuleText,
        string? fallback)
    {
        var parts = new List<string>();
        var normalizedEligibility = NormalizeEligibilityType(eligibilityType);

        if (normalizedEligibility == BenefitDomainConstants.EligibilityType.Level && allowedLevels is not null && allowedLevels.Any())
            parts.Add($"NÃ­veis: {string.Join(", ", allowedLevels)}");

        if (minFrequencyEnabled && minFrequencyValue.HasValue)
            parts.Add($"FrequÃªncia mÃ­nima: {minFrequencyValue.Value}");

        if (minTicketEnabled && minTicketValue.HasValue)
            parts.Add($"Ticket mÃ­nimo: {minTicketValue.Value:0.##}");

        if (firstUseOnly)
            parts.Add("Apenas primeiro uso");

        if (requiresAccessCode)
            parts.Add("Exige cÃ³digo");

        if (!string.IsNullOrWhiteSpace(customRuleText))
            parts.Add(customRuleText.Trim());

        if (parts.Count == 0)
            return fallback;

        return string.Join(" | ", parts);
    }

    public static BenefitLevelScopeDto BuildLevelScope(string? levelType, IEnumerable<string>? allowedLevels)
    {
        return new BenefitLevelScopeDto
        {
            LevelType = string.IsNullOrWhiteSpace(levelType) ? null : levelType,
            LevelCode = JoinList(allowedLevels)
        };
    }

    public static BenefitBehaviorRulesDto BuildBehaviorRules(
        bool minFrequencyEnabled,
        int? minFrequencyValue,
        int? frequencyWindowMonths,
        bool minTicketEnabled,
        decimal? minTicketValue,
        int? ticketWindowMonths,
        bool firstUseOnly,
        bool requiresMatilhaApproval,
        string? customRuleText)
    {
        return new BenefitBehaviorRulesDto
        {
            MinFrequencyEnabled = minFrequencyEnabled,
            MinFrequencyValue = minFrequencyValue,
            FrequencyWindowMonths = frequencyWindowMonths,
            MinTicketEnabled = minTicketEnabled,
            MinTicketValue = minTicketValue,
            TicketWindowMonths = ticketWindowMonths,
            FirstUseOnly = firstUseOnly,
            RequiresMatilhaApproval = requiresMatilhaApproval,
            CustomRuleText = customRuleText
        };
    }

    public static BenefitCodeRulesDto BuildCodeRules(
        bool requiresAccessCode,
        bool allowAnyActivePartnerCode,
        Guid? specificAccessCodeId,
        string? codeValidationMode)
    {
        return new BenefitCodeRulesDto
        {
            RequiresAccessCode = requiresAccessCode,
            AllowAnyActivePartnerCode = allowAnyActivePartnerCode,
            SpecificAccessCodeId = specificAccessCodeId,
            CodeValidationMode = codeValidationMode
        };
    }
}