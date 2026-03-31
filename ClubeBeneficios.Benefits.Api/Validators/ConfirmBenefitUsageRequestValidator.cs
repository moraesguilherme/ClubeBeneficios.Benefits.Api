using FluentValidation;
using ClubeBeneficios.Benefits.Domain.Dtos.Requests;

namespace ClubeBeneficios.Benefits.Api.Validators;

public class ConfirmBenefitUsageRequestValidator : AbstractValidator<ConfirmBenefitUsageRequest>
{
    public ConfirmBenefitUsageRequestValidator()
    {
        RuleFor(x => x.BenefitId).NotEmpty();
        RuleFor(x => x.UsedByType).NotEmpty().MaximumLength(50);
        RuleFor(x => x.SnapshotTitle).MaximumLength(200).When(x => !string.IsNullOrWhiteSpace(x.SnapshotTitle));
        RuleFor(x => x.SnapshotPartnerName).MaximumLength(200).When(x => !string.IsNullOrWhiteSpace(x.SnapshotPartnerName));
        RuleFor(x => x.SnapshotRuleSummary).MaximumLength(1000).When(x => !string.IsNullOrWhiteSpace(x.SnapshotRuleSummary));
    }
}