using ClubeBeneficios.Benefits.Domain.Dtos;
using ClubeBeneficios.Benefits.Domain.Dtos.Requests;
using ClubeBeneficios.Benefits.Domain.Repositories;
using ClubeBeneficios.Benefits.Domain.Security;
using ClubeBeneficios.Benefits.Domain.Services;
using Microsoft.Extensions.Configuration;

namespace ClubeBeneficios.Benefits.Infrastructure.Services;

public class BenefitRequestService : IBenefitRequestService
{
    private readonly IBenefitRequestRepository _repository;
    private readonly ICurrentUser _currentUser;
    private readonly IConfirmationTokenService _confirmationTokenService;
    private readonly IConfiguration _configuration;

    public BenefitRequestService(
        IBenefitRequestRepository repository,
        ICurrentUser currentUser,
        IConfirmationTokenService confirmationTokenService,
        IConfiguration configuration)
    {
        _repository = repository;
        _currentUser = currentUser;
        _confirmationTokenService = confirmationTokenService;
        _configuration = configuration;
    }

    public Task<Guid> CreateAsync(
    CreateBenefitRequestDto request,
    CancellationToken cancellationToken = default)
    => _repository.CreateAsync(request, _currentUser.UserId, cancellationToken);

    public Task SubmitHealthAsync(
        Guid requestId,
        SubmitBenefitRequestHealthRequest request,
        CancellationToken cancellationToken = default)
        => _repository.SubmitHealthAsync(requestId, request, cancellationToken);

    public Task AddReviewAsync(
        Guid requestId,
        AddBenefitRequestReviewRequest request,
        CancellationToken cancellationToken = default)
    {
        request.ReviewedByUserId = _currentUser.UserId;
        return _repository.AddReviewAsync(requestId, request, _currentUser.UserId, cancellationToken);
    }

    public Task ApproveAsync(
        Guid requestId,
        AddBenefitRequestReviewRequest request,
        CancellationToken cancellationToken = default)
    {
        request.ReviewStatus = "approved";
        request.ReviewedByUserId = _currentUser.UserId;

        return _repository.AddReviewAsync(requestId, request, _currentUser.UserId, cancellationToken);
    }

    public Task RequestChangesAsync(
        Guid requestId,
        AddBenefitRequestReviewRequest request,
        CancellationToken cancellationToken = default)
    {
        request.ReviewStatus = "under_review";
        request.ReviewedByUserId = _currentUser.UserId;

        if (string.IsNullOrWhiteSpace(request.ReviewPoint))
        {
            request.ReviewPoint = "health_documents";
        }

        return _repository.AddReviewAsync(requestId, request, _currentUser.UserId, cancellationToken);
    }

    public Task RejectAsync(
        Guid requestId,
        AddBenefitRequestReviewRequest request,
        CancellationToken cancellationToken = default)
    {
        request.ReviewStatus = "rejected";
        request.ReviewedByUserId = _currentUser.UserId;

        return _repository.AddReviewAsync(requestId, request, _currentUser.UserId, cancellationToken);
    }

    public Task ChangeStatusAsync(
        Guid requestId,
        ChangeBenefitRequestStatusRequest request,
        CancellationToken cancellationToken = default)
        => _repository.ChangeStatusAsync(requestId, request, _currentUser.UserId, cancellationToken);

    public Task<BenefitRequestDetailDto?> GetByIdAsync(
        Guid requestId,
        CancellationToken cancellationToken = default)
        => _repository.GetByIdAsync(requestId, cancellationToken);

    public Task<PagedResultDto<BenefitRequestListItemDto>> SearchAdminAsync(
        BenefitRequestFilterDto filter,
        CancellationToken cancellationToken = default)
        => _repository.SearchAsync(filter, cancellationToken);

    public Task<PagedResultDto<BenefitRequestListItemDto>> SearchPartnerAsync(
        BenefitRequestFilterDto filter,
        CancellationToken cancellationToken = default)
    {
        filter.PartnerId = _currentUser.PartnerId;
        return _repository.SearchAsync(filter, cancellationToken);
    }

    public Task<PagedResultDto<BenefitRequestListItemDto>> SearchPendingReviewAsync(
        BenefitRequestFilterDto filter,
        CancellationToken cancellationToken = default)
        => _repository.SearchPendingReviewAsync(filter, cancellationToken);

    public Task<BenefitRequestApprovalSummaryDto> GetApprovalSummaryAsync(
        CancellationToken cancellationToken = default)
        => _repository.GetApprovalSummaryAsync(cancellationToken);

    public async Task<BenefitUsageConfirmationPairResultDto> CreateUsageConfirmationPairAsync(
    Guid requestId,
    CreateBenefitUsageConfirmationPairRequest request,
    CancellationToken cancellationToken = default)
    {
        if (request is null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        if (string.IsNullOrWhiteSpace(request.PartnerRecipientEmail))
        {
            throw new ArgumentException("E-mail do parceiro é obrigatório.", nameof(request));
        }

        var expirationHours = request.ExpirationHours <= 0
            ? 72
            : request.ExpirationHours;

        var publicBaseUrl = _configuration["BenefitUsageConfirmations:PublicBaseUrl"];

        if (string.IsNullOrWhiteSpace(publicBaseUrl))
        {
            throw new InvalidOperationException(
                "Configuração BenefitUsageConfirmations:PublicBaseUrl não foi informada.");
        }

        var clientToken = _confirmationTokenService.GenerateToken();
        var partnerToken = _confirmationTokenService.GenerateToken();

        var clientTokenHash = _confirmationTokenService.ComputeHash(clientToken);
        var partnerTokenHash = _confirmationTokenService.ComputeHash(partnerToken);

        var baseUrl = publicBaseUrl.TrimEnd('/');

        var clientConfirmationUrl =
            $"{baseUrl}/confirmar-uso?token={Uri.EscapeDataString(clientToken)}";

        var partnerConfirmationUrl =
            $"{baseUrl}/confirmar-uso?token={Uri.EscapeDataString(partnerToken)}";

        var confirmationExpiresAt = DateTime.UtcNow.AddHours(expirationHours);

        return await _repository.CreateUsageConfirmationPairAsync(
            requestId,
            clientTokenHash,
            partnerTokenHash,
            clientConfirmationUrl,
            partnerConfirmationUrl,
            confirmationExpiresAt,
            request.PartnerRecipientEmail,
            request.PartnerRecipientName,
            _currentUser.UserId,
            cancellationToken);
    }

    public async Task<BenefitUsageConfirmationTokenDto?> GetUsageConfirmationByTokenAsync(
        string token,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            throw new ArgumentException("Token é obrigatório.", nameof(token));
        }

        var tokenHash = _confirmationTokenService.ComputeHash(token);

        return await _repository.GetUsageConfirmationByTokenHashAsync(
            tokenHash,
            cancellationToken);
    }

    public async Task<BenefitUsageConfirmationConfirmResultDto> ConfirmUsageConfirmationAsync(
        string token,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            throw new ArgumentException("Token é obrigatório.", nameof(token));
        }

        var tokenHash = _confirmationTokenService.ComputeHash(token);

        return await _repository.ConfirmUsageConfirmationAsync(
            tokenHash,
            cancellationToken);
    }
}