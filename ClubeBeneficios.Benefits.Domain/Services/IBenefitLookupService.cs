using ClubeBeneficios.Benefits.Domain.Dtos;

namespace ClubeBeneficios.Benefits.Domain.Services;

public interface IBenefitLookupService
{
    Task<BenefitLookupOptionsDto> GetAdminOptionsAsync(Guid? partnerId = null, CancellationToken cancellationToken = default);
    Task<BenefitLookupOptionsDto> GetPartnerOptionsAsync(CancellationToken cancellationToken = default);
}