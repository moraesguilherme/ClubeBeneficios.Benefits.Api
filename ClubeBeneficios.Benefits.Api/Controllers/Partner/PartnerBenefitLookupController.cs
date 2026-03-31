using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ClubeBeneficios.Benefits.Domain.Services;

namespace ClubeBeneficios.Benefits.Api.Controllers.Partner;

[ApiController]
[Produces("application/json")]
[Route("api/partner/benefits/lookups")]
[Authorize(Roles = "partner")]
public class PartnerBenefitLookupController : ControllerBase
{
    private readonly IBenefitLookupService _lookupService;

    public PartnerBenefitLookupController(IBenefitLookupService lookupService)
    {
        _lookupService = lookupService;
    }

    [HttpGet]
    public async Task<IActionResult> GetOptions(CancellationToken cancellationToken)
    {
        var result = await _lookupService.GetPartnerOptionsAsync(cancellationToken);
        return Ok(result);
    }
}