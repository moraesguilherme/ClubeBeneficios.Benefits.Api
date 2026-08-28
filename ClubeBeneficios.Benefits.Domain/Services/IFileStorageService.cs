using Microsoft.AspNetCore.Http;

namespace ClubeBeneficios.Benefits.Domain.Services;

public interface IFileStorageService
{
    Task<StoredFileResult> SavePartnerCustomerDocumentAsync(
        IFormFile file,
        Guid partnerCustomerId,
        CancellationToken cancellationToken = default);
}

public class StoredFileResult
{
    public string FileUrl { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
    public string MimeType { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
}