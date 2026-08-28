using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using ClubeBeneficios.Benefits.Domain.Options;
using ClubeBeneficios.Benefits.Domain.Services;

namespace ClubeBeneficios.Benefits.Infrastructure.Services;

public class LocalFileStorageService : IFileStorageService
{
    private readonly FileStorageOptions _options;

    private static readonly string[] AllowedMimeTypes =
    [
        "application/pdf",
        "image/jpeg",
        "image/png",
        "image/webp"
    ];

    public LocalFileStorageService(IOptions<FileStorageOptions> options)
    {
        _options = options.Value;
    }

    public async Task<StoredFileResult> SavePartnerCustomerDocumentAsync(
        IFormFile file,
        Guid partnerCustomerId,
        CancellationToken cancellationToken = default)
    {
        if (file is null || file.Length == 0)
            throw new ArgumentException("Arquivo não informado.");

        if (!AllowedMimeTypes.Contains(file.ContentType))
            throw new ArgumentException("Formato da carteirinha inválido. Envie PDF, JPG, PNG ou WEBP.");

        if (file.Length > 10 * 1024 * 1024)
            throw new ArgumentException("A carteirinha deve ter no máximo 10MB.");

        if (string.IsNullOrWhiteSpace(_options.LocalRootPath))
            throw new InvalidOperationException("Caminho de armazenamento não configurado.");

        var documentsFolder = Path.Combine(
            _options.LocalRootPath,
            "partner-customer-documents");

        Directory.CreateDirectory(documentsFolder);

        var extension = Path.GetExtension(file.FileName);

        if (string.IsNullOrWhiteSpace(extension))
            extension = GuessExtension(file.ContentType);

        var safeFileName = $"{partnerCustomerId:N}-{Guid.NewGuid():N}{extension}";
        var filePath = Path.Combine(documentsFolder, safeFileName);

        await using (var stream = File.Create(filePath))
        {
            await file.CopyToAsync(stream, cancellationToken);
        }

        var publicBasePath = string.IsNullOrWhiteSpace(_options.PublicBasePath)
            ? "/uploads"
            : _options.PublicBasePath.TrimEnd('/');

        return new StoredFileResult
        {
            FileUrl = $"{publicBasePath}/partner-customer-documents/{safeFileName}",
            OriginalFileName = file.FileName,
            MimeType = file.ContentType,
            SizeBytes = file.Length
        };
    }

    private static string GuessExtension(string mimeType)
    {
        return mimeType switch
        {
            "application/pdf" => ".pdf",
            "image/jpeg" => ".jpg",
            "image/png" => ".png",
            "image/webp" => ".webp",
            _ => ".bin"
        };
    }
}