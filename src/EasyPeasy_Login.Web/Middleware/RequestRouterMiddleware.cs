using Microsoft.AspNetCore.Http;

namespace EasyPeasy_Login.Web.Middleware;

public class RequestRouterMiddleware
{
    private readonly RequestDelegate _next;
    private const string PortalUrl = "http://192.168.100.1:8080/portal";

    public RequestRouterMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value?.ToLower() ?? string.Empty;

        if (path.StartsWith("/portal") || 
            path.StartsWith("/css") || 
            path.StartsWith("/js") || 
            path.StartsWith("/images") || 
            path.StartsWith("/lib") || 
            path.StartsWith("/api")) 
        {
            await _next(context);
            return;
        }

        context.Response.Redirect(PortalUrl);
    }
}