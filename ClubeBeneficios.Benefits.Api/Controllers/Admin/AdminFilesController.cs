using ClubeBeneficios.Benefits.Domain.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Options;

namespace ClubeBeneficios.Benefits.Api.Controllers.Admin;

[ApiController]
[Authorize(Roles = "admin")]
[Route("api/admin/files")]
public class AdminFilesController : ControllerBase
{
    private readonly FileStorageOptions _options;
    private readonly FileExtensionContentTypeProvider _contentTypeProvider = new();

    public AdminFilesController(IOptions<FileStorageOptions> options)
    {
        _options = options.Value;
    }

    [HttpGet("download")]
    public IActionResult Download(
        [FromQuery] string fileUrl,
        [FromQuery] string? fileName = null)
    {
        if (string.IsNullOrWhiteSpace(fileUrl))
            return BadRequest(new { message = "Arquivo não informado." });

        if (string.IsNullOrWhiteSpace(_options.LocalRootPath))
            return BadRequest(new { message = "Caminho de armazenamento não configurado." });

        var publicBasePath = string.IsNullOrWhiteSpace(_options.PublicBasePath)
            ? "/uploads"
            : _options.PublicBasePath.TrimEnd('/');

        var normalizedUrl = Uri.UnescapeDataString(fileUrl).Replace('\\', '/');

        if (Uri.TryCreate(normalizedUrl, UriKind.Absolute, out var absoluteUri))
        {
            normalizedUrl = absoluteUri.AbsolutePath;
        }

        if (!normalizedUrl.StartsWith(publicBasePath + "/", StringComparison.OrdinalIgnoreCase))
            return BadRequest(new { message = "Caminho de arquivo inválido." });

        var relativePath = normalizedUrl
            .Substring(publicBasePath.Length)
            .TrimStart('/')
            .Replace('/', Path.DirectorySeparatorChar);

        var rootPath = Path.GetFullPath(_options.LocalRootPath);
        var fullPath = Path.GetFullPath(Path.Combine(rootPath, relativePath));

        if (!fullPath.StartsWith(rootPath + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase)
            && !string.Equals(fullPath, rootPath, StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest(new { message = "Caminho de arquivo inválido." });
        }

        if (!System.IO.File.Exists(fullPath))
            return NotFound(new { message = "Arquivo não encontrado." });

        if (!_contentTypeProvider.TryGetContentType(fullPath, out var contentType))
            contentType = "application/octet-stream";

        var downloadFileName = string.IsNullOrWhiteSpace(fileName)
            ? Path.GetFileName(fullPath)
            : Path.GetFileName(fileName);

        return PhysicalFile(
            fullPath,
            contentType,
            downloadFileName,
            enableRangeProcessing: true);
    }
}