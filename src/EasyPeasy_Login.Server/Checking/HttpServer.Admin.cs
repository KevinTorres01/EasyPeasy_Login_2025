using System;
using System.Text.Json;
using System.Threading.Tasks;
using EasyPeasy_Login.Application.DTOs;

namespace EasyPeasy_Login.Server.Checking;

/// <summary>
/// HttpServer - Admin: Handles admin panel requests (GET, POST, PUT, DELETE)
/// </summary>
public partial class HttpServer
{
    /// <summary>
    /// Handles admin requests from localhost
    /// </summary>
    private async Task<string> HandleAdminRequestAsync(HttpPetition petition)
    {
        Console.WriteLine($"🔐 ADMIN request: {petition.Method} {petition.Path}");

        string path = petition.Path.ToLower();

        // ===== API Routes =====
        if (path.StartsWith("/api/"))
        {
            return await HandleAdminApiAsync(petition);
        }

        // ===== Admin Panel Pages =====
        if (path.StartsWith("/admin"))
        {
            return HandleAdminPage(petition);
        }

        // ===== Default: Redirect to admin =====
        return BuildRedirectResponse(AdminPage);
    }

    #region Admin API Routes

    private async Task<string> HandleAdminApiAsync(HttpPetition petition)
    {
        string path = petition.Path.ToLower();

        // ==================== USERS API ====================
        if (path.StartsWith("/api/users"))
        {
            return await HandleUsersApiAsync(petition);
        }

        // ==================== SESSIONS API ====================
        if (path.StartsWith("/api/sessions"))
        {
            return await HandleSessionsApiAsync(petition);
        }

        // ==================== DEVICES API ====================
        if (path.StartsWith("/api/devices"))
        {
            return await HandleDevicesApiAsync(petition);
        }

        // ==================== STATS/DASHBOARD API ====================
        if (path.StartsWith("/api/stats") || path.StartsWith("/api/dashboard"))
        {
            return await HandleStatsApiAsync(petition);
        }

        // ==================== SETTINGS API ====================
        if (path.StartsWith("/api/settings"))
        {
            return await HandleSettingsApiAsync(petition);
        }

        return Build404Response();
    }

    #endregion

    #region Users API

    private async Task<string> HandleUsersApiAsync(HttpPetition petition)
    {
        string path = petition.Path.ToLower();
        string? userId = ExtractIdFromPath(path, "/api/users/");

        return petition.Method switch
        {
            "GET" when userId == null => await GetAllUsersAsync(),
            "GET" when userId != null => await GetUserByIdAsync(userId),
            "POST" => await CreateUserAsync(petition),
            "PUT" when userId != null => await UpdateUserAsync(userId, petition),
            "DELETE" when userId != null => await DeleteUserAsync(userId),
            _ => Build405MethodNotAllowedResponse()
        };
    }

    private async Task<string> GetAllUsersAsync()
    {
        // TODO: Implement with UserManagementService
        var users = new[] 
        { 
            new { Id = "1", Username = "admin", Email = "admin@local" },
            new { Id = "2", Username = "user1", Email = "user1@local" }
        };
        return BuildJsonResponse(200, users);
    }

    private async Task<string> GetUserByIdAsync(string userId)
    {
        // TODO: Implement with UserManagementService
        var user = new { Id = userId, Username = "user", Email = "user@local" };
        return BuildJsonResponse(200, user);
    }

    private async Task<string> CreateUserAsync(HttpPetition petition)
    {
        // TODO: Parse petition.Body and create user
        // var dto = JsonSerializer.Deserialize<CreateUserRequestDto>(petition.Body);
        // await _userManagementService.CreateUser(dto);
        return BuildJsonResponse(201, new { Message = "User created", Id = Guid.NewGuid().ToString() });
    }

    private async Task<string> UpdateUserAsync(string userId, HttpPetition petition)
    {
        // TODO: Parse petition.Body and update user
        // var dto = JsonSerializer.Deserialize<UpdateUserRequestDto>(petition.Body);
        // await _userManagementService.UpdateUser(userId, dto);
        return BuildJsonResponse(200, new { Message = "User updated", Id = userId });
    }

    private async Task<string> DeleteUserAsync(string userId)
    {
        // TODO: Implement with UserManagementService
        // await _userManagementService.DeleteUser(userId);
        return BuildJsonResponse(200, new { Message = "User deleted", Id = userId });
    }

