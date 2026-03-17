namespace ImageLayoutComposer.Shared;

public static class AppConstants
{
    // Image Composition Constants
    public const int DefaultTotalWidth = 2048;
    public const int DefaultTotalHeight = 2048;
    public const int ImageCellPadding = 10;
    
    // Upload Configuration
    public const long MaxFileSizeInBytes = 10 * 1024 * 1024; // 10MB
    
    // Network & API
    public const string CorrelationIdHeader = "X-Correlation-ID";

    // File Validation
    public static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png" };
}
