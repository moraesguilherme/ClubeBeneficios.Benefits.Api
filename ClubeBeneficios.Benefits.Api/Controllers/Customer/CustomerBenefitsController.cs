using ClubeBeneficios.Benefits.Domain.Dtos.Common;
using ClubeBeneficios.Benefits.Domain.Dtos.Customer;
using ClubeBeneficios.Benefits.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClubeBeneficios.Benefits.Api.Controllers.Customer;

[ApiController]
[Produces("application/json")]
[Route("api/customer/benefits")]
[Authorize(Roles = "client")]
public sealed class CustomerBenefitsController : ControllerBase
{
    private readonly ICustomerBenefitService _service;

    public CustomerBenefitsController(ICustomerBenefitService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResultDto<CustomerBenefitListItemDto>>> GetAvailable(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 12,
        [FromQuery] string? search = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _service.GetAvailableAsync(
            page,
            pageSize,
            search,
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("{benefitId:guid}")]
    public async Task<ActionResult<CustomerBenefitDetailDto>> GetById(
        Guid benefitId,
        CancellationToken cancellationToken)
    {
        var result = await _service.GetByIdAsync(
            benefitId,
            cancellationToken);

        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost("{benefitId:guid}/requests")]
    public async Task<ActionResult<Guid>> CreateRequest(
        Guid benefitId,
        [FromBody] CreateCustomerBenefitRequestDto request,
        CancellationToken cancellationToken)
    {
        var id = await _service.CreateRequestAsync(
            benefitId,
            request,
            cancellationToken);

        return Ok(new { id });
    }

    [HttpGet("requests")]
    public async Task<ActionResult<PagedResultDto<CustomerBenefitRequestListItemDto>>> GetMyRequests(
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 12,
    [FromQuery] string? status = null,
    CancellationToken cancellationToken = default)
    {
        var result = await _service.GetMyRequestsAsync(
            page,
            pageSize,
            status,
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("usages")]
    public async Task<ActionResult<PagedResultDto<CustomerBenefitUsageListItemDto>>> GetMyUsages(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 12,
        CancellationToken cancellationToken = default)
    {
        var result = await _service.GetMyUsagesAsync(
            page,
            pageSize,
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("requests/{requestId:guid}")]
    public async Task<ActionResult<CustomerBenefitRequestDetailDto>> GetMyRequestById(
    Guid requestId,
    CancellationToken cancellationToken)
    {
        var result = await _service.GetMyRequestByIdAsync(
            requestId,
            cancellationToken);

        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("usages/{usageId:guid}")]
    public async Task<ActionResult<CustomerBenefitUsageDetailDto>> GetMyUsageById(
        Guid usageId,
        CancellationToken cancellationToken)
    {
        var result = await _service.GetMyUsageByIdAsync(
            usageId,
            cancellationToken);

        return result is null ? NotFound() : Ok(result);
    }
}