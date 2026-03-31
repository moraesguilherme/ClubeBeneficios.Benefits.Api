using ClubeBeneficios.Benefits.Domain.Dtos;
using ClubeBeneficios.Benefits.Domain.Dtos.Requests;

namespace ClubeBeneficios.Benefits.Domain.Repositories;

public interface IBenefitUsageRepository
{
    Task<Guid> ConfirmAsync(ConfirmBenefitUsageRequest request, Guid? performedByUserId, bool performedByPartner, CancellationToken cancellationToken = default);
    Task CancelAsync(Guid usageId, CancelBenefitUsageRequest request, Guid? performedByUserId, CancellationToken cancellationToken = default);
    Task<BenefitUsageDetailDto?> GetByIdAsync(Guid usageId, CancellationToken cancellationToken = default);
    Task<PagedResultDto<BenefitUsageListItemDto>> SearchAsync(BenefitUsageFilterDto filter, CancellationToken cancellationToken = default);
    Task<BenefitEligibilityValidationResultDto?> ValidateAsync(ValidateBenefitUsageRequest request, CancellationToken cancellationToken = default);
}