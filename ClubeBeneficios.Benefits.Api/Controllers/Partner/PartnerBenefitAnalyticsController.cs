using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ClubeBeneficios.Benefits.Domain.Dtos.Requests;
using ClubeBeneficios.Benefits.Domain.Services;

namespace ClubeBeneficios.Benefits.Api.Controllers.Partner;

[ApiController]
[Produces("application/json")]
[Route("api/partner/benefits/analytics")]
[Authorize(Roles = "partner")]
public class PartnerBenefitAnalyticsController : ControllerBase
{
    private readonly IBenefitAnalyticsService _analyticsService;

    public PartnerBenefitAnalyticsController(IBenefitAnalyticsService analyticsService)
    {
        _analyticsService = analyticsService;
    }

    [HttpGet("dashboard-summary")]
    public async Task<IActionResult> GetDashboardSummary(
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        CancellationToken cancellationToken)
    {
        var result = await _analyticsService.GetPartnerDashboardSummaryAsync(
            new BenefitDashboardSummaryFilterDto
            {
                StartDate = startDate,
                EndDate = endDate
            },
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("{benefitId:guid}/history")]
    public async Task<IActionResult> GetHistory(Guid benefitId, CancellationToken cancellationToken)
    {
        var result = await _analyticsService.GetPartnerHistoryAsync(
            new BenefitHistoryFilterDto
            {
                BenefitId = benefitId
            },
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("{benefitId:guid}/metrics")]
    public async Task<IActionResult> GetMetrics(
        Guid benefitId,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        CancellationToken cancellationToken)
    {
        var result = await _analyticsService.GetPartnerMetricsAsync(
            new BenefitMetricsFilterDto
            {
                BenefitId = benefitId,
                StartDate = startDate,
                EndDate = endDate
            },
            cancellationToken);

        return Ok(result);
    }
}