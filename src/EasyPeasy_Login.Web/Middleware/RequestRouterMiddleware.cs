using Microsoft.AspNetCore.Http;
using EasyPeasy_Login.Application.Services.SessionManagement;
using EasyPeasy_Login.Application.DTOs;

namespace EasyPeasy_Login.Web.Middleware;

public class RequestRouterMiddleware
{
    private readonly RequestDelegate _next;
    private const string PortalUrl = "http://192.168.100.1:8080/portal";
    private const string AdminUrl = "http://192.168.100.1:8080/admin";

    public RequestRouterMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ISessionManagementService sessionService)
    {
        var path = context.Request.Path.Value?.ToLower() ?? string.Empty;

        // Allow static resources always
        if (path.StartsWith("/css") ||
            path.StartsWith("/js") ||
            path.StartsWith("/images") ||
            path.StartsWith("/lib"))
        {
            await _next(context);
            return;
        }

        // Check if it's a local request (from localhost/127.0.0.1)
        if (IsLocalRequest(context))
        {
            // Allow admin interface for local requests
            if (!path.StartsWith("/admin"))
            {
                context.Response.Redirect(AdminUrl);
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
                Username = string.Empty // Not needed for validation
            };
            var isActive = await sessionService.IsActiveSession(session);
            
            if (isActive)
            {
                // Let authenticated devices access internet (pass through)
                await _next(context);
                return;
            }
        }

        // Unauthenticated devices go to captive portal
        if (!path.StartsWith("/portal"))
        {
            context.Response.Redirect(PortalUrl);
            return;
        }

        await _next(context);
    }

    private bool IsLocalRequest(HttpContext context)
    {
        var connection = context.Connection;

        // Check if local IP
        if (connection.RemoteIpAddress != null)
        {
            // Localhost
            if (connection.RemoteIpAddress.Equals(connection.LocalIpAddress))
                return true;

            // Check for localhost addresses
            var remoteIp = connection.RemoteIpAddress.ToString();
            return remoteIp == "127.0.0.1" ||
                   remoteIp == "::1" ||
                   remoteIp == "localhost";
        }

        return false;
    }

    private string GetClientIpAddress(HttpContext context)
    {
        // Check for forwarded IP first
        var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwardedFor))
        {
            return forwardedFor.Split(',')[0].Trim();
        }

        // Fall back to remote IP
        return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }
}