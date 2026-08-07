using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ClubeBeneficios.Benefits.Api.Mappers;
using ClubeBeneficios.Benefits.Domain.Dtos.Partner;
using ClubeBeneficios.Benefits.Domain.Security;
using ClubeBeneficios.Benefits.Domain.Services;
using ClubeBeneficios.Benefits.Domain.Dtos.Common;
using ClubeBeneficios.Benefits.Domain.Dtos.Requests.Benefits;
using ClubeBeneficios.Benefits.Domain.Dtos.Requests.Filters;

namespace ClubeBeneficios.Benefits.Api.Controllers.Partner;

[ApiController]
[Produces("application/json")]
[Route("api/partner/benefits")]
[Authorize(Roles = "partner")]
public class BenefitsPartnerController : ControllerBase
{
    private readonly IBenefitService _benefitService;
    private readonly ICurrentUser _currentUser;

    public BenefitsPartnerController(
        IBenefitService benefitService,
        ICurrentUser currentUser)
    {
        _benefitService = benefitService;
        _currentUser = currentUser;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResultDto<PartnerBenefitListItemDto>>> GetPaged(
        [FromQuery] BenefitFilterDto filter,
        CancellationToken cancellationToken)
    {
        var partnerId = GetPartnerId();

        if (partnerId is null)
            return Unauthorized();

        filter.PartnerId = partnerId;

        var result = await _benefitService.GetPagedAsync(filter, cancellationToken);

        return Ok(result.ToPartnerPagedResult());
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary(CancellationToken cancellationToken)
    {
        var partnerId = GetPartnerId();

        if (partnerId is null)
            return Unauthorized();

        var filter = new BenefitFilterDto
        {
            PartnerId = partnerId,
            Page = 1,
            PageSize = 500
        };

        var result = await _benefitService.GetPagedAsync(filter, cancellationToken);
        var items = result.Items.ToList();

        var activeBenefits = items.Count(x => x.Status == "active");
        var pendingBenefits = items.Count(x => x.Status is "pending_review" or "under_review");
        var inactiveBenefits = items.Count(x => x.Status == "inactive");

        var totalRequests = items.Sum(x => x.RequestsCount);
        var totalUsages = items.Sum(x => x.UsagesCount);

        var averageConversionRate = items.Count == 0
            ? 0
            : (int)Math.Round(items.Average(x => x.ConversionRate), 0);

        return Ok(new
        {
            totalBenefits = result.TotalCount,
            activeBenefits,
            pendingBenefits,
            inactiveBenefits,
            totalRequests,
            totalUsages,
            averageConversionRate
        });
    }

    [HttpGet("filter-options")]
    public async Task<IActionResult> GetFilterOptions(CancellationToken cancellationToken)
    {
        var result = await _benefitService.GetFilterOptionsAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PartnerBenefitDetailsDto>> GetById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var partnerId = GetPartnerId();

        if (partnerId is null)
            return Unauthorized();

        var item = await _benefitService.GetByIdAsync(id, cancellationToken);

        if (item is null)
            return NotFound();

        if (item.PartnerId != partnerId)
            return NotFound();

        return Ok(item.ToPartnerDto());
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateBenefitOfferRequest request,
        CancellationToken cancellationToken)
    {
        var partnerId = GetPartnerId();

        if (partnerId is null)
            return Unauthorized();

        request.PartnerId = partnerId;

        var id = await _benefitService.CreateAsync(request, cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id },
            new { id });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        [FromRoute] Guid id,
        [FromBody] UpdateBenefitOfferRequest request,
        CancellationToken cancellationToken)
    {
        var partnerId = GetPartnerId();

        if (partnerId is null)
            return Unauthorized();

        var current = await _benefitService.GetByIdAsync(id, cancellationToken);

        if (current is null)
            return NotFound();

        if (current.PartnerId != partnerId)
            return NotFound();

        request.PartnerId = partnerId;

        var success = await _benefitService.UpdateAsync(id, request, cancellationToken);

        return success ? NoContent() : NotFound();
    }

    [HttpPut("{id:guid}/status")]
    public async Task<IActionResult> ChangeStatus(
        [FromRoute] Guid id,
        [FromBody] ChangeBenefitOfferStatusRequest request,
        CancellationToken cancellationToken)
    {
        var partnerId = GetPartnerId();

        if (partnerId is null)
            return Unauthorized();

        var current = await _benefitService.GetByIdAsync(id, cancellationToken);

        if (current is null)
            return NotFound();

        if (current.PartnerId != partnerId)
            return NotFound();

        var success = await _benefitService.ChangeStatusAsync(id, request, cancellationToken);

        return success ? NoContent() : NotFound();
    }

    private Guid? GetPartnerId()
    {
        return _currentUser.PartnerId;
    }
}