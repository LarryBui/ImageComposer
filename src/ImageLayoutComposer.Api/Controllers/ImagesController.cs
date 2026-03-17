using ImageLayoutComposer.Api.Services;
using ImageLayoutComposer.Shared;
using ImageLayoutComposer.Shared.Models;
using Microsoft.AspNetCore.Mvc;

namespace ImageLayoutComposer.Api.Controllers;

/// <summary>
/// API Controller for managing uploaded image files.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ImagesController : ControllerBase
{
    private readonly IStorageService _storageService;
    private readonly ILogger<ImagesController> _logger;

    public ImagesController(IStorageService storageService, ILogger<ImagesController> logger)
    {
        _storageService = storageService;
        _logger = logger;
    }

    /// <summary>
    /// Uploads multiple images for a specific patient.
    /// Files are limited by AppConstants.MaxFileSizeInBytes.
    /// </summary>
    /// <param name="files">The list of image files from the form body.</param>
    /// <param name="patientName">The name of the patient (required).</param>
    /// <returns>A list of metadata objects for the successfully uploaded images.</returns>
    [HttpPost("upload")]
    public async Task<ActionResult<List<ImageMetadata>>> Upload(List<IFormFile> files, [FromForm] string patientName)
    {
        _logger.LogInformation("Uploading {Count} files for patient {PatientName}", files?.Count ?? 0, patientName);
        if (files == null || !files.Any())
        {
            return BadRequest("No files uploaded.");
        }

        if (string.IsNullOrWhiteSpace(patientName))
        {
            return BadRequest("Patient name is required.");
        }

        var results = new List<ImageMetadata>();
        foreach (var file in files)
        {
            _logger.LogDebug("Processing file {FileName}, size {Size}", file.FileName, file.Length);
            // Simple validation for file size from constants
            // more advanced image validation can be added here
            if (file.Length > AppConstants.MaxFileSizeInBytes)
            {
                _logger.LogWarning("File {FileName} exceeds size limit", file.FileName);
                continue;
            }

            
            using var stream = file.OpenReadStream();
            var metadata = await _storageService.SaveImageAsync(stream, file.FileName, patientName, file.ContentType);
            results.Add(metadata);
        }

        return Ok(results);
    }

    /// <summary>
    /// Lists uploaded images metadata, optionally filtered by patient name.
    /// Populates DownloadUrl for each metadata object.
    /// </summary>
    /// <param name="patientName">Optional name to filter images.</param>
    /// <returns>A list of image metadata.</returns>
    [HttpGet]
    public async Task<ActionResult<List<ImageMetadata>>> List([FromQuery] string? patientName)
    {
        var images = await _storageService.ListImagesAsync(patientName);
        foreach (var image in images)
        {
            image.DownloadUrl = Url.Action(nameof(Get), new { id = image.Id });
        }
        return Ok(images);
    }

    /// <summary>
    /// Temporary endpoint to test the global exception handler.
    /// </summary>
    [HttpGet("test-error")]
    public IActionResult TestError()
    {
        _logger.LogInformation("TestError endpoint called - about to throw a simulated exception.");
        throw new Exception("This is a simulated enterprise-level crash for testing purposes!");
    }

    /// <summary>
    /// Downloads the original uploaded image by its unique ID.
    /// </summary>
    /// <param name="id">The unique identifier of the image.</param>
    /// <returns>The file stream with appropriate content type.</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var metadata = await _storageService.GetImageMetadataAsync(id);
        if (metadata == null) return NotFound();

        var stream = await _storageService.GetImageStreamAsync(id);
        if (stream == null) return NotFound();

        return File(stream, metadata.ContentType, metadata.OriginalFileName);
    }
}
