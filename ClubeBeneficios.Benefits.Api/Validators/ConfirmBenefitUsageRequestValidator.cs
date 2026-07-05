using FluentValidation;
using ClubeBeneficios.Benefits.Domain.Dtos.Requests;

namespace ClubeBeneficios.Benefits.Api.Validators;

public class ConfirmBenefitUsageRequestValidator : AbstractValidator<ConfirmBenefitUsageRequest>
{
    public ConfirmBenefitUsageRequestValidator()
    {
        RuleFor(x => x.BenefitId)
            .NotEmpty();

        RuleFor(x => x.BenefitRequestId)
            .NotEmpty()
            .When(x =>
                string.IsNullOrWhiteSpace(x.UsedByType) &&
                x.UsedByUserId is null &&
                x.UsedByClientId is null &&
                x.UsedByPartnerCustomerId is null);

        RuleFor(x => x.UsedByType)
            .MaximumLength(30)
            .Must(value =>
                string.IsNullOrWhiteSpace(value) ||
                value == "client" ||
                value == "partner_customer")
            .WithMessage("UsedByType deve ser 'client' ou 'partner_customer'.");

        RuleFor(x => x.UsedByType)
            .NotEmpty()
            .When(x => x.BenefitRequestId is null);

        RuleFor(x => x)
            .Must(x =>
                x.BenefitRequestId is not null ||
                x.UsedByType != "client" ||
                x.UsedByClientId is not null ||
                x.UsedByUserId is not null)
            .WithMessage("Uso de cliente exige UsedByClientId ou UsedByUserId quando não houver BenefitRequestId.");

        RuleFor(x => x)
            .Must(x =>
                x.BenefitRequestId is not null ||
                x.UsedByType != "partner_customer" ||
                x.UsedByPartnerCustomerId is not null)
            .WithMessage("Uso de cliente parceiro exige UsedByPartnerCustomerId quando não houver BenefitRequestId.");

        RuleFor(x => x.PetSourceType)
            .MaximumLength(30)
            .Must(value =>
                string.IsNullOrWhiteSpace(value) ||
                value == "client_pet" ||
                value == "partner_customer_pet")
            .WithMessage("PetSourceType deve ser 'client_pet' ou 'partner_customer_pet'.");

        RuleFor(x => x)
            .Must(x =>
                x.PetSourceType != "client_pet" ||
                x.ClientPetId is not null)
            .WithMessage("PetSourceType client_pet exige ClientPetId.");

        RuleFor(x => x)
            .Must(x =>
                x.PetSourceType != "partner_customer_pet" ||
                x.PartnerCustomerPetId is not null)
            .WithMessage("PetSourceType partner_customer_pet exige PartnerCustomerPetId.");

        RuleFor(x => x.MonetaryValue)
            .GreaterThanOrEqualTo(0)
            .When(x => x.MonetaryValue.HasValue);

        RuleFor(x => x.DiscountValue)
            .GreaterThanOrEqualTo(0)
            .When(x => x.DiscountValue.HasValue);

        RuleFor(x => x.RuleSummary)
            .MaximumLength(1000)
            .When(x => !string.IsNullOrWhiteSpace(x.RuleSummary));
    }
}