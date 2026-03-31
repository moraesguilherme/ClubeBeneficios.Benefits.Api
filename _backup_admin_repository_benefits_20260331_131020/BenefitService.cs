using ClubeBeneficios.Benefits.Domain.Exceptions;
using ClubeBeneficios.Benefits.Domain.Dtos;
using ClubeBeneficios.Benefits.Domain.Dtos.Filters;
using ClubeBeneficios.Benefits.Domain.Dtos.Requests;
using ClubeBeneficios.Benefits.Domain.Repositories;
using ClubeBeneficios.Benefits.Domain.Security;
using ClubeBeneficios.Benefits.Domain.Services;

namespace ClubeBeneficios.Benefits.Infrastructure.Services;

public class BenefitService : IBenefitService
{
    private readonly IBenefitRepository _repository;
    private readonly ICurrentUser _currentUser;

    public BenefitService(IBenefitRepository repository, ICurrentUser currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public Task<PagedResultDto<BenefitListItemDto>> GetAdminPagedAsync(BenefitFilterDto filter, CancellationToken cancellationToken = default)
        => _repository.GetPagedAsync(filter, cancellationToken);

    public Task<PagedResultDto<BenefitListItemDto>> GetPartnerPagedAsync(BenefitFilterDto filter, CancellationToken cancellationToken = default)
    {
        filter.PartnerId = GetRequiredPartnerId();
        return _repository.GetPagedAsync(filter, cancellationToken);
    }

    public Task<PagedResultDto<BenefitApprovalQueueItemDto>> GetApprovalQueueAsync(BenefitFilterDto filter, CancellationToken cancellationToken = default)
        => _repository.GetApprovalQueueAsync(filter, cancellationToken);

    public Task<BenefitSummaryDto> GetAdminSummaryAsync(CancellationToken cancellationToken = default)
        => _repository.GetSummaryAsync(null, cancellationToken);

    public Task<BenefitSummaryDto> GetPartnerSummaryAsync(CancellationToken cancellationToken = default)
        => _repository.GetSummaryAsync(GetRequiredPartnerId(), cancellationToken);

    public Task<BenefitFilterOptionsDto> GetAdminFilterOptionsAsync(CancellationToken cancellationToken = default)
        => _repository.GetFilterOptionsAsync(null, cancellationToken);

    public Task<BenefitFilterOptionsDto> GetPartnerFilterOptionsAsync(CancellationToken cancellationToken = default)
        => _repository.GetFilterOptionsAsync(GetRequiredPartnerId(), cancellationToken);

    public Task<BenefitDetailsDto?> GetAdminByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => _repository.GetByIdAsync(id, null, false, cancellationToken);

    public Task<BenefitDetailsDto?> GetPartnerByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => _repository.GetByIdAsync(id, GetRequiredPartnerId(), true, cancellationToken);

    public Task<Guid> CreateAdminAsync(CreateBenefitRequest request, CancellationToken cancellationToken = default)
        => _repository.CreateAsync(request, GetRequiredUserId(), null, false, cancellationToken);

    public Task<Guid> CreatePartnerAsync(CreateBenefitRequest request, CancellationToken cancellationToken = default)
        => _repository.CreateAsync(request, GetRequiredUserId(), GetRequiredPartnerId(), true, cancellationToken);

    public Task UpdateAdminAsync(Guid id, UpdateBenefitRequest request, CancellationToken cancellationToken = default)
        => _repository.UpdateAsync(id, request, GetRequiredUserId(), null, false, cancellationToken);

    public Task UpdatePartnerAsync(Guid id, UpdateBenefitRequest request, CancellationToken cancellationToken = default)
        => _repository.UpdateAsync(id, request, GetRequiredUserId(), GetRequiredPartnerId(), true, cancellationToken);

    public Task ChangeAdminStatusAsync(Guid id, ChangeBenefitStatusRequest request, CancellationToken cancellationToken = default)
        => _repository.ChangeStatusAsync(id, request, GetRequiredUserId(), null, false, cancellationToken);

    public Task ChangePartnerStatusAsync(Guid id, ChangeBenefitStatusRequest request, CancellationToken cancellationToken = default)
        => _repository.ChangeStatusAsync(id, request, GetRequiredUserId(), GetRequiredPartnerId(), true, cancellationToken);

    public Task AddAdminReviewAsync(Guid id, ReviewBenefitRequest request, CancellationToken cancellationToken = default)
        => _repository.AddReviewAsync(id, request, GetRequiredUserId(), cancellationToken);

    private Guid GetRequiredUserId()
    {
        return _currentUser.UserId ?? throw new InvalidOperationException("UsuÃƒÆ’Ã‚Â¡rio autenticado nÃƒÆ’Ã‚Â£o encontrado no token.");
    }

    private Guid GetRequiredPartnerId()
    {
        return _currentUser.PartnerId ?? throw new InvalidOperationException("PartnerId nÃƒÆ’Ã‚Â£o encontrado no token.");
    }

    private void EnsurePartnerOwnership(Guid? partnerId)
    {
        if (_currentUser.Role == "partner")
        {
            if (!_currentUser.PartnerId.HasValue)
            {
                throw new ForbiddenException("NÃ£o foi possÃ­vel identificar o parceiro autenticado.");
            }

            if (partnerId.HasValue && partnerId.Value != _currentUser.PartnerId.Value)
            {
                throw new ForbiddenException("O parceiro autenticado nÃ£o pode operar benefÃ­cios de outro parceiro.");
            }
        }
    }
}
