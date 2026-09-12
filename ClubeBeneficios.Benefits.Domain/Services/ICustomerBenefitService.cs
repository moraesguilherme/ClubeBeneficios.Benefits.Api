using ClubeBeneficios.Benefits.Domain.Dtos.Common;
using ClubeBeneficios.Benefits.Domain.Dtos.Customer;

namespace ClubeBeneficios.Benefits.Domain.Services;

public interface ICustomerBenefitService
{
    Task<PagedResultDto<CustomerBenefitListItemDto>> GetAvailableAsync(
        int page = 1,
        int pageSize = 12,
        string? search = null,
        CancellationToken cancellationToken = default);

    Task<CustomerBenefitDetailDto?> GetByIdAsync(
        Guid benefitId,
        CancellationToken cancellationToken = default);

    Task<Guid> CreateRequestAsync(
        Guid benefitId,
        CreateCustomerBenefitRequestDto request,
        CancellationToken cancellationToken = default);

    Task<PagedResultDto<CustomerBenefitRequestListItemDto>> GetMyRequestsAsync(
        int page = 1,
        int pageSize = 12,
        string? status = null,
        CancellationToken cancellationToken = default);

    Task<PagedResultDto<CustomerBenefitUsageListItemDto>> GetMyUsagesAsync(
        int page = 1,
        int pageSize = 12,
        CancellationToken cancellationToken = default);

    Task<CustomerBenefitRequestDetailDto?> GetMyRequestByIdAsync(
        Guid requestId,
        CancellationToken cancellationToken = default);

    Task<CustomerBenefitUsageDetailDto?> GetMyUsageByIdAsync(
        Guid usageId,
        CancellationToken cancellationToken = default);
}