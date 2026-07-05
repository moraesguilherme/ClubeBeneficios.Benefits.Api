using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ClubeBeneficios.Benefits.Domain.Dtos;
using ClubeBeneficios.Benefits.Domain.Services;

namespace ClubeBeneficios.Benefits.Api.Controllers;

[ApiController]
[Produces("application/json")]
[Route("api/benefit-usage-confirmations")]
public class BenefitUsageConfirmationsController : ControllerBase
{
    private readonly IBenefitRequestService _service;

    public BenefitUsageConfirmationsController(IBenefitRequestService service)
    {
        _service = service;
    }

    [HttpGet("{token}")]
    [AllowAnonymous]
    public async Task<ActionResult<BenefitUsageConfirmationTokenDto>> GetByToken(
        string token,
        CancellationToken cancellationToken)
    {
        var result = await _service.GetUsageConfirmationByTokenAsync(
            token,
            cancellationToken);

        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost("{token}/confirm")]
    [AllowAnonymous]
    public async Task<ActionResult<BenefitUsageConfirmationConfirmResultDto>> Confirm(
        string token,
        CancellationToken cancellationToken)
    {
        var result = await _service.ConfirmUsageConfirmationAsync(
            token,
            cancellationToken);

        return Ok(result);
    }
}