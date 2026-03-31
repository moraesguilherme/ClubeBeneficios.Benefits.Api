using System;
using System.Threading;
using System.Threading.Tasks;
using ClubeBeneficios.Benefits.Domain.Dtos.Filters;
using ClubeBeneficios.Benefits.Domain.Dtos.Requests;
using ClubeBeneficios.Benefits.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace ClubeBeneficios.Benefits.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/benefits")]
public class BenefitsAdminController : ControllerBase
{
    private readonly IBenefitService _benefitService;

    public BenefitsAdminController(IBenefitService benefitService)
    {
        _benefitService = benefitService;
    }

    [HttpGet]
    public async Task<IActionResult> GetPaged(
        [FromQuery] BenefitFilterDto filter,
        CancellationToken cancellationToken)
    {
        var result = await _benefitService.GetPagedAsync(filter, cancellationToken);
        return Ok(result);
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary(CancellationToken cancellationToken)
    {
        var result = await _benefitService.GetDashboardSummaryAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet("filter-options")]
    public async Task<IActionResult> GetFilterOptions(CancellationToken cancellationToken)
    {
        var result = await _benefitService.GetFilterOptionsAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet("approvals")]
    public async Task<IActionResult> GetPending(
        [FromQuery] BenefitFilterDto filter,
        CancellationToken cancellationToken)
    {
        var result = await _benefitService.GetPendingAsync(filter, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _benefitService.GetByIdAsync(id, cancellationToken);
        if (result is null)
            return NotFound();

        return Ok(result);
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
        Guid id,
        [FromBody] UpdateBenefitRequest request,
        CancellationToken cancellationToken)
    {
        var success = await _benefitService.UpdateAsync(id, request, cancellationToken);
        return success ? NoContent() : NotFound();
    }

    [HttpPut("{id:guid}/status")]
    public async Task<IActionResult> ChangeStatus(
        Guid id,
        [FromBody] ChangeBenefitStatusRequest request,
        CancellationToken cancellationToken)
    {
        var success = await _benefitService.ChangeStatusAsync(id, request, cancellationToken);
        return success ? NoContent() : NotFound();
    }

    [HttpPost("{id:guid}/reviews")]
    public async Task<IActionResult> AddReview(
        Guid id,
        [FromBody] AddBenefitReviewRequest request,
        CancellationToken cancellationToken)
    {
        var success = await _benefitService.AddReviewAsync(id, request, cancellationToken);
        return success ? NoContent() : NotFound();
    }
}