    #endregion

    #region Sessions API

    private async Task<string> HandleSessionsApiAsync(HttpPetition petition)
    {
        string path = petition.Path.ToLower();
        string? sessionId = ExtractIdFromPath(path, "/api/sessions/");

        return petition.Method switch
        {
            "GET" when sessionId == null => await GetAllSessionsAsync(),
            "GET" when sessionId != null => await GetSessionByIdAsync(sessionId),
            "POST" => await CreateSessionAsync(petition),
            "DELETE" when sessionId != null => await TerminateSessionAsync(sessionId),
            "DELETE" when path.EndsWith("/all") => await TerminateAllSessionsAsync(),
            _ => Build405MethodNotAllowedResponse()
        };
    }

    private async Task<string> GetAllSessionsAsync()
    {
        // TODO: Implement with SessionManagementService
        var sessions = new[]
        {
            new { Id = "1", MacAddress = "aa:bb:cc:dd:ee:ff", Username = "user1", IsActive = true },
            new { Id = "2", MacAddress = "11:22:33:44:55:66", Username = "user2", IsActive = true }
        };
        return BuildJsonResponse(200, sessions);
    }

    private async Task<string> GetSessionByIdAsync(string sessionId)
    {
        // TODO: Implement with SessionManagementService
        var session = new { Id = sessionId, MacAddress = "aa:bb:cc:dd:ee:ff", Username = "user1", IsActive = true };
        return BuildJsonResponse(200, session);
    }

    private async Task<string> CreateSessionAsync(HttpPetition petition)
    {
        // TODO: Parse petition.Body and create session
        return BuildJsonResponse(201, new { Message = "Session created", Id = Guid.NewGuid().ToString() });
    }

    private async Task<string> TerminateSessionAsync(string sessionId)
    {
        // TODO: Implement with SessionManagementService
        return BuildJsonResponse(200, new { Message = "Session terminated", Id = sessionId });
    }

    private async Task<string> TerminateAllSessionsAsync()
    {
        // TODO: Implement with SessionManagementService
        return BuildJsonResponse(200, new { Message = "All sessions terminated" });
    }

    #endregion

    #region Devices API

    private async Task<string> HandleDevicesApiAsync(HttpPetition petition)
    {
        string path = petition.Path.ToLower();
        string? deviceId = ExtractIdFromPath(path, "/api/devices/");

        return petition.Method switch
        {
            "GET" when deviceId == null => await GetAllDevicesAsync(),
            "GET" when deviceId != null => await GetDeviceByIdAsync(deviceId),
            "POST" when path.Contains("/block") => await BlockDeviceAsync(deviceId!),
            "POST" when path.Contains("/unblock") => await UnblockDeviceAsync(deviceId!),
            "DELETE" when deviceId != null => await RemoveDeviceAsync(deviceId),
            _ => Build405MethodNotAllowedResponse()
        };
    }

    private async Task<string> GetAllDevicesAsync()
    {
        // TODO: Implement with DeviceManagementService
        var devices = new[]
        {
            new { Id = "1", MacAddress = "aa:bb:cc:dd:ee:ff", Hostname = "iPhone-Kevin", Status = "Connected" },
            new { Id = "2", MacAddress = "11:22:33:44:55:66", Hostname = "Android-Guest", Status = "Connected" }
        };
        return BuildJsonResponse(200, devices);
    }

    private async Task<string> GetDeviceByIdAsync(string deviceId)
    {
        // TODO: Implement with DeviceManagementService
        var device = new { Id = deviceId, MacAddress = "aa:bb:cc:dd:ee:ff", Hostname = "Device", Status = "Connected" };
        return BuildJsonResponse(200, device);
    }

    private async Task<string> BlockDeviceAsync(string deviceId)
    {
        // TODO: Implement device blocking
        return BuildJsonResponse(200, new { Message = "Device blocked", Id = deviceId });
    }

    private async Task<string> UnblockDeviceAsync(string deviceId)
    {
        // TODO: Implement device unblocking
        return BuildJsonResponse(200, new { Message = "Device unblocked", Id = deviceId });
    }

    private async Task<string> RemoveDeviceAsync(string deviceId)
    {
        // TODO: Implement device removal
        return BuildJsonResponse(200, new { Message = "Device removed", Id = deviceId });
    }

    #endregion

    #region Stats/Dashboard API

