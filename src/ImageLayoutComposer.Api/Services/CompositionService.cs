using ImageLayoutComposer.Shared;
using ImageLayoutComposer.Shared.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace ImageLayoutComposer.Api.Services;

/// <summary>
/// Implementation of ICompositionService using SixLabors.ImageSharp.
/// Handles the core image processing logic for grid composition.
/// </summary>
public class CompositionService : ICompositionService
{
    private readonly IStorageService _storageService;

    /// <summary>
    /// Initializes a new instance of the <see cref="CompositionService"/> class.
    /// </summary>
    /// <param name="storageService">The storage service to retrieve images from.</param>
    public CompositionService(IStorageService storageService)
    {
        _storageService = storageService;
    }

    /// <summary>
    /// Composes a grid of images from the provided identifiers.
    /// Resizes images to fit cell dimensions while maintaining aspect ratio.
    /// </summary>
    /// <param name="request">The composition parameters including grid type and image IDs.</param>
    /// <returns>A JPEG image stream if successful; otherwise, null.</returns>
    public async Task<Stream?> ComposeAsync(CompositionRequest request)
    {
        var (rows, cols) = GetGridDimensions(request.Layout);
        var imagesToCompose = new List<Image>();

        try
        {
            // Load and pre-process images
            foreach (var id in request.ImageIds)
            {
                using var imageStream = await _storageService.GetImageStreamAsync(id);
                if (imageStream != null)
                {
                    var image = await Image.LoadAsync(imageStream);
                    imagesToCompose.Add(image);
                }
            }

            if (!imagesToCompose.Any()) return null;

            // Define final image size from constants
            const int totalWidth = AppConstants.DefaultTotalWidth;
            const int totalHeight = AppConstants.DefaultTotalHeight;
            const int padding = AppConstants.ImageCellPadding;

            int cellWidth = totalWidth / cols;
            int cellHeight = totalHeight / rows;

            var outputImage = new Image<Rgba32>(totalWidth, totalHeight);
            try
            {
                // Clear background to white for a consistent look
                outputImage.Mutate(x => x.Fill(Color.White));

                for (int i = 0; i < imagesToCompose.Count && i < rows * cols; i++)
                {
                    int row = i / cols;
                    int col = i % cols;

                    var currentImage = imagesToCompose[i];
                    
                    // Calculate available space within the cell minus padding
                    int availableWidth = cellWidth - (padding * 2);
                    int availableHeight = cellHeight - (padding * 2);

                    // Resize image to fit the padded cell while preserving aspect ratio
                    currentImage.Mutate(x => x.Resize(new ResizeOptions
                    {
                        Size = new Size(availableWidth, availableHeight),
                        Mode = ResizeMode.Max
                    }));

                    // Calculate position to center the image within its grid cell, including padding
                    int xOffset = (col * cellWidth) + padding + (availableWidth - currentImage.Width) / 2;
                    int yOffset = (row * cellHeight) + padding + (availableHeight - currentImage.Height) / 2;

                    // Draw current image onto the master canvas
                    outputImage.Mutate(x => x.DrawImage(currentImage, new Point(xOffset, yOffset), 1f));
                }

                // Return final image as JPEG stream
                var outputStream = new MemoryStream();
                await outputImage.SaveAsJpegAsync(outputStream);
                outputStream.Position = 0;
                return outputStream;
            }
            finally
            {
                outputImage.Dispose();
            }
        }
        finally
        {
            // Clean up loaded image resources
            foreach (var img in imagesToCompose)
            {
                img.Dispose();
            }
        }
    }

    private (int rows, int cols) GetGridDimensions(GridType layout)
    {
        return layout switch
        {
            GridType.TwoByTwo => (2, 2),
            GridType.ThreeByThree => (3, 3),
            GridType.FourByFour => (4, 4),
            _ => (2, 2)
        };
    }
}
