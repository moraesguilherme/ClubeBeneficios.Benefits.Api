using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ClubeBeneficios.Benefits.Api.Mappers;
using ClubeBeneficios.Benefits.Domain.Dtos;
using ClubeBeneficios.Benefits.Domain.Dtos.Partner;
using ClubeBeneficios.Benefits.Domain.Dtos.Requests;
using ClubeBeneficios.Benefits.Domain.Security;
using ClubeBeneficios.Benefits.Domain.Services;

namespace ClubeBeneficios.Benefits.Api.Controllers.Partner;

[ApiController]
[Produces("application/json")]
[Route("api/partner/benefit-usages")]
[Authorize(Roles = "partner")]
public class PartnerBenefitUsagesController : ControllerBase
{
    private readonly IBenefitUsageService _service;
    private readonly ICurrentUser _currentUser;

    public PartnerBenefitUsagesController(
        IBenefitUsageService service,
        ICurrentUser currentUser)
    {
        _service = service;
        _currentUser = currentUser;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResultDto<PartnerBenefitUsageListItemDto>>> Search(
        [FromQuery] BenefitUsageFilterDto filter,
        CancellationToken cancellationToken)
    {
        var partnerId = GetPartnerId();

        if (partnerId is null)
            return Unauthorized();

        var result = await _service.SearchPartnerAsync(filter, cancellationToken);

        return Ok(result.ToPartnerPagedResult());
    }

    [HttpGet("{usageId:guid}")]
    public async Task<ActionResult<PartnerBenefitUsageDetailDto>> GetById(
        Guid usageId,
        CancellationToken cancellationToken)
    {
        var partnerId = GetPartnerId();

        if (partnerId is null)
            return Unauthorized();

        var result = await _service.GetByIdAsync(usageId, cancellationToken);

        if (result is null)
            return NotFound();

        if (result.PartnerId != partnerId)
            return NotFound();

        return Ok(result.ToPartnerDto());
    }

    [HttpPost("confirm")]
    public async Task<ActionResult<Guid>> Confirm(
        [FromBody] ConfirmBenefitUsageRequest request,
        CancellationToken cancellationToken)
    {
        var partnerId = GetPartnerId();

        if (partnerId is null)
            return Unauthorized();

        request.PartnerId = partnerId;

        var id = await _service.ConfirmPartnerAsync(request, cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { usageId = id },
            id);
    }

    [HttpPost("validate")]
    public async Task<ActionResult<BenefitEligibilityValidationResultDto>> Validate(
        [FromBody] ValidateBenefitUsageRequest request,
        CancellationToken cancellationToken)
    {
        var partnerId = GetPartnerId();

        if (partnerId is null)
            return Unauthorized();

        request.PartnerId = partnerId;

        var result = await _service.ValidateAsync(request, cancellationToken);

        return Ok(result);
    }

    [HttpPut("{usageId:guid}/cancel")]
    public async Task<IActionResult> Cancel(
        Guid usageId,
        [FromBody] CancelBenefitUsageRequest request,
        CancellationToken cancellationToken)
    {
        var partnerId = GetPartnerId();

        if (partnerId is null)
            return Unauthorized();

        var current = await _service.GetByIdAsync(usageId, cancellationToken);

        if (current is null)
            return NotFound();

        if (current.PartnerId != partnerId)
            return NotFound();

        await _service.CancelAsync(usageId, request, cancellationToken);

        return NoContent();
    }

    private Guid? GetPartnerId()
    {
        return _currentUser.PartnerId;
    }
}