using ClubeBeneficios.Benefits.Domain.Dtos.Lookups;

namespace ClubeBeneficios.Benefits.Domain.Repositories;

public interface IBenefitLookupRepository
{
    Task<BenefitLookupOptionsDto> GetOptionsAsync(Guid? partnerId = null, CancellationToken cancellationToken = default);
}