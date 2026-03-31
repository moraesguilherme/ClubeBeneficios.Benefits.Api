using ClubeBeneficios.Benefits.Domain.Dtos;

namespace ClubeBeneficios.Benefits.Domain.Repositories;

public interface IBenefitLookupRepository
{
    Task<BenefitLookupOptionsDto> GetOptionsAsync(Guid? partnerId = null, CancellationToken cancellationToken = default);
}