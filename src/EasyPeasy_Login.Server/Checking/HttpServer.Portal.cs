using System;
using System.Threading.Tasks;
using EasyPeasy_Login.Application.DTOs;

namespace EasyPeasy_Login.Server.Checking;

/// <summary>
/// HttpServer - Portal: Handles remote client requests (captive portal users)
/// </summary>
public partial class HttpServer
{
    /// <summary>
    /// Handles remote requests - checks authentication by MAC
    /// </summary>
    private async Task<string> HandleRemoteRequestAsync(HttpPetition petition)
    {
        Console.WriteLine($"🌐 REMOTE request from {petition.ClientIP}: {petition.Method} {petition.Path}");

        // 1. Get MAC address from IP using ARP table
        string? macAddress = await GetMacAddressFromIpAsync(petition.ClientIP);

        if (string.IsNullOrEmpty(macAddress))
        {
            Console.WriteLine($"⚠️ Can't obtain MAC for IP: {petition.ClientIP}");
            return BuildRedirectResponse(PortalLoginPage);
        }

        Console.WriteLine($"🔍 MAC detected: {macAddress} for IP: {petition.ClientIP}");

        // 2. Check if there is an active session for this MAC
        var sessionDto = new SessionDto
        {
            MacAddress = macAddress,
            Username = string.Empty
        };

        bool isAuthenticated = await _sessionManagementService.IsActiveSession(sessionDto);

        if (isAuthenticated)
        {
            return HandleAuthenticatedRequest(petition, macAddress);
        }
        else
        {
            return HandleUnauthenticatedRequest(petition, macAddress);
        }
    }

    #region Authenticated User Requests

    private string HandleAuthenticatedRequest(HttpPetition petition, string macAddress)
    {
        Console.WriteLine($"✅ Device {macAddress} AUTHENTICATED");
        string path = petition.Path.ToLower();

        // Portal pages for authenticated users
        if (path.StartsWith("/portal"))
        {
            if (path == "/portal/logout")
            {
                return HandleLogoutPage(macAddress);
            }
            if (path == "/portal/status")
            {
                return BuildStatusPage(macAddress);
            }
            return BuildSuccessPage(macAddress);
        }

        // Static files
        if (IsStaticFileRequest(path))
        {
            return HandleStaticFile(path);
        }

        // API endpoints for portal users
        if (path.StartsWith("/api/portal/"))
        {
            return HandlePortalApi(petition, macAddress);
        }

        // Any other request - show success page (user has internet)
        return BuildAllowedResponse();
    }

    #endregion

    #region Unauthenticated User Requests

    private string HandleUnauthenticatedRequest(HttpPetition petition, string macAddress)
    {
        Console.WriteLine($"🔒 Device {macAddress} NOT AUTHENTICATED - redirecting to login");
        string path = petition.Path.ToLower();

        // Allow access to portal login pages
        if (path.StartsWith("/portal"))
        {
            if (path == "/portal/login" && petition.Method == "POST")
            {
                return HandleLoginPost(petition, macAddress);
            }
            if (path == "/portal/register" && petition.Method == "POST")
            {
                return HandleRegisterPost(petition, macAddress);
            }
            return BuildLoginPage(macAddress, petition.ClientIP);
        }

        // Allow static files (CSS, JS for login page)
        if (IsStaticFileRequest(path))
        {
            return HandleStaticFile(path);
        }

        // Redirect everything else to login
        return BuildRedirectResponse(PortalLoginPage);
    }

    #endregion

    #region Portal API

    private string HandlePortalApi(HttpPetition petition, string macAddress)
    {
        string path = petition.Path.ToLower();

        if (path == "/api/portal/status")
        {
            return BuildJsonResponse(200, new
            {
                Authenticated = true,
                MacAddress = macAddress,
                Message = "You are connected"
            });
        }

        if (path == "/api/portal/logout" && petition.Method == "POST")
        {
            // TODO: Terminate session
            return BuildJsonResponse(200, new { Message = "Logged out successfully" });
        }

        return Build404Response();
    }

    #endregion

    #region Authentication Handlers

    private string HandleLoginPost(HttpPetition petition, string macAddress)
    {
        // TODO: Parse petition.Body for username/password
        // TODO: Validate credentials with AuthenticationService
        // TODO: Create session with SessionManagementService

        Console.WriteLine($"🔑 Login attempt from MAC: {macAddress}");

        // Placeholder response - implement actual authentication
        return BuildJsonResponse(200, new
        {
            Success = true,
            Message = "Login successful",
            RedirectUrl = "/portal/success"
        });
    }

