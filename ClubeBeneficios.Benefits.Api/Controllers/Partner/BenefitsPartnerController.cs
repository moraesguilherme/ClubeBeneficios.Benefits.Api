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

    public BenefitsPartnerController(IBenefitService benefitService)
    {
        _benefitService = benefitService;
    }

    [HttpGet]
    public async Task<IActionResult> GetPaged([FromQuery] BenefitFilterDto filter, CancellationToken cancellationToken)
        => Ok(await _benefitService.GetPartnerPagedAsync(filter, cancellationToken));

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary(CancellationToken cancellationToken)
        => Ok(await _benefitService.GetPartnerSummaryAsync(cancellationToken));

    [HttpGet("filter-options")]
    public async Task<IActionResult> GetFilterOptions(CancellationToken cancellationToken)
        => Ok(await _benefitService.GetPartnerFilterOptionsAsync(cancellationToken));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var item = await _benefitService.GetPartnerByIdAsync(id, cancellationToken);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBenefitRequest request, CancellationToken cancellationToken)
    {
        var id = await _benefitService.CreatePartnerAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateBenefitRequest request, CancellationToken cancellationToken)
    {
        await _benefitService.UpdatePartnerAsync(id, request, cancellationToken);
        return NoContent();
    }

    [HttpPut("{id:guid}/status")]
    public async Task<IActionResult> ChangeStatus([FromRoute] Guid id, [FromBody] ChangeBenefitStatusRequest request, CancellationToken cancellationToken)
    {
        await _benefitService.ChangePartnerStatusAsync(id, request, cancellationToken);
        return NoContent();
    }
}