namespace ImageLayoutComposer.Shared.Models;

public class ImageMetadata
{
    public Guid Id { get; set; }
    public string OriginalFileName { get; set; } = string.Empty;
    public string PatientName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long SizeInBytes { get; set; }
    public string? DownloadUrl { get; set; }
}
