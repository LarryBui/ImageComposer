using ImageLayoutComposer.Api.Services;
using ImageLayoutComposer.Shared.Models;
using Microsoft.AspNetCore.Mvc;

namespace ImageLayoutComposer.Api.Controllers;

/// <summary>
/// API Controller for generating image grid layouts.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class LayoutsController : ControllerBase
{
    private readonly ICompositionService _compositionService;

    public LayoutsController(ICompositionService compositionService)
    {
        _compositionService = compositionService;
    }

    /// <summary>
    /// Composes images into a grid based on the provided request.
    /// </summary>
    /// <param name="request">The composition parameters including grid type and image IDs.</param>
    /// <returns>A JPEG file of the composed layout.</returns>
    [HttpPost("compose")]
    public async Task<IActionResult> Compose([FromBody] CompositionRequest request)
    {
        if (request == null || !request.ImageIds.Any())
        {
            return BadRequest("Invalid composition request.");
        }

        var resultStream = await _compositionService.ComposeAsync(request);
        if (resultStream == null)
        {
            return NotFound("One or more images not found or no images to compose.");
        }

        return File(resultStream, "image/jpeg", "composed-layout.jpg");
    }
}
