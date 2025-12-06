using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using EasyPeasy_Login.Application.DTOs;
using EasyPeasy_Login.Application.Services;
using EasyPeasy_Login.Application.Services.SessionManagement;
using EasyPeasy_Login.Application.Services.UserManagement;
using EasyPeasy_Login.Application.Services.DeviceManagement;
using EasyPeasy_Login.Server.HtmlPages;
using EasyPeasy_Login.Server.HtmlPages.Admin;

namespace EasyPeasy_Login.Server.Checking;

/// <summary>
/// HTTP Server for Captive Portal - Simplified version.
/// Flow: Parse → Connectivity check? → Localhost? → Has session? → Respond
/// </summary>
public class HttpServer
{
    private readonly ISessionManagementService _sessionManagementService;
    private readonly IAuthenticationService _authenticationService;
    private readonly IUserManagementService _userManagementService;
    private readonly IDeviceManagement _deviceManagement;
    private Socket? _listener;
    private volatile bool _isRunning;

    private const string GatewayIP = "192.168.100.1";
    private const int ServerPort = 8080;
    private const string PortalUrl = "/portal/login";

    public HttpServer(
        ISessionManagementService sessionManagementService, 
        IAuthenticationService authenticationService,
        IUserManagementService userManagementService,
        IDeviceManagement deviceManagement)
    {
        _sessionManagementService = sessionManagementService;
        _authenticationService = authenticationService;
        _userManagementService = userManagementService;
        _deviceManagement = deviceManagement;
    }

    public void Start()
    {
        _listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        _listener.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        _listener.Bind(new IPEndPoint(IPAddress.Any, ServerPort));
        _listener.Listen(100);
        _isRunning = true;
        Console.WriteLine($"🚀 HttpServer on port {ServerPort}");
        _ = AcceptConnectionsAsync();
    }

    public void Stop()
    {
        _isRunning = false;
        _listener?.Close();
    }

    private async Task AcceptConnectionsAsync()
    {
        while (_isRunning && _listener != null)
        {
            try
            {
                var client = await _listener.AcceptAsync();
                _ = HandleClientAsync(client);
            }
            catch { if (!_isRunning) break; }
        }
    }

    private async Task HandleClientAsync(Socket client)
    {
        try
        {
            client.ReceiveTimeout = 3000;
            client.SendTimeout = 3000;

            var buffer = new byte[2048];
            int bytesRead = await client.ReceiveAsync(buffer, SocketFlags.None);
            if (bytesRead == 0) return;

            string rawRequest = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            string clientIP = ((IPEndPoint)client.RemoteEndPoint!).Address.ToString();

            var request = HttpPetition.Parse(rawRequest, clientIP);
            string response = await ProcessRequestAsync(request);

            await client.SendAsync(Encoding.UTF8.GetBytes(response), SocketFlags.None);
        }
        catch { }
        finally
        {
            try { client.Shutdown(SocketShutdown.Both); } catch { }
            client.Close();
        }
    }

