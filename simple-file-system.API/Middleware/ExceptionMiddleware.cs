using System.Net;
using simple_file_system.API.Exceptions;

namespace simple_file_system.API.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            var (statusCode, title, detail) = ex is AppException appEx
                ? (appEx.StatusCode, appEx.Title, appEx.Detail)
                : (HttpStatusCode.InternalServerError, "Internal Server Error", "An unexpected error occurred.");

            await WriteResponseAsync(context, statusCode, title, detail);
        }
    }

    private static async Task WriteResponseAsync(HttpContext context, HttpStatusCode statusCode, string title, string detail)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;
        await context.Response.WriteAsJsonAsync(new { title, detail });
    }
}
