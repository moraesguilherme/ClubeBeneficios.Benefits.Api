namespace ClubeBeneficios.Benefits.Domain.Options;

public class FileStorageOptions
{
    public string Provider { get; set; } = "Local";
    public string LocalRootPath { get; set; } = string.Empty;
    public string PublicBasePath { get; set; } = "/uploads";
}