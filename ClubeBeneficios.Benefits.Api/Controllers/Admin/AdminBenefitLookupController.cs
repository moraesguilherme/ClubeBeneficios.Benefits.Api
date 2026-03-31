using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ClubeBeneficios.Benefits.Domain.Services;

namespace ClubeBeneficios.Benefits.Api.Controllers.Admin;

[ApiController]
[Produces("application/json")]
[Route("api/admin/benefits/lookups")]
[Authorize(Roles = "admin")]
public class AdminBenefitLookupController : ControllerBase
{
    private readonly IBenefitLookupService _lookupService;

    public AdminBenefitLookupController(IBenefitLookupService lookupService)
    {
        _lookupService = lookupService;
    }

    [HttpGet]
    public async Task<IActionResult> GetOptions(
        [FromQuery] Guid? partnerId,
        CancellationToken cancellationToken)
    {
        var result = await _lookupService.GetAdminOptionsAsync(partnerId, cancellationToken);
        return Ok(result);
    }
}