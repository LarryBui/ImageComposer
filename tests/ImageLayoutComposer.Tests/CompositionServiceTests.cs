using ImageLayoutComposer.Api.Services;
using ImageLayoutComposer.Shared.Models;
using Moq;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace ImageLayoutComposer.Tests;

public class CompositionServiceTests
{
    private readonly Mock<IStorageService> _mockStorage;
    private readonly CompositionService _service;

    public CompositionServiceTests()
    {
        _mockStorage = new Mock<IStorageService>();
        _service = new CompositionService(_mockStorage.Object);
    }

    [Fact]
    public async Task ComposeAsync_ReturnsNull_WhenNoImagesProvided()
    {
        var request = new CompositionRequest
        {
            Layout = GridType.TwoByTwo,
            ImageIds = new List<Guid>()
        };

        var result = await _service.ComposeAsync(request);

        Assert.Null(result);
    }

    [Fact]
    public async Task ComposeAsync_ComposesImages_WhenValidRequest()
    {
        // Arrange
        var id1 = Guid.NewGuid();
        var id2 = Guid.NewGuid();
        var request = new CompositionRequest
        {
            Layout = GridType.TwoByTwo,
            ImageIds = new List<Guid> { id1, id2 }
        };

        using var img1 = new Image<Rgba32>(100, 100);
        using var img2 = new Image<Rgba32>(100, 100);
        
        var ms1 = new MemoryStream();
        await img1.SaveAsPngAsync(ms1);
        ms1.Position = 0;

        var ms2 = new MemoryStream();
        await img2.SaveAsPngAsync(ms2);
        ms2.Position = 0;

        _mockStorage.Setup(s => s.GetImageStreamAsync(id1)).ReturnsAsync(ms1);
        _mockStorage.Setup(s => s.GetImageStreamAsync(id2)).ReturnsAsync(ms2);

        // Act
        var result = await _service.ComposeAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Length > 0);
    }
}
