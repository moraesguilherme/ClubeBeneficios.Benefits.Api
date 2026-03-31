using Microsoft.AspNetCore.Mvc;
using ClubeBeneficios.Benefits.Domain.Dtos;

namespace ClubeBeneficios.Benefits.Api.Controllers;

[ApiController]
[Produces("application/json")]
[Route("api/health")]
public class HealthController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(HealthCheckResponseDto), StatusCodes.Status200OK)]
    public IActionResult Get()
    {
        return Ok(new HealthCheckResponseDto
        {
            UtcNow = DateTime.UtcNow
        });
    }
}