using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ClubeBeneficios.Benefits.Domain.Services;
using ClubeBeneficios.Benefits.Domain.Dtos.Common;
using ClubeBeneficios.Benefits.Domain.Dtos.Benefits;
using ClubeBeneficios.Benefits.Domain.Dtos.BenefitUsages;
using ClubeBeneficios.Benefits.Domain.Dtos.Requests.BenefitUsages;

namespace ClubeBeneficios.Benefits.Api.Controllers.Admin;

[ApiController]
[Produces("application/json")]
[Route("api/admin/benefit-usages")]
[Authorize(Roles = "admin")]
public class BenefitUsagesController : ControllerBase
{
    private readonly IBenefitUsageService _service;

    public BenefitUsagesController(IBenefitUsageService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResultDto<BenefitUsageListItemDto>>> Search(
        [FromQuery] BenefitUsageFilterDto filter,
        CancellationToken cancellationToken)
    {
        var result = await _service.SearchAdminAsync(filter, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{usageId:guid}")]
    public async Task<ActionResult<BenefitUsageDetailDto>> GetById(
        Guid usageId,
        CancellationToken cancellationToken)
    {
        var result = await _service.GetByIdAsync(usageId, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost("confirm")]
    public async Task<ActionResult<Guid>> Confirm(
        [FromBody] ConfirmBenefitUsageRequest request,
        CancellationToken cancellationToken)
    {
        var id = await _service.ConfirmAdminAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { usageId = id }, id);
    }

    [HttpPost("validate")]
    public async Task<ActionResult<BenefitOfferEligibilityValidationResultDto>> Validate(
        [FromBody] ValidateBenefitUsageRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _service.ValidateAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpPut("{usageId:guid}/cancel")]
    public async Task<IActionResult> Cancel(
        Guid usageId,
        [FromBody] CancelBenefitUsageRequest request,
        CancellationToken cancellationToken)
    {
        await _service.CancelAsync(usageId, request, cancellationToken);
        return NoContent();
    }
}