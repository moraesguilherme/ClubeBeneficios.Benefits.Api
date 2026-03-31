using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ClubeBeneficios.Benefits.Domain.Dtos;
using ClubeBeneficios.Benefits.Domain.Dtos.Requests;
using ClubeBeneficios.Benefits.Domain.Services;

namespace ClubeBeneficios.Benefits.Api.Controllers.Partner;

[ApiController]
[Produces("application/json")]
[Route("api/partner/benefit-requests")]
[Authorize(Roles = "partner")]
public class PartnerBenefitRequestsController : ControllerBase
{
    private readonly IBenefitRequestService _service;

    public PartnerBenefitRequestsController(IBenefitRequestService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResultDto<BenefitRequestListItemDto>>> Search(
        [FromQuery] BenefitRequestFilterDto filter,
        CancellationToken cancellationToken)
        => Ok(await _service.SearchPartnerAsync(filter, cancellationToken));

    [HttpGet("{requestId:guid}")]
    public async Task<ActionResult<BenefitRequestDetailDto>> GetById(Guid requestId, CancellationToken cancellationToken)
    {
        var result = await _service.GetByIdAsync(requestId, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPut("{requestId:guid}/status")]
    public async Task<IActionResult> ChangeStatus(Guid requestId, [FromBody] ChangeBenefitRequestStatusRequest request, CancellationToken cancellationToken)
    {
        await _service.ChangeStatusAsync(requestId, request, cancellationToken);
        return NoContent();
    }
}