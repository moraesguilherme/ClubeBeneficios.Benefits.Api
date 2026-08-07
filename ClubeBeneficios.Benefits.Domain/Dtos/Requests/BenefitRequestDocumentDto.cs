namespace ClubeBeneficios.Benefits.Domain.Dtos;

public class BenefitRequestDocumentDto
{
    public Guid? Id { get; set; }
    public string? SourceType { get; set; }

    public Guid? ClientDocumentId { get; set; }
    public Guid? PartnerCustomerDocumentId { get; set; }

    public string? FileUrl { get; set; }
    public string? FileName { get; set; }

    public string? SubmissionStatus { get; set; }
    public string? Notes { get; set; }

    public DateTime? ReviewedAt { get; set; }
    public Guid? ReviewedByUserId { get; set; }
}