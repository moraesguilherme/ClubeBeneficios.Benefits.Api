using ClubeBeneficios.Benefits.Domain.Dtos;
using ClubeBeneficios.Benefits.Domain.Dtos.Requests;

namespace ClubeBeneficios.Benefits.Domain.Services;

public interface IBenefitUsageService
{
    Task<Guid> ConfirmAdminAsync(ConfirmBenefitUsageRequest request, CancellationToken cancellationToken = default);
    Task<Guid> ConfirmPartnerAsync(ConfirmBenefitUsageRequest request, CancellationToken cancellationToken = default);
    Task CancelAsync(Guid usageId, CancelBenefitUsageRequest request, CancellationToken cancellationToken = default);
    Task<BenefitUsageDetailDto?> GetByIdAsync(Guid usageId, CancellationToken cancellationToken = default);
    Task<PagedResultDto<BenefitUsageListItemDto>> SearchAdminAsync(BenefitUsageFilterDto filter, CancellationToken cancellationToken = default);
    Task<PagedResultDto<BenefitUsageListItemDto>> SearchPartnerAsync(BenefitUsageFilterDto filter, CancellationToken cancellationToken = default);
    Task<BenefitEligibilityValidationResultDto?> ValidateAsync(ValidateBenefitUsageRequest request, CancellationToken cancellationToken = default);
}