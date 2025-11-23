using System.Diagnostics;

namespace EasyPeasy_Login.Web.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var ip = context.Connection.RemoteIpAddress?.ToString();
        
        _logger.LogInformation($"Recibiendo petición: {context.Request.Method} {context.Request.Path} desde {ip}");

        // Pasa al siguiente middleware
        await _next(context);

        stopwatch.Stop();
        _logger.LogInformation($"Finalizada petición: {context.Request.Path} - Status: {context.Response.StatusCode} - Tiempo: {stopwatch.ElapsedMilliseconds}ms");
    }
}