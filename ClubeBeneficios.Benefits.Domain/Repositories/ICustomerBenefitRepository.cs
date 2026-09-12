using ClubeBeneficios.Benefits.Domain.Dtos.Common;
using ClubeBeneficios.Benefits.Domain.Dtos.Customer;

namespace ClubeBeneficios.Benefits.Domain.Repositories;

public interface ICustomerBenefitRepository
{
    Task<Guid?> GetClientIdByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<bool> ClientPetBelongsToClientAsync(
        Guid clientId,
        Guid petId,
        CancellationToken cancellationToken = default);

    Task<PagedResultDto<CustomerBenefitListItemDto>> GetAvailableAsync(
        int page,
        int pageSize,
        string? search,
        CancellationToken cancellationToken = default);

    Task<CustomerBenefitDetailDto?> GetByIdAsync(
        Guid benefitId,
        CancellationToken cancellationToken = default);

    Task<PagedResultDto<CustomerBenefitRequestListItemDto>> GetRequestsByClientAsync(
        Guid clientId,
        int page,
        int pageSize,
        string? status,
        CancellationToken cancellationToken = default);

    Task<PagedResultDto<CustomerBenefitUsageListItemDto>> GetUsagesByClientAsync(
        Guid clientId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<CustomerBenefitRequestDetailDto?> GetRequestByIdForClientAsync(
        Guid clientId,
        Guid requestId,
        CancellationToken cancellationToken = default);

    Task<CustomerBenefitUsageDetailDto?> GetUsageByIdForClientAsync(
        Guid clientId,
        Guid usageId,
        CancellationToken cancellationToken = default);
}