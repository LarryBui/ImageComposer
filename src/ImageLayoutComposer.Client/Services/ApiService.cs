using System.Net.Http.Json;
using ImageLayoutComposer.Shared;
using ImageLayoutComposer.Shared.Models;

namespace ImageLayoutComposer.Client.Services;

/// <summary>
/// Client-side service to handle all communication with the backend API.
/// </summary>
public class ApiService
{
    private readonly HttpClient _http;
    private readonly NotificationService _notifier;

    /// <summary>
    /// Gets the base URL of the backend API.
    /// </summary>
    public string BaseUrl => _http.BaseAddress?.ToString() ?? "";

    public ApiService(HttpClient http, NotificationService notifier)
    {
        _http = http;
        _notifier = notifier;
    }

    private async Task<T?> SendAsync<T>(HttpRequestMessage request)
    {
        try
        {
            var response = await _http.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
                _notifier.NotifyError(error?.Message ?? $"API Error: {response.StatusCode}");
                return default;
            }
            return await response.Content.ReadFromJsonAsync<T>();
        }
        catch (Exception ex)
        {
            _notifier.NotifyError($"Connection Error: {ex.Message}");
            return default;
        }
    }

    private async Task<HttpRequestMessage> CreateRequest(HttpMethod method, string url)
    {
        var request = new HttpRequestMessage(method, url);
        request.Headers.Add(AppConstants.CorrelationIdHeader, Guid.NewGuid().ToString());
        return request;
    }

    /// <summary>
    /// Fetches the list of all uploaded images or for a specific patient.
    /// </summary>
    /// <param name="patientName">Optional patient name to filter.</param>
    /// <returns>A list of <see cref="ImageMetadata"/>.</returns>
    public async Task<List<ImageMetadata>> GetImagesAsync(string? patientName = null)
    {
        var url = "api/images";
        if (!string.IsNullOrWhiteSpace(patientName))
        {
            url += $"?patientName={Uri.EscapeDataString(patientName)}";
        }

        var request = await CreateRequest(HttpMethod.Get, url);
        return await SendAsync<List<ImageMetadata>>(request) ?? new();
    }

    /// <summary>
    /// Uploads one or more images for a specific patient.
    /// </summary>
    /// <param name="content">The multipart form content containing the image streams.</param>
    /// <param name="patientName">The name of the patient.</param>
    /// <returns>The metadata of the newly uploaded images.</returns>
    public async Task<List<ImageMetadata>> UploadImagesAsync(MultipartFormDataContent content, string patientName)
    {
        content.Add(new StringContent(patientName), "patientName");
        var request = await CreateRequest(HttpMethod.Post, "api/images/upload");
        request.Content = content;
        return await SendAsync<List<ImageMetadata>>(request) ?? new();
    }

    /// <summary>
    /// Sends a composition request to the API and returns the raw image bytes.
    /// </summary>
    /// <param name="request">The composition request parameters.</param>
    /// <returns>A byte array of the composed image.</returns>
    public async Task<byte[]> ComposeAsync(CompositionRequest request)
    {
        var apiRequest = await CreateRequest(HttpMethod.Post, "api/layouts/compose");
        apiRequest.Content = JsonContent.Create(request);
        
        try
        {
            var response = await _http.SendAsync(apiRequest);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
                _notifier.NotifyError(error?.Message ?? "Composition failed.");
                return Array.Empty<byte>();
            }
            return await response.Content.ReadAsByteArrayAsync();
        }
        catch (Exception ex)
        {
            _notifier.NotifyError($"Connection Error: {ex.Message}");
            return Array.Empty<byte>();
        }
    }
}
