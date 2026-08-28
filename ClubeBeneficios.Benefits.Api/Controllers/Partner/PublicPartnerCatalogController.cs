using ClubeBeneficios.Benefits.Domain.Dtos.PublicCatalog;
using ClubeBeneficios.Benefits.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClubeBeneficios.Benefits.Api.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/public/partner-catalog")]
public class PublicPartnerCatalogController : ControllerBase
{
    private readonly IPublicPartnerCatalogService _service;
    private readonly IFileStorageService _fileStorageService;

    public PublicPartnerCatalogController(
        IPublicPartnerCatalogService service,
        IFileStorageService fileStorageService)
    {
        _service = service;
        _fileStorageService = fileStorageService;
    }

    [HttpGet("{slug}")]
    public async Task<IActionResult> GetBySlug(
        [FromRoute] string slug,
        CancellationToken cancellationToken)
    {
        var result = await _service.GetBySlugAsync(slug, cancellationToken);

        if (result is null)
        {
            return NotFound(new
            {
                message = "Vitrine pública não encontrada ou indisponível."
            });
        }

        return Ok(result);
    }

    [HttpPost("{slug}/benefits/{benefitId:guid}/requests")]
    [RequestSizeLimit(10_000_000)]
    public async Task<IActionResult> CreateRequest(
    [FromRoute] string slug,
    [FromRoute] Guid benefitId,
    [FromForm] CreatePublicPartnerBenefitRequestFormDto request,
    CancellationToken cancellationToken)
    {
        try
        {
            var result = await _service.CreateRequestFromFormAsync(
                slug,
                benefitId,
                request,
                cancellationToken);

            if (request.VaccinationCardFile is not null && request.VaccinationCardFile.Length > 0)
            {
                var storedFile = await _fileStorageService.SavePartnerCustomerDocumentAsync(
                    request.VaccinationCardFile,
                    result.PartnerCustomerId,
                    cancellationToken);

                var partnerCustomerDocumentId = await _service.InsertPartnerCustomerDocumentAsync(
                    result.PartnerCustomerId,
                    result.PartnerCustomerPetId,
                    "vaccination_card",
                    storedFile.FileUrl,
                    storedFile.OriginalFileName,
                    storedFile.MimeType,
                    cancellationToken);

                await _service.LinkVaccinationCardToBenefitRequestAsync(
                    result.RequestId,
                    partnerCustomerDocumentId,
                    storedFile.FileUrl,
                    storedFile.OriginalFileName,
                    cancellationToken);
            }

            await _service.UpsertBenefitRequestPreventiveAsync(
                result.RequestId,
                "dewormer",
                request.DewormerBrandName,
                request.DewormerAppliedAt,
                request.DewormerExpiresAt,
                cancellationToken);

            await _service.UpsertBenefitRequestPreventiveAsync(
                result.RequestId,
                "flea_tick",
                request.FleaTickBrandName,
                request.FleaTickAppliedAt,
                request.FleaTickExpiresAt,
                cancellationToken);

            await _service.EnqueueRequestNotificationAsync(
                result.RequestId,
                "benefits.request.submitted.admin",
                "Nova solicitação pública de benefício recebida.",
                cancellationToken);

            return Accepted(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }
}