    private string HandleRegisterPost(HttpPetition petition, string macAddress)
    {
        // TODO: Parse petition.Body for registration data
        // TODO: Create user with UserManagementService
        // TODO: Create session

        Console.WriteLine($"📝 Registration attempt from MAC: {macAddress}");

        // Placeholder response
        return BuildJsonResponse(201, new
        {
            Success = true,
            Message = "Registration successful",
            RedirectUrl = "/portal/success"
        });
    }

    private string HandleLogoutPage(string macAddress)
    {
        // TODO: Terminate session
        Console.WriteLine($"👋 Logout from MAC: {macAddress}");
        return BuildRedirectResponse(PortalLoginPage);
    }

    #endregion

    #region Static Files

    private bool IsStaticFileRequest(string path)
    {
        return path.EndsWith(".css") ||
               path.EndsWith(".js") ||
               path.EndsWith(".png") ||
               path.EndsWith(".jpg") ||
               path.EndsWith(".jpeg") ||
               path.EndsWith(".gif") ||
               path.EndsWith(".svg") ||
               path.EndsWith(".woff") ||
               path.EndsWith(".woff2") ||
               path.EndsWith(".ttf") ||
               path.EndsWith(".ico") ||
               path.StartsWith("/static/") ||
               path.StartsWith("/assets/") ||
               path.StartsWith("/css/") ||
               path.StartsWith("/js/") ||
               path.StartsWith("/images/");
    }

    private string HandleStaticFile(string path)
    {
        // TODO: Implement actual static file serving
        // For now, return 404 - files would be served from wwwroot
        Console.WriteLine($"📁 Static file request: {path}");
        return Build404Response();
    }

    #endregion

    #region Portal Pages

    private string BuildLoginPage(string macAddress, string clientIp)
    {
        string html = $@"<!DOCTYPE html>
<html>
<head>
    <title>WiFi Login - EasyPeasy</title>
    <meta name='viewport' content='width=device-width, initial-scale=1'>
    <style>
        body {{ font-family: Arial, sans-serif; max-width: 400px; margin: 50px auto; padding: 20px; }}
        .form-group {{ margin-bottom: 15px; }}
        input {{ width: 100%; padding: 10px; box-sizing: border-box; }}
        button {{ width: 100%; padding: 12px; background: #007bff; color: white; border: none; cursor: pointer; }}
        button:hover {{ background: #0056b3; }}
    </style>
</head>
<body>
    <h1>WiFi Login</h1>
    <form method='POST' action='/portal/login'>
        <input type='hidden' name='mac' value='{macAddress}'>
        <div class='form-group'>
            <label>Username:</label>
            <input type='text' name='username' required>
        </div>
        <div class='form-group'>
            <label>Password:</label>
            <input type='password' name='password' required>
        </div>
        <button type='submit'>Connect</button>
    </form>
    <p><a href='/portal/register'>Create account</a></p>
</body>
</html>";
        return BuildHtmlResponse(200, "OK", html);
    }

    private string BuildSuccessPage(string macAddress)
    {
        string html = $@"<!DOCTYPE html>
<html>
<head>
    <title>Connected - EasyPeasy</title>
    <meta name='viewport' content='width=device-width, initial-scale=1'>
</head>
<body>
    <h1>✅ You are connected!</h1>
    <p>Your device ({macAddress}) is now connected to the internet.</p>
    <p><a href='/portal/logout'>Disconnect</a></p>
</body>
</html>";
        return BuildHtmlResponse(200, "OK", html);
    }

    private string BuildStatusPage(string macAddress)
    {
        string html = $@"<!DOCTYPE html>
<html>
<head>
    <title>Status - EasyPeasy</title>
</head>
<body>
    <h1>Connection Status</h1>
    <p>Device: {macAddress}</p>
    <p>Status: Connected</p>
    <p><a href='/portal/logout'>Disconnect</a></p>
</body>
</html>";
        return BuildHtmlResponse(200, "OK", html);
    }

    private string BuildAllowedResponse()
    {
        string html = @"<!DOCTYPE html>
<html>
<head><title>Connected</title></head>
<body>
    <h1>You have internet access</h1>
    <p><a href='/portal/status'>View connection status</a></p>
</body>
</html>";
        return BuildHtmlResponse(200, "OK", html);
    }

    #endregion
}