    private async Task<string> HandleStatsApiAsync(HttpPetition petition)
    {
        if (petition.Method != "GET")
            return Build405MethodNotAllowedResponse();

        var stats = new
        {
            TotalUsers = 25,
            ActiveSessions = 12,
            ConnectedDevices = 15,
            BlockedDevices = 3,
            TotalDataUsage = "15.4 GB",
            Uptime = "3 days, 14 hours"
        };

        return BuildJsonResponse(200, stats);
    }

    #endregion

    #region Settings API

    private async Task<string> HandleSettingsApiAsync(HttpPetition petition)
    {
        return petition.Method switch
        {
            "GET" => await GetSettingsAsync(),
            "PUT" or "POST" => await UpdateSettingsAsync(petition),
            _ => Build405MethodNotAllowedResponse()
        };
    }

    private async Task<string> GetSettingsAsync()
    {
        var settings = new
        {
            PortalName = "EasyPeasy WiFi",
            SessionTimeout = 3600,
            MaxDevicesPerUser = 3,
            RequireEmail = false,
            EnableGuestAccess = true
        };
        return BuildJsonResponse(200, settings);
    }

    private async Task<string> UpdateSettingsAsync(HttpPetition petition)
    {
        // TODO: Parse petition.Body and update settings
        return BuildJsonResponse(200, new { Message = "Settings updated" });
    }

    #endregion

    #region Admin Pages

    private string HandleAdminPage(HttpPetition petition)
    {
        string path = petition.Path.ToLower();

        // For now, return placeholder HTML. Replace with actual Blazor/HTML pages
        if (path == "/admin" || path == "/admin/")
        {
            return BuildAdminDashboardPage();
        }
        if (path.StartsWith("/admin/users"))
        {
            return BuildAdminUsersPage();
        }
        if (path.StartsWith("/admin/sessions"))
        {
            return BuildAdminSessionsPage();
        }
        if (path.StartsWith("/admin/devices"))
        {
            return BuildAdminDevicesPage();
        }
        if (path.StartsWith("/admin/settings"))
        {
            return BuildAdminSettingsPage();
        }

        return BuildAdminDashboardPage();
    }

    private string BuildAdminDashboardPage()
    {
        string html = @"<!DOCTYPE html>
<html>
<head><title>Admin Dashboard - EasyPeasy</title></head>
<body>
    <h1>Admin Dashboard</h1>
    <nav>
        <a href='/admin/users'>Users</a> | 
        <a href='/admin/sessions'>Sessions</a> | 
        <a href='/admin/devices'>Devices</a> | 
        <a href='/admin/settings'>Settings</a>
    </nav>
    <p>Welcome to the admin panel.</p>
</body>
</html>";
        return BuildHtmlResponse(200, "OK", html);
    }

    private string BuildAdminUsersPage()
    {
        string html = @"<!DOCTYPE html>
<html>
<head><title>Users - Admin</title></head>
<body><h1>User Management</h1><p>TODO: User list and management</p></body>
</html>";
        return BuildHtmlResponse(200, "OK", html);
    }

    private string BuildAdminSessionsPage()
    {
        string html = @"<!DOCTYPE html>
<html>
<head><title>Sessions - Admin</title></head>
<body><h1>Session Management</h1><p>TODO: Active sessions list</p></body>
</html>";
        return BuildHtmlResponse(200, "OK", html);
    }

    private string BuildAdminDevicesPage()
    {
        string html = @"<!DOCTYPE html>
<html>
<head><title>Devices - Admin</title></head>
<body><h1>Device Management</h1><p>TODO: Connected devices list</p></body>
</html>";
        return BuildHtmlResponse(200, "OK", html);
    }

    private string BuildAdminSettingsPage()
    {
        string html = @"<!DOCTYPE html>
<html>
<head><title>Settings - Admin</title></head>
<body><h1>Portal Settings</h1><p>TODO: Configuration options</p></body>
</html>";
        return BuildHtmlResponse(200, "OK", html);
    }

    #endregion

    #region Helper Methods

    private string? ExtractIdFromPath(string path, string prefix)
    {
        if (!path.StartsWith(prefix))
            return null;

        string remainder = path.Substring(prefix.Length);
        int slashIndex = remainder.IndexOf('/');
        
        if (slashIndex > 0)
            return remainder.Substring(0, slashIndex);
        
        return string.IsNullOrEmpty(remainder) ? null : remainder;
    }

    #endregion
}
