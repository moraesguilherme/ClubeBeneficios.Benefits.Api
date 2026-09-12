using ClubeBeneficios.Benefits.Domain.Dtos.Common;
using ClubeBeneficios.Benefits.Domain.Dtos.Customer;
using ClubeBeneficios.Benefits.Domain.Dtos.Requests.BenefitRequests;
using ClubeBeneficios.Benefits.Domain.Repositories;
using ClubeBeneficios.Benefits.Domain.Security;
using ClubeBeneficios.Benefits.Domain.Services;

namespace ClubeBeneficios.Benefits.Infrastructure.Services;

public sealed class CustomerBenefitService : ICustomerBenefitService
{
    private readonly ICustomerBenefitRepository _repository;
    private readonly IBenefitRequestService _benefitRequestService;
    private readonly ICurrentUser _currentUser;

    public CustomerBenefitService(
        ICustomerBenefitRepository repository,
        IBenefitRequestService benefitRequestService,
        ICurrentUser currentUser)
    {
        _repository = repository;
        _benefitRequestService = benefitRequestService;
        _currentUser = currentUser;
    }

    public Task<PagedResultDto<CustomerBenefitListItemDto>> GetAvailableAsync(
        int page = 1,
        int pageSize = 12,
        string? search = null,
        CancellationToken cancellationToken = default)
    {
        page = page <= 0 ? 1 : page;
        pageSize = pageSize <= 0 ? 12 : pageSize;

        return _repository.GetAvailableAsync(
            page,
            pageSize,
            search,
            cancellationToken);
    }

    public Task<CustomerBenefitDetailDto?> GetByIdAsync(
        Guid benefitId,
        CancellationToken cancellationToken = default)
    {
        return _repository.GetByIdAsync(benefitId, cancellationToken);
    }

    public async Task<Guid> CreateRequestAsync(
        Guid benefitId,
        CreateCustomerBenefitRequestDto request,
        CancellationToken cancellationToken = default)
    {
        if (_currentUser.UserId is null)
        {
            throw new UnauthorizedAccessException("Usuário autenticado inválido ou não informado.");
        }

        var userId = _currentUser.UserId.Value;
        var clientId = await ResolveClientIdAsync(cancellationToken);

        var benefit = await _repository.GetByIdAsync(
            benefitId,
            cancellationToken);

        if (benefit is null)
        {
            throw new KeyNotFoundException("Benefício não encontrado ou indisponível para cliente Matilha.");
        }

        if (benefit.UsageScope == "customer_pet" && !request.PetId.HasValue)
        {
            throw new InvalidOperationException("Selecione o pet para solicitar este benefício.");
        }

        if (request.PetId.HasValue)
        {
            var petBelongsToClient = await _repository.ClientPetBelongsToClientAsync(
                clientId,
                request.PetId.Value,
                cancellationToken);

            if (!petBelongsToClient)
            {
                throw new UnauthorizedAccessException("O pet selecionado não pertence ao cliente autenticado.");
            }
        }

        var createRequest = new CreateBenefitUsageRequest
        {
            BenefitId = benefitId,
            RequesterType = "client",
            RequesterUserId = userId,
            RequesterClientId = clientId,
            RequestedByUserId = userId,

            PetSourceType = request.PetId.HasValue ? "client_pet" : null,
            RequesterClientPetId = request.PetId,

            ScheduledFor = request.ScheduledFor,

            ReviewRequired =
                benefit.RequiresManualRelease ||
                benefit.RequiresMatilhaAcceptanceRules ||
                benefit.RequiresPartnerAvailability
        };

        return await _benefitRequestService.CreateAsync(
            createRequest,
            cancellationToken);
    }

    public async Task<PagedResultDto<CustomerBenefitRequestListItemDto>> GetMyRequestsAsync(
        int page = 1,
        int pageSize = 12,
        string? status = null,
        CancellationToken cancellationToken = default)
    {
        var clientId = await ResolveClientIdAsync(cancellationToken);

        return await _repository.GetRequestsByClientAsync(
            clientId,
            page,
            pageSize,
            status,
            cancellationToken);
    }

    public async Task<PagedResultDto<CustomerBenefitUsageListItemDto>> GetMyUsagesAsync(
        int page = 1,
        int pageSize = 12,
        CancellationToken cancellationToken = default)
    {
        var clientId = await ResolveClientIdAsync(cancellationToken);

        return await _repository.GetUsagesByClientAsync(
            clientId,
            page,
            pageSize,
            cancellationToken);
    }

    private async Task<Guid> ResolveClientIdAsync(CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is null)
        {
            throw new UnauthorizedAccessException("Usuário autenticado inválido ou não informado.");
        }

        var clientId = await _repository.GetClientIdByUserIdAsync(
            _currentUser.UserId.Value,
            cancellationToken);

        if (!clientId.HasValue)
        {
            throw new InvalidOperationException("Não foi encontrado cliente Matilha vinculado ao usuário autenticado.");
        }

        return clientId.Value;
    }

    public async Task<CustomerBenefitRequestDetailDto?> GetMyRequestByIdAsync(
        Guid requestId,
        CancellationToken cancellationToken = default)
    {
        var clientId = await ResolveClientIdAsync(cancellationToken);

        return await _repository.GetRequestByIdForClientAsync(
            clientId,
            requestId,
            cancellationToken);
    }

    public async Task<CustomerBenefitUsageDetailDto?> GetMyUsageByIdAsync(
        Guid usageId,
        CancellationToken cancellationToken = default)
    {
        var clientId = await ResolveClientIdAsync(cancellationToken);

        return await _repository.GetUsageByIdForClientAsync(
            clientId,
            usageId,
            cancellationToken);
    }
}