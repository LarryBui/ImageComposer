using ImageLayoutComposer.Shared.Models;

namespace ImageLayoutComposer.Api.Services;

/// <summary>
/// Service for composing multiple images into a grid layout.
/// </summary>
public interface ICompositionService
{
    /// <summary>
    /// Processes a composition request and returns the resulting image stream.
    /// </summary>
    /// <param name="request">The composition parameters including grid type and image IDs.</param>
    /// <returns>A readable JPEG image stream of the composed grid if successful; otherwise, null.</returns>
    Task<Stream?> ComposeAsync(CompositionRequest request);
}
