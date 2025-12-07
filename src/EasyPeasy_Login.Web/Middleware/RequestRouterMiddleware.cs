using Microsoft.AspNetCore.Http;
using EasyPeasy_Login.Application.Services.SessionManagement;
using EasyPeasy_Login.Application.DTOs;

namespace EasyPeasy_Login.Web.Middleware;

public class RequestRouterMiddleware
{
    private readonly RequestDelegate _next;

    public RequestRouterMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ISessionManagementService sessionService)
    {
        var path = context.Request.Path.Value?.ToLower() ?? string.Empty;

        // Allow Blazor framework files, SignalR, and static resources
        if (path.StartsWith("/_framework") ||
            path.StartsWith("/_blazor") ||
            path.StartsWith("/_content") ||
            path.StartsWith("/css") ||
            path.StartsWith("/js") ||
            path.StartsWith("/images") ||
            path.StartsWith("/lib") ||
            path.EndsWith(".css") ||
            path.EndsWith(".js") ||
            path.EndsWith(".map"))
        {
            await _next(context);
            return;
        }

        // Check if it's a local request
        if (IsLocalRequest(context))
        {
            // Allow admin interface for local requests
            if (!path.StartsWith("/admin"))
            {
                context.Response.Redirect("/admin");
                return;
            }
            await _next(context);
            return;
        }

        // Check if device has an active session by MAC address
        var macAddress = context.Request.Cookies["device_mac"];
        if (!string.IsNullOrEmpty(macAddress))
        {
            var session = new SessionDto 
            { 
                MacAddress = macAddress,
                Username = string.Empty
            };
            var isActive = await sessionService.IsActiveSession(session);
            
            if (isActive)
            {
                // Let authenticated devices access internet
                await _next(context);
                return;
            }
        }

        // Unauthenticated devices go to captive portal
        if (!path.StartsWith("/portal"))
        {
            context.Response.Redirect("/portal");
            return;
        }

        await _next(context);
    }

    private bool IsLocalRequest(HttpContext context)
    {
        var connection = context.Connection;

        if (connection.RemoteIpAddress != null)
        {
            if (connection.RemoteIpAddress.Equals(connection.LocalIpAddress))
                return true;

            var remoteIp = connection.RemoteIpAddress.ToString();
            return remoteIp == "127.0.0.1" ||
                   remoteIp == "::1" ||
                   remoteIp == "localhost";
        }

        return false;
    }
}