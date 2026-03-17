using System.Net;
using System.Text.Json;
using ImageLayoutComposer.Shared;
using ImageLayoutComposer.Shared.Models;

namespace ImageLayoutComposer.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception has occurred.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var correlationId = context.Response.Headers[AppConstants.CorrelationIdHeader].ToString();

        var response = new ApiErrorResponse
        {
            StatusCode = context.Response.StatusCode,
            Message = "An internal server error occurred. Please use the correlation ID to contact support.",
            CorrelationId = correlationId
        };

        // In development, you might want to include the actual exception message
        // if (env.IsDevelopment()) { response.Message = exception.Message; }

        var result = JsonSerializer.Serialize(response);
        return context.Response.WriteAsync(result);
    }
}
