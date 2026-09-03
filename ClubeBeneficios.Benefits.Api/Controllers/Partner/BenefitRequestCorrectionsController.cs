using ClubeBeneficios.Benefits.Domain.Dtos.PublicCatalog;
using ClubeBeneficios.Benefits.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClubeBeneficios.Benefits.Api.Controllers.Public;

[ApiController]
[AllowAnonymous]
[Route("api/public/benefit-request-corrections")]
public class BenefitRequestCorrectionsController : ControllerBase
{
    private readonly IPublicPartnerCatalogService _service;
    private readonly IFileStorageService _fileStorageService;

    public BenefitRequestCorrectionsController(
        IPublicPartnerCatalogService service,
        IFileStorageService fileStorageService)
    {
        _service = service;
        _fileStorageService = fileStorageService;
    }

    [HttpGet("{token}")]
    public async Task<IActionResult> GetByToken(
        [FromRoute] string token,
        CancellationToken cancellationToken)
    {
        var result = await _service.GetCorrectionByTokenAsync(
            token,
            cancellationToken);

        if (result is null)
        {
            return NotFound(new
            {
                message = "Link de ajuste inválido, expirado ou já utilizado."
            });
        }

        return Ok(result);
    }

    [HttpPost("{token}")]
    [RequestSizeLimit(10_000_000)]
    public async Task<IActionResult> SubmitCorrection(
        [FromRoute] string token,
        [FromForm] SubmitPublicBenefitRequestCorrectionFormDto request,
        CancellationToken cancellationToken)
    {
        try
        {
            var correction = await _service.GetCorrectionByTokenAsync(
                token,
                cancellationToken);

            if (correction is null)
            {
                return NotFound(new
                {
                    message = "Link de ajuste inválido, expirado ou já utilizado."
                });
            }

            if (request.VaccinationCardFile is not null && request.VaccinationCardFile.Length > 0)
            {
                var storedFile = await _fileStorageService.SavePartnerCustomerDocumentAsync(
                    request.VaccinationCardFile,
                    correction.PartnerCustomerId,
                    cancellationToken);

                var partnerCustomerDocumentId = await _service.InsertPartnerCustomerDocumentAsync(
                    correction.PartnerCustomerId,
                    correction.PartnerCustomerPetId,
                    "vaccination_card",
                    storedFile.FileUrl,
                    storedFile.OriginalFileName,
                    storedFile.MimeType,
                    cancellationToken);

                await _service.LinkVaccinationCardToBenefitRequestAsync(
                    correction.RequestId,
                    partnerCustomerDocumentId,
                    storedFile.FileUrl,
                    storedFile.OriginalFileName,
                    cancellationToken);
            }

            await _service.UpsertBenefitRequestPreventiveAsync(
                correction.RequestId,
                "dewormer",
                request.DewormerBrandName,
                request.DewormerAppliedAt,
                request.DewormerExpiresAt,
                cancellationToken);

            await _service.UpsertBenefitRequestPreventiveAsync(
                correction.RequestId,
                "flea_tick",
                request.FleaTickBrandName,
                request.FleaTickAppliedAt,
                request.FleaTickExpiresAt,
                cancellationToken);

            var result = await _service.SubmitCorrectionAsync(
                token,
                request.CustomerNotes,
                cancellationToken);

            return Ok(result);
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