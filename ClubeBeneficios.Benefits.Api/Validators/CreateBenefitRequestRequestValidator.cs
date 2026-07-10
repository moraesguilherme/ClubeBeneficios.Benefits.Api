using FluentValidation;
using ClubeBeneficios.Benefits.Domain.Dtos.Requests.BenefitRequests;

namespace ClubeBeneficios.Benefits.Api.Validators;

public class CreateBenefitRequestDtoValidator : AbstractValidator<CreateBenefitRequestDto>
{
    public CreateBenefitRequestDtoValidator()
    {
        RuleFor(x => x.BenefitId)
            .NotEmpty();

        RuleFor(x => x.RequesterType)
            .NotEmpty()
            .MaximumLength(30)
            .Must(value =>
                value == "client" ||
                value == "partner_customer")
            .WithMessage("RequesterType deve ser 'client' ou 'partner_customer'.");

        RuleFor(x => x)
            .Must(x =>
                x.RequesterType != "client" ||
                x.RequesterClientId is not null ||
                x.RequesterUserId is not null)
            .WithMessage("Solicitação de cliente exige RequesterClientId ou RequesterUserId.");

        RuleFor(x => x)
            .Must(x =>
                x.RequesterType != "partner_customer" ||
                x.RequesterPartnerCustomerId is not null)
            .WithMessage("Solicitação de cliente parceiro exige RequesterPartnerCustomerId.");

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
                x.RequesterClientPetId is not null)
            .WithMessage("PetSourceType client_pet exige RequesterClientPetId.");

        RuleFor(x => x)
            .Must(x =>
                x.PetSourceType != "partner_customer_pet" ||
                x.RequesterPartnerCustomerPetId is not null)
            .WithMessage("PetSourceType partner_customer_pet exige RequesterPartnerCustomerPetId.");

        RuleFor(x => x)
            .Must(x =>
                x.ExpiresAt is null ||
                x.ScheduledFor is null ||
                x.ExpiresAt >= x.ScheduledFor)
            .WithMessage("ExpiresAt não pode ser anterior a ScheduledFor.");
    }
}