    /// <summary>
    /// Main request flow:
    /// 1. Is connectivity check (OS/server)? → Respond based on auth
    /// 2. Admin pages → Serve HTML from HtmlPages
    /// 3. Portal pages → Serve login/success pages
    /// 4. Is from localhost? → Allow (for API access)
    /// 5. User request → Check session and respond accordingly
    /// </summary>
    private async Task<string> ProcessRequestAsync(HttpPetition request)
    {
        string path = request.Path.ToLower();

        // Handle CORS preflight requests
        if (request.Method.ToUpper() == "OPTIONS")
        {
            return HttpOptions();
        }

        // 1. Connectivity check (OS trying to detect captive portal)
        if (IsConnectivityCheck(request))
        {
            bool auth = await IsAuthenticatedAsync(request.ClientIP);
            return auth ? Http204() : RedirectToPortal();
        }

        // 2. API endpoints - Handle REST API calls from JavaScript
        if (path.StartsWith("/api/"))
        {
            return await HandleApiRequestAsync(request);
        }

        // 3. Admin pages - Serve directly from HtmlPages
        if (path.StartsWith("/admin"))
        {
            return path switch
            {
                "/admin" or "/admin/" => HttpHtml(DashboardPage.GenerateDashboard()),
                "/admin/users" => await GenerateUsersPageAsync(),
                "/admin/devices" => await GenerateDevicesPageAsync(),
                "/admin/network" => HttpHtml(NetworkControlPage.GenerateNetworkControl()),
                "/admin/settings" => HttpHtml("<h1>Settings Page - Coming Soon</h1>"),
                _ => HttpHtml("<h1>404 - Admin Page Not Found</h1>")
            };
        }

        // 4. Portal pages - Handle login
        if (path == "/portal/login" && request.Method == "POST")
            return await HandleLoginAsync(request);

        if (path.StartsWith("/portal") || path == "/")
        {
            string? portalMac = await GetMacAddressAsync(request.ClientIP);
            bool portalHasSession = !string.IsNullOrEmpty(portalMac) && 
                await _sessionManagementService.IsActiveSession(new SessionDto { MacAddress = portalMac, Username = "" });

            if (portalHasSession && path.StartsWith("/portal"))
                return HttpHtml(SuccessPage.GenerateSuccessPage(portalMac!, "You are connected!"));
            
            return HttpHtml(LoginPage.GenerateCleanLoginPage(request.ClientIP, portalMac ?? "Not detected"));
        }

        // 5. Localhost = Allow (for development access)
        if (request.IsFromLocalhost())
            return Http204();

        // 6. User request - check if has active session
        string? mac = await GetMacAddressAsync(request.ClientIP);
        bool hasSession = !string.IsNullOrEmpty(mac) && 
            await _sessionManagementService.IsActiveSession(new SessionDto { MacAddress = mac, Username = "" });

        if (hasSession)
        {
            // User authenticated - allow traffic
            return Http204();
        }

        // No session - redirect to portal
        return RedirectToPortal();
    }

