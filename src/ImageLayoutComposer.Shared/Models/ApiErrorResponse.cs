namespace ImageLayoutComposer.Shared.Models;

/// <summary>
/// Standard structure for all API error responses.
/// </summary>
public class ApiErrorResponse
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? CorrelationId { get; set; }
}
