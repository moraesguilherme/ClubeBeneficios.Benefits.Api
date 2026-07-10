using ClubeBeneficios.Benefits.Domain.Dtos.Lookups;

namespace ClubeBeneficios.Benefits.Domain.Services;

public interface IBenefitLookupService
{
    Task<BenefitLookupOptionsDto> GetAdminOptionsAsync(Guid? partnerId = null, CancellationToken cancellationToken = default);
    Task<BenefitLookupOptionsDto> GetPartnerOptionsAsync(CancellationToken cancellationToken = default);
}