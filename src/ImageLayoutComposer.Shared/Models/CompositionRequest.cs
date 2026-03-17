namespace ImageLayoutComposer.Shared.Models;

public class CompositionRequest
{
    public GridType Layout { get; set; }
    public List<Guid> ImageIds { get; set; } = new();
}
