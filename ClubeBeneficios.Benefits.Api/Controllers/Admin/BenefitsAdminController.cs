using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ClubeBeneficios.Benefits.Domain.Dtos.Filters;
using ClubeBeneficios.Benefits.Domain.Dtos.Requests;
using ClubeBeneficios.Benefits.Domain.Services;

namespace ClubeBeneficios.Benefits.Api.Controllers.Admin;

[ApiController]
[Produces("application/json")]
[Route("api/admin/benefits")]
[Authorize(Roles = "admin")]
public class BenefitsAdminController : ControllerBase
{
    private readonly IBenefitService _benefitService;

    public BenefitsAdminController(IBenefitService benefitService)
    {
        _benefitService = benefitService;
    }

    [HttpGet]
    public async Task<IActionResult> GetPaged([FromQuery] BenefitFilterDto filter, CancellationToken cancellationToken)
        => Ok(await _benefitService.GetAdminPagedAsync(filter, cancellationToken));

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary(CancellationToken cancellationToken)
        => Ok(await _benefitService.GetAdminSummaryAsync(cancellationToken));

    [HttpGet("filter-options")]
    public async Task<IActionResult> GetFilterOptions(CancellationToken cancellationToken)
        => Ok(await _benefitService.GetAdminFilterOptionsAsync(cancellationToken));

    [HttpGet("approvals")]
    public async Task<IActionResult> GetApprovalQueue([FromQuery] BenefitFilterDto filter, CancellationToken cancellationToken)
    {
        filter.OnlyPendingApproval = true;
        return Ok(await _benefitService.GetApprovalQueueAsync(filter, cancellationToken));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var item = await _benefitService.GetAdminByIdAsync(id, cancellationToken);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBenefitRequest request, CancellationToken cancellationToken)
    {
        var id = await _benefitService.CreateAdminAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateBenefitRequest request, CancellationToken cancellationToken)
    {
        await _benefitService.UpdateAdminAsync(id, request, cancellationToken);
        return NoContent();
    }

    [HttpPut("{id:guid}/status")]
    public async Task<IActionResult> ChangeStatus([FromRoute] Guid id, [FromBody] ChangeBenefitStatusRequest request, CancellationToken cancellationToken)
    {
        await _benefitService.ChangeAdminStatusAsync(id, request, cancellationToken);
        return NoContent();
    }

    [HttpPost("{id:guid}/reviews")]
    public async Task<IActionResult> AddReview([FromRoute] Guid id, [FromBody] ReviewBenefitRequest request, CancellationToken cancellationToken)
    {
        await _benefitService.AddAdminReviewAsync(id, request, cancellationToken);
        return NoContent();
    }
}