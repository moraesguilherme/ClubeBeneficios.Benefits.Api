using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ClubeBeneficios.Benefits.Domain.Dtos.Requests;
using ClubeBeneficios.Benefits.Domain.Services;
using System.Security.Claims;

namespace ClubeBeneficios.Benefits.Api.Controllers.Admin;

[ApiController]
[Produces("application/json")]
[Route("api/admin/benefits/analytics")]
[Authorize(Roles = "admin")]
public class AdminBenefitAnalyticsController : ControllerBase
{
    private readonly IBenefitAnalyticsService _analyticsService;
    private readonly IBenefitLevelAutomationService _levelAutomationService;

    public AdminBenefitAnalyticsController(
        IBenefitAnalyticsService analyticsService,
        IBenefitLevelAutomationService levelAutomationService)
    {
        _analyticsService = analyticsService;
        _levelAutomationService = levelAutomationService;
    }

    [HttpGet("dashboard-summary")]
    public async Task<IActionResult> GetDashboardSummary(
        [FromQuery] Guid? partnerId,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        CancellationToken cancellationToken)
    {
        var result = await _analyticsService.GetAdminDashboardSummaryAsync(
            new BenefitDashboardSummaryFilterDto
            {
                PartnerId = partnerId,
                StartDate = startDate,
                EndDate = endDate
            },
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("{benefitId:guid}/history")]
    public async Task<IActionResult> GetHistory(Guid benefitId, CancellationToken cancellationToken)
    {
        var result = await _analyticsService.GetAdminHistoryAsync(
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
        [FromQuery] Guid? partnerId,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        CancellationToken cancellationToken)
    {
        var result = await _analyticsService.GetAdminMetricsAsync(
            new BenefitMetricsFilterDto
            {
                BenefitId = benefitId,
                PartnerId = partnerId,
                StartDate = startDate,
                EndDate = endDate
            },
            cancellationToken);

        return Ok(result);
    }


    [HttpPost("analytics/levels/partners/recalculate")]
    public async Task<IActionResult> RecalculatePartnerLevels(
        [FromBody] RecalculatePartnerLevelsRequest request,
        CancellationToken cancellationToken)
    {
        var userIdValue =
            User.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
            User.FindFirst("sub")?.Value;

        Guid? changedByUserId = null;
        if (Guid.TryParse(userIdValue, out var parsedUserId))
        {
            changedByUserId = parsedUserId;
        }

        var result = await _levelAutomationService.RecalculatePartnerLevelsAsync(
            request,
            changedByUserId,
            cancellationToken);

        return Ok(result);
    }

    //migrar endpoint para o serviço de clientes

    [HttpPost("levels/clients/recalculate")]
    public async Task<IActionResult> RecalculateClientLevels(
            [FromBody] RecalculateClientLevelsRequest request,
            CancellationToken cancellationToken)
    {
        var result = await _levelAutomationService.RecalculateClientLevelsAsync(request, cancellationToken);
        return Ok(result);
    }
}