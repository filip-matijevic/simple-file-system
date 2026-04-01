using System.Net;
using simple_file_system.API.Exceptions;

namespace simple_file_system.API.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
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
        catch (AppException ex)
        {
            _logger.LogWarning(ex, "Domain exception: {Title} — {Detail}", ex.Title, ex.Detail);
            await WriteResponseAsync(context, ex.StatusCode, ex.Title, ex.Detail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception on {Method} {Path}", context.Request.Method, context.Request.Path);
            await WriteResponseAsync(context, HttpStatusCode.InternalServerError, "Internal Server Error", "An unexpected error occurred.");
        }
    }

    private static async Task WriteResponseAsync(HttpContext context, HttpStatusCode statusCode, string title, string detail)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;
        await context.Response.WriteAsJsonAsync(new { title, detail });
    }
}
