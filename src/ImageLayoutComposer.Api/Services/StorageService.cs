using System.Collections.Concurrent;
using ImageLayoutComposer.Shared.Models;

namespace ImageLayoutComposer.Api.Services;

public class StorageService : IStorageService
{
    private readonly string _uploadPath;
    private readonly ConcurrentDictionary<Guid, ImageMetadata> _metadataStore = new();

    public StorageService(IWebHostEnvironment environment)
    {
        _uploadPath = Path.Combine(environment.ContentRootPath, "uploads");
        if (!Directory.Exists(_uploadPath))
        {
            Directory.CreateDirectory(_uploadPath);
        }
    }

    public async Task<ImageMetadata> SaveImageAsync(Stream content, string originalFileName, string patientName, string contentType)
    {
        var id = Guid.NewGuid();
        
        // Sanitize patient name for filesystem use
        var sanitizedPatientName = string.Join("_", patientName.Split(Path.GetInvalidFileNameChars())).Replace(" ", "_");
        
        // Final filename: {PatientName}_{GUID}{Extension}
        var fileName = $"{sanitizedPatientName}_{id}{Path.GetExtension(originalFileName)}";
        var filePath = Path.Combine(_uploadPath, fileName);

        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await content.CopyToAsync(fileStream);
        }

        var metadata = new ImageMetadata
        {
            Id = id,
            OriginalFileName = originalFileName,
            PatientName = patientName,
            ContentType = contentType,
            SizeInBytes = content.Length
        };

        _metadataStore[id] = metadata;

        return metadata;
    }

    public Task<ImageMetadata?> GetImageMetadataAsync(Guid id)
    {
        _metadataStore.TryGetValue(id, out var metadata);
        return Task.FromResult(metadata);
    }

    public Task<Stream?> GetImageStreamAsync(Guid id)
    {
        if (_metadataStore.TryGetValue(id, out var metadata))
        {
            var sanitizedPatientName = string.Join("_", metadata.PatientName.Split(Path.GetInvalidFileNameChars())).Replace(" ", "_");
            var fileName = $"{sanitizedPatientName}_{id}{Path.GetExtension(metadata.OriginalFileName)}";
            var filePath = Path.Combine(_uploadPath, fileName);
            if (File.Exists(filePath))
            {
                return Task.FromResult<Stream?>(new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read));
            }
        }
        return Task.FromResult<Stream?>(null);
    }

    public Task<List<ImageMetadata>> ListImagesAsync(string? patientName = null)
    {
        if (string.IsNullOrWhiteSpace(patientName))
        {
            return Task.FromResult(_metadataStore.Values.ToList());
        }

        return Task.FromResult(_metadataStore.Values
            .Where(m => m.PatientName.Equals(patientName, StringComparison.OrdinalIgnoreCase))
            .ToList());
    }
}
