using ImageLayoutComposer.Shared.Models;

namespace ImageLayoutComposer.Api.Services;

/// <summary>
/// Service for managing image storage and metadata retrieval.
/// </summary>
public interface IStorageService
{
    /// <summary>
    /// Saves an image stream to the local filesystem and stores its metadata.
    /// </summary>
    /// <param name="content">The image file stream.</param>
    /// <param name="originalFileName">The original name of the uploaded file.</param>
    /// <param name="patientName">The name of the patient associated with the image.</param>
    /// <param name="contentType">The MIME type of the file.</param>
    /// <returns>A <see cref="ImageMetadata"/> object containing the image's unique identifier and details.</returns>
    Task<ImageMetadata> SaveImageAsync(Stream content, string originalFileName, string patientName, string contentType);

    /// <summary>
    /// Retrieves metadata for a specific image.
    /// </summary>
    /// <param name="id">The unique identifier of the image.</param>
    /// <returns>The image metadata if found; otherwise, null.</returns>
    Task<ImageMetadata?> GetImageMetadataAsync(Guid id);

    /// <summary>
    /// Retrieves the file stream for a specific image.
    /// </summary>
    /// <param name="id">The unique identifier of the image.</param>
    /// <returns>A readable stream of the image file if found; otherwise, null.</returns>
    Task<Stream?> GetImageStreamAsync(Guid id);

    /// <summary>
    /// Lists stored image metadata, optionally filtered by patient name.
    /// </summary>
    /// <param name="patientName">Optional patient name to filter by.</param>
    /// <returns>A list of <see cref="ImageMetadata"/> objects.</returns>
    Task<List<ImageMetadata>> ListImagesAsync(string? patientName = null);
}
