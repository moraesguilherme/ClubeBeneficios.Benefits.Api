using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ClubeBeneficios.Benefits.Api.Mappers;
using ClubeBeneficios.Benefits.Domain.Dtos.Partner;
using ClubeBeneficios.Benefits.Domain.Security;
using ClubeBeneficios.Benefits.Domain.Services;
using ClubeBeneficios.Benefits.Domain.Dtos.Common;
using ClubeBeneficios.Benefits.Domain.Dtos.BenefitRequests;
using ClubeBeneficios.Benefits.Domain.Dtos.Requests.BenefitRequests;

namespace ClubeBeneficios.Benefits.Api.Controllers.Partner;

[ApiController]
[Produces("application/json")]
[Route("api/partner/benefit-requests")]
[Authorize(Roles = "partner")]
public class PartnerBenefitRequestsController : ControllerBase
{
    private readonly IBenefitRequestService _service;
    private readonly ICurrentUser _currentUser;

    public PartnerBenefitRequestsController(
        IBenefitRequestService service,
        ICurrentUser currentUser)
    {
        _service = service;
        _currentUser = currentUser;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResultDto<PartnerBenefitRequestListItemDto>>> Search(
        [FromQuery] BenefitUsageRequestFilterDto filter,
        CancellationToken cancellationToken)
    {
        var partnerId = GetPartnerId();

        if (partnerId is null)
            return Unauthorized();

        filter.PartnerId = partnerId;
        filter.RequesterType = "partner_customer";

        var result = await _service.SearchPartnerAsync(filter, cancellationToken);

        return Ok(result.ToPartnerPagedResult());
    }

    [HttpGet("{requestId:guid}")]
    public async Task<ActionResult<PartnerBenefitRequestDetailDto>> GetById(
        Guid requestId,
        CancellationToken cancellationToken)
    {
        var partnerId = GetPartnerId();

        if (partnerId is null)
            return Unauthorized();

        var result = await _service.GetByIdAsync(requestId, cancellationToken);

        if (result is null)
            return NotFound();

        if (result.PartnerId != partnerId)
            return NotFound();

        return Ok(result.ToPartnerDto());
    }

    [HttpPut("{requestId:guid}/status")]
    public async Task<IActionResult> ChangeStatus(
        Guid requestId,
        [FromBody] ChangeBenefitUsageRequestStatusRequest request,
        CancellationToken cancellationToken)
    {
        var partnerId = GetPartnerId();

        if (partnerId is null)
            return Unauthorized();

        var current = await _service.GetByIdAsync(requestId, cancellationToken);

        if (current is null)
            return NotFound();

        if (current.PartnerId != partnerId)
            return NotFound();

        if (request.RequestStatus is not "scheduled" and not "cancelled" and not "no_show")
        {
            return BadRequest(new
            {
                message = "Parceiro só pode alterar a solicitação para scheduled, cancelled ou no_show."
            });
        }

        await _service.ChangeStatusAsync(requestId, request, cancellationToken);

        return NoContent();
    }

    private Guid? GetPartnerId()
    {
        return _currentUser.PartnerId;
    }
}