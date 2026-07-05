using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ClubeBeneficios.Benefits.Domain.Dtos;
using ClubeBeneficios.Benefits.Domain.Dtos.Requests;
using ClubeBeneficios.Benefits.Domain.Services;

namespace ClubeBeneficios.Benefits.Api.Controllers.Admin;

[ApiController]
[Produces("application/json")]
[Route("api/admin/benefit-requests")]
[Authorize(Roles = "admin")]
public class BenefitRequestsController : ControllerBase
{
    private readonly IBenefitRequestService _service;

    public BenefitRequestsController(IBenefitRequestService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResultDto<BenefitRequestListItemDto>>> Search(
        [FromQuery] BenefitRequestFilterDto filter,
        CancellationToken cancellationToken)
        => Ok(await _service.SearchAdminAsync(filter, cancellationToken));

    [HttpGet("approval-queue")]
    public async Task<ActionResult<PagedResultDto<BenefitRequestListItemDto>>> GetApprovalQueue(
        [FromQuery] BenefitRequestFilterDto filter,
        CancellationToken cancellationToken)
        => Ok(await _service.SearchPendingReviewAsync(filter, cancellationToken));

    [HttpGet("approval-summary")]
    public async Task<ActionResult<BenefitRequestApprovalSummaryDto>> GetApprovalSummary(
        CancellationToken cancellationToken)
        => Ok(await _service.GetApprovalSummaryAsync(cancellationToken));

    [HttpGet("{requestId:guid}")]
    public async Task<ActionResult<BenefitRequestDetailDto>> GetById(
        Guid requestId,
        CancellationToken cancellationToken)
    {
        var result = await _service.GetByIdAsync(requestId, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create(
    [FromBody] CreateBenefitRequestDto request,
    CancellationToken cancellationToken)
    {
        var id = await _service.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { requestId = id }, id);
    }

    [HttpPost("{requestId:guid}/health")]
    public async Task<IActionResult> SubmitHealth(
        Guid requestId,
        [FromBody] SubmitBenefitRequestHealthRequest request,
        CancellationToken cancellationToken)
    {
        await _service.SubmitHealthAsync(requestId, request, cancellationToken);
        return NoContent();
    }

    [HttpPost("{requestId:guid}/reviews")]
    public async Task<IActionResult> AddReview(
        Guid requestId,
        [FromBody] AddBenefitRequestReviewRequest request,
        CancellationToken cancellationToken)
    {
        await _service.AddReviewAsync(requestId, request, cancellationToken);
        return NoContent();
    }

    [HttpPost("{requestId:guid}/approve")]
    public async Task<IActionResult> Approve(
        Guid requestId,
        [FromBody] AddBenefitRequestReviewRequest request,
        CancellationToken cancellationToken)
    {
        await _service.ApproveAsync(requestId, request, cancellationToken);
        return NoContent();
    }

    [HttpPost("{requestId:guid}/request-changes")]
    public async Task<IActionResult> RequestChanges(
        Guid requestId,
        [FromBody] AddBenefitRequestReviewRequest request,
        CancellationToken cancellationToken)
    {
        await _service.RequestChangesAsync(requestId, request, cancellationToken);
        return NoContent();
    }

    [HttpPost("{requestId:guid}/reject")]
    public async Task<IActionResult> Reject(
        Guid requestId,
        [FromBody] AddBenefitRequestReviewRequest request,
        CancellationToken cancellationToken)
    {
        await _service.RejectAsync(requestId, request, cancellationToken);
        return NoContent();
    }

    [HttpPut("{requestId:guid}/status")]
    public async Task<IActionResult> ChangeStatus(
        Guid requestId,
        [FromBody] ChangeBenefitRequestStatusRequest request,
        CancellationToken cancellationToken)
    {
        await _service.ChangeStatusAsync(requestId, request, cancellationToken);
        return NoContent();
    }

    [HttpPost("{requestId:guid}/usage-confirmations")]
    public async Task<ActionResult<BenefitUsageConfirmationPairResultDto>> CreateUsageConfirmationPair(
    Guid requestId,
    [FromBody] CreateBenefitUsageConfirmationPairRequest request,
    CancellationToken cancellationToken)
    {
        var result = await _service.CreateUsageConfirmationPairAsync(
            requestId,
            request,
            cancellationToken);

        return Ok(result);
    }
}