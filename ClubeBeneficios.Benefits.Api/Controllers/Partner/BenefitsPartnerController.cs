using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ClubeBeneficios.Benefits.Domain.Dtos.Filters;
using ClubeBeneficios.Benefits.Domain.Dtos.Requests;
using ClubeBeneficios.Benefits.Domain.Services;

namespace ClubeBeneficios.Benefits.Api.Controllers.Partner;

[ApiController]
[Produces("application/json")]
[Route("api/partner/benefits")]
[Authorize(Roles = "partner")]
public class BenefitsPartnerController : ControllerBase
{
    private readonly IBenefitService _benefitService;
    // TODO: Implementar isolamento de parceiro.
    // Quando a visão partner for ativada de fato, este controller não deve usar
    // apenas os métodos genéricos do módulo administrativo.
    // Será necessário aplicar escopo do parceiro autenticado (partnerId vindo do token/claims)
    // para garantir que o parceiro visualize e altere apenas os próprios benefícios.
    public BenefitsPartnerController(IBenefitService benefitService)
    {
        _benefitService = benefitService;
    }

    [HttpGet]
    public async Task<IActionResult> GetPaged(
        [FromQuery] BenefitFilterDto filter,
        CancellationToken cancellationToken)
        => Ok(await _benefitService.GetPagedAsync(filter, cancellationToken));

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary(CancellationToken cancellationToken)
        => Ok(await _benefitService.GetDashboardSummaryAsync(cancellationToken));

    [HttpGet("filter-options")]
    public async Task<IActionResult> GetFilterOptions(CancellationToken cancellationToken)
        => Ok(await _benefitService.GetFilterOptionsAsync(cancellationToken));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var item = await _benefitService.GetByIdAsync(id, cancellationToken);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateBenefitRequest request,
        CancellationToken cancellationToken)
    {
        var id = await _benefitService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        [FromRoute] Guid id,
        [FromBody] UpdateBenefitRequest request,
        CancellationToken cancellationToken)
    {
        await _benefitService.UpdateAsync(id, request, cancellationToken);
        return NoContent();
    }

    [HttpPut("{id:guid}/status")]
    public async Task<IActionResult> ChangeStatus(
        [FromRoute] Guid id,
        [FromBody] ChangeBenefitStatusRequest request,
        CancellationToken cancellationToken)
    {
        await _benefitService.ChangeStatusAsync(id, request, cancellationToken);
        return NoContent();
    }
}