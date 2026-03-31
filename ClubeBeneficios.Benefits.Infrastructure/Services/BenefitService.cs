using System;
using System.Threading;
using System.Threading.Tasks;
using ClubeBeneficios.Benefits.Domain.Dtos;
using ClubeBeneficios.Benefits.Domain.Dtos.Filters;
using ClubeBeneficios.Benefits.Domain.Dtos.Requests;
using ClubeBeneficios.Benefits.Domain.Repositories;
using ClubeBeneficios.Benefits.Domain.Services;

namespace ClubeBeneficios.Benefits.Infrastructure.Services;

public class BenefitService : IBenefitService
{
    private readonly IBenefitRepository _benefitRepository;

    public BenefitService(IBenefitRepository benefitRepository)
    {
        _benefitRepository = benefitRepository;
    }

    public Task<PagedResultDto<BenefitListItemDto>> GetPagedAsync(
        BenefitFilterDto filter,
        CancellationToken cancellationToken = default)
        => _benefitRepository.GetPagedAsync(filter, cancellationToken);

    public Task<PagedResultDto<BenefitApprovalItemDto>> GetPendingAsync(
        BenefitFilterDto filter,
        CancellationToken cancellationToken = default)
        => _benefitRepository.GetPendingAsync(filter, cancellationToken);

    public Task<BenefitDashboardSummaryDto> GetDashboardSummaryAsync(
        CancellationToken cancellationToken = default)
        => _benefitRepository.GetDashboardSummaryAsync(cancellationToken);

    public Task<BenefitFilterOptionsDto> GetFilterOptionsAsync(
        CancellationToken cancellationToken = default)
        => _benefitRepository.GetFilterOptionsAsync(cancellationToken);

    public Task<BenefitDetailsDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
        => _benefitRepository.GetByIdAsync(id, cancellationToken);

    public Task<Guid> CreateAsync(
        CreateBenefitRequest request,
        CancellationToken cancellationToken = default)
        => _benefitRepository.CreateAsync(request, cancellationToken);

    public Task<bool> UpdateAsync(
        Guid id,
        UpdateBenefitRequest request,
        CancellationToken cancellationToken = default)
        => _benefitRepository.UpdateAsync(id, request, cancellationToken);

    public Task<bool> ChangeStatusAsync(
        Guid id,
        ChangeBenefitStatusRequest request,
        CancellationToken cancellationToken = default)
        => _benefitRepository.ChangeStatusAsync(id, request, cancellationToken);

    public Task<bool> AddReviewAsync(
        Guid id,
        AddBenefitReviewRequest request,
        CancellationToken cancellationToken = default)
        => _benefitRepository.AddReviewAsync(id, request, cancellationToken);
}