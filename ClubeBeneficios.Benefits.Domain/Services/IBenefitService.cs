using ClubeBeneficios.Benefits.Domain.Dtos;
using ClubeBeneficios.Benefits.Domain.Dtos.Filters;
using ClubeBeneficios.Benefits.Domain.Dtos.Requests;

namespace ClubeBeneficios.Benefits.Domain.Services;

public interface IBenefitService
{
    // TODO: Criar camada específica para visão partner.
    // Os métodos atuais atendem o fluxo administrativo.
    // Antes de liberar a visão partner em produção, implementar métodos com filtro obrigatório por partnerId,
    // incluindo listagem, resumo, filtros, detalhe, criação, edição e alteração de status dentro do escopo do parceiro autenticado.
    Task<PagedResultDto<BenefitListItemDto>> GetPagedAsync(
        BenefitFilterDto filter,
        CancellationToken cancellationToken = default);

    Task<PagedResultDto<BenefitApprovalItemDto>> GetPendingAsync(
        BenefitFilterDto filter,
        CancellationToken cancellationToken = default);

    Task<BenefitDashboardSummaryDto> GetDashboardSummaryAsync(
        CancellationToken cancellationToken = default);

    Task<BenefitFilterOptionsDto> GetFilterOptionsAsync(
        CancellationToken cancellationToken = default);

    Task<BenefitDetailsDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<Guid> CreateAsync(
        CreateBenefitRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(
        Guid id,
        UpdateBenefitRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> ChangeStatusAsync(
        Guid id,
        ChangeBenefitStatusRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> AddReviewAsync(
        Guid id,
        AddBenefitReviewRequest request,
        CancellationToken cancellationToken = default);
}