    /// <summary>
    /// Handle API requests from JavaScript in admin pages
    /// </summary>
    private async Task<string> HandleApiRequestAsync(HttpPetition request)
    {
        string path = request.Path.ToLower();
        string method = request.Method.ToUpper();

        try
        {
            // Users API
            if (path == "/api/users" && method == "GET")
            {
                var users = await _userManagementService.GetAllUsersAsync();
                return HttpJson(users);
            }

            if (path == "/api/users" && method == "POST")
            {
                var body = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(request.Body);
                if (body == null) return HttpError(400, "Invalid request body");

                var result = await _userManagementService.CreateUserAsync(
                    body["username"].GetString() ?? "",
                    body["name"].GetString() ?? "",
                    body["password"].GetString() ?? ""
                );

                return result.Success ? HttpJson(result) : HttpError(400, result.Message);
            }

            if (path.StartsWith("/api/users/") && method == "PUT")
            {
                string username = path.Replace("/api/users/", "");
                var body = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(request.Body);
                if (body == null) return HttpError(400, "Invalid request body");

                var updateDto = new Application.DTOs.UpdateUserRequestDto
                {
                    Username = username,
                    Name = body.ContainsKey("name") ? body["name"].GetString() : null,
                    Password = body.ContainsKey("password") ? body["password"].GetString() : null,
                    IsActive = body.ContainsKey("isActive") ? body["isActive"].GetBoolean() : null
                };

                var result = await _userManagementService.UpdateUserAsync(updateDto);
                return result.Success ? HttpJson(result) : HttpError(400, result.Message);
            }

            if (path.StartsWith("/api/users/") && method == "DELETE")
            {
                string username = path.Replace("/api/users/", "");
                await _userManagementService.DeleteUserAsync(username);
                return HttpJson(new { success = true, message = "User deleted successfully" });
            }

            // Devices API
            if (path == "/api/device" && method == "GET")
            {
                var devices = await _deviceManagement.GetConnectedDevicesAsync();
                var deviceList = devices.Select(d => new
                {
                    macAddress = d.MacAddress,
                    ipAddress = d.IpAddress,
                    username = d.Username
                });
                return HttpJson(deviceList);
            }

            if (path.StartsWith("/api/device/") && method == "DELETE")
            {
                string macAddress = path.Replace("/api/device/", "");
                await _deviceManagement.DisconnectDeviceAsync(macAddress);
                return HttpJson(new { success = true, message = "Device removed successfully" });
            }

            // Sessions API
            if (path == "/api/session" && method == "GET")
            {
                var sessions = await _sessionManagementService.GetAllSessionsAsync();
                return HttpJson(sessions);
            }

            if (path.StartsWith("/api/session/") && method == "DELETE")
            {
                string macAddress = path.Replace("/api/session/", "");
                var session = await _sessionManagementService.GetSessionByMacAsync(macAddress);
                if (session != null)
                {
                    await _sessionManagementService.InvalidateSession(session);
                    return HttpJson(new { success = true, message = "Session terminated successfully" });
                }
                return HttpError(404, "Session not found");
            }

            // Network API - Placeholder for future implementation
            if (path == "/api/network/start" && method == "POST")
            {
                // TODO: Implement network start functionality
                // This would call NetworkOrchestrator or similar service
                return HttpJson(new { success = true, message = "Network start requested (not yet implemented)" });
            }

            if (path == "/api/network/stop" && method == "POST")
            {
                // TODO: Implement network stop functionality
                // This would call NetworkOrchestrator or similar service
                return HttpJson(new { success = true, message = "Network stop requested (not yet implemented)" });
            }

            return HttpError(404, "API endpoint not found");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"API Error: {ex.Message}");
            return HttpError(500, ex.Message);
        }
    }

    /// <summary>
    /// Detects connectivity checks from any OS (Android, iOS, Windows, Linux, etc.)
    /// </summary>
    private bool IsConnectivityCheck(HttpPetition request)
    {
        string path = request.Path.ToLower();
        string host = request.Host.ToLower();
        string ua = request.UserAgent.ToLower();

        // Known connectivity check paths
        if (path.Contains("generate_204") || path.Contains("gen_204") ||
            path is "/hotspot-detect.html" or "/ncsi.txt" or "/connecttest.txt" or 
            "/success.txt" or "/canonical.html" or "/library/test/success.html")
            return true;

        // Known connectivity check hosts
        if (host.Contains("apple.com") || host.Contains("gstatic.com") || 
            host.Contains("msft") || host.Contains("firefox") ||
            host.Contains("ubuntu") || host.Contains("android"))
            return true;

        // Known connectivity check user agents
        if (ua.Contains("captivenetworksupport") || ua.Contains("microsoft ncsi") ||
            ua.Contains("dalvik") || string.IsNullOrEmpty(request.UserAgent))
            return true;

        return false;
    }

    private async Task<string> HandleLoginAsync(HttpPetition request)
    {
        try
        {
            // Parse form data from body
            var formData = ParseFormData(request.Body);
            
            string username = formData.GetValueOrDefault("username", "");
            string password = formData.GetValueOrDefault("password", "");
            string clientMac = formData.GetValueOrDefault("client_mac", "");

            if (string.IsNullOrEmpty(clientMac))
                clientMac = await GetMacAddressAsync(request.ClientIP) ?? "unknown";

            // 1. Validate credentials with AuthenticationService
            var loginResult = await _authenticationService.AuthenticateAsync(new LoginRequestDto
            {
                Username = username,
                Password = password,
                MacAddress = clientMac,
                IpAddress = request.ClientIP
            });

            if (!loginResult.Success)
            {
                return HttpHtml(LoginPage.GenerateLoginPageWithError(loginResult.Message, username, request.ClientIP, clientMac));
            }
            // Redirect to success page
            return $"HTTP/1.1 302 Found\r\nLocation: http://{GatewayIP}:{ServerPort}/portal/success\r\nContent-Length: 0\r\n\r\n";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Login error: {ex.Message}");
            return HttpHtml(LoginPage.GenerateLoginPageWithError("An error occurred during login. Please try again.", "", request.ClientIP, ""));
        }
    }

    private Dictionary<string, string> ParseFormData(string body)
    {
        var result = new Dictionary<string, string>();
        if (string.IsNullOrEmpty(body)) return result;

        var pairs = body.Split('&');
        foreach (var pair in pairs)
        {
            var keyValue = pair.Split('=');
            if (keyValue.Length == 2)
            {
                string key = Uri.UnescapeDataString(keyValue[0]);
                string value = Uri.UnescapeDataString(keyValue[1]);
                result[key] = value;
            }
        }
        return result;
    }

    private async Task<bool> IsAuthenticatedAsync(string clientIP)
    {
        string? mac = await GetMacAddressAsync(clientIP);
        if (string.IsNullOrEmpty(mac)) return false;
        return await _sessionManagementService.IsActiveSession(new SessionDto { MacAddress = mac, Username = "" });
    }

    private async Task<string?> GetMacAddressAsync(string ip)
    {
        try
        {
            var p = new Process
            {
                StartInfo = new()
                {
                    FileName = "/bin/bash",
                    Arguments = $"-c \"ip neigh show {ip}\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            p.Start();
            string output = await p.StandardOutput.ReadToEndAsync();
            await p.WaitForExitAsync();

            var match = Regex.Match(output, @"([0-9A-Fa-f]{2}[:-]){5}[0-9A-Fa-f]{2}");
            return match.Success ? match.Value.ToLower() : null;
        }
        catch { return null; }
    }

    #region HTTP Responses

    private string Http204() => "HTTP/1.1 204 No Content\r\nContent-Length: 0\r\n\r\n";

    private string HttpOptions() => "HTTP/1.1 204 No Content\r\nAccess-Control-Allow-Origin: *\r\nAccess-Control-Allow-Methods: GET, POST, PUT, DELETE, OPTIONS\r\nAccess-Control-Allow-Headers: Content-Type\r\nContent-Length: 0\r\n\r\n";

    private string RedirectToPortal()
    {
        // Redirect to portal on port 8080 (HttpServer serves the HTML)
        string url = $"http://{GatewayIP}:{ServerPort}/portal/login";
        return $"HTTP/1.1 302 Found\r\nLocation: {url}\r\nContent-Length: 0\r\n\r\n";
    }

    private string HttpHtml(string html)
    {
        int len = Encoding.UTF8.GetByteCount(html);
        return $"HTTP/1.1 200 OK\r\nContent-Type: text/html\r\nContent-Length: {len}\r\n\r\n{html}";
    }

    private string HttpJson(object data)
    {
        string json = JsonSerializer.Serialize(data);
        int len = Encoding.UTF8.GetByteCount(json);
        return $"HTTP/1.1 200 OK\r\nContent-Type: application/json\r\nAccess-Control-Allow-Origin: *\r\nAccess-Control-Allow-Methods: GET, POST, PUT, DELETE, OPTIONS\r\nAccess-Control-Allow-Headers: Content-Type\r\nContent-Length: {len}\r\n\r\n{json}";
    }

    private string HttpError(int statusCode, string message)
    {
        var errorObj = new { error = message };
        string json = JsonSerializer.Serialize(errorObj);
        int len = Encoding.UTF8.GetByteCount(json);
        string statusText = statusCode switch
        {
            400 => "Bad Request",
            404 => "Not Found",
            500 => "Internal Server Error",
            _ => "Error"
        };
        return $"HTTP/1.1 {statusCode} {statusText}\r\nContent-Type: application/json\r\nAccess-Control-Allow-Origin: *\r\nAccess-Control-Allow-Methods: GET, POST, PUT, DELETE, OPTIONS\r\nAccess-Control-Allow-Headers: Content-Type\r\nContent-Length: {len}\r\n\r\n{json}";
    }

    #endregion

    #region Admin Page Generators

    private async Task<string> GenerateUsersPageAsync()
    {
        try
        {
            var users = await _userManagementService.GetAllUsersAsync();
            var usersList = users.Select(u => (u.Name, u.Username, u.IsActive)).ToList();
            return HttpHtml(UserManagementPage.GenerateUserManagement(usersList));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading users: {ex.Message}");
            return HttpHtml(UserManagementPage.GenerateUserManagement());
        }
    }

    private async Task<string> GenerateDevicesPageAsync()
    {
        try
        {
            var devices = await _deviceManagement.GetConnectedDevicesAsync();
            var devicesList = devices.Select(d => (d.IpAddress, d.MacAddress, d.Username)).ToList();
            return HttpHtml(DevicesPage.GenerateDevices(devicesList));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading devices: {ex.Message}");
            return HttpHtml(DevicesPage.GenerateDevices());
        }
    }

    #endregion
}
