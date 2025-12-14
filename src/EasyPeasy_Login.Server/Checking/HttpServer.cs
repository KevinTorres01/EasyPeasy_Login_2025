using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using EasyPeasy_Login.Application.DTOs;
using EasyPeasy_Login.Application.Services;
using EasyPeasy_Login.Application.Services.SessionManagement;
using EasyPeasy_Login.Application.Services.UserManagement;
using EasyPeasy_Login.Application.Services.DeviceManagement;
using EasyPeasy_Login.Infrastructure.Network.Configuration;
using EasyPeasy_Login.Server.HtmlPages;
using EasyPeasy_Login.Server.HtmlPages.Admin;
using EasyPeasy_Login.Shared;

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
    private readonly INetworkConfiguration _networkConfiguration;
    private readonly IMacAddressResolver _macAddressResolver;
    private readonly ILogger _logger;
    private readonly ApiRouter _apiRouter;
    private Socket? _listener;
    private volatile bool _isRunning;

    private const string GatewayIP = "192.168.100.1";
    private const int ServerPort = 8080;
    private const string PortalUrl = "/portal/login";
    private const int MaxRequestSizeBytes = 4 * 1024 * 1024; // 4 MB safety cap
    private static readonly byte[] HeaderDelimiter = Encoding.ASCII.GetBytes("\r\n\r\n");

    public HttpServer(
        ISessionManagementService sessionManagementService, 
        IAuthenticationService authenticationService,
        IUserManagementService userManagementService,
        IDeviceManagement deviceManagement,
        INetworkOrchestrator networkOrchestrator,
        INetworkConfiguration networkConfiguration,
        IMacAddressResolver macAddressResolver,
        ILogger logger)
    {
        _sessionManagementService = sessionManagementService;
        _authenticationService = authenticationService;
        _userManagementService = userManagementService;
        _deviceManagement = deviceManagement;
        _networkConfiguration = networkConfiguration;
        _macAddressResolver = macAddressResolver;
        _logger = logger;
        _apiRouter = new ApiRouter(sessionManagementService, authenticationService, userManagementService, deviceManagement, networkOrchestrator, networkConfiguration, macAddressResolver, logger);
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

            string clientIP = ((IPEndPoint)client.RemoteEndPoint!).Address.ToString();

            string? rawRequest = await ReadHttpRequestAsync(client);
            if (string.IsNullOrEmpty(rawRequest)) return;

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
    /// Reads an HTTP request handling cases where headers and body arrive in separate chunks.
    /// Supports Content-Length and Transfer-Encoding: chunked bodies with a safety size cap.
    /// </summary>
    private async Task<string?> ReadHttpRequestAsync(Socket client)
    {
        var buffer = new byte[4096];
        using var ms = new MemoryStream();
        int headerEnd = -1;

        while (ms.Length < MaxRequestSizeBytes)
        {
            int read = await client.ReceiveAsync(buffer, SocketFlags.None);
            if (read <= 0) break;

            ms.Write(buffer, 0, read);

            if (headerEnd < 0)
            {
                headerEnd = FindHeaderEnd(ms.GetBuffer(), (int)ms.Length);
                if (headerEnd >= 0) break;
            }
        }

        if (headerEnd < 0) return null;

        byte[] rawBytes = ms.ToArray();
        string headerText = Encoding.UTF8.GetString(rawBytes, 0, headerEnd);
        bool isChunked = headerText.Contains("transfer-encoding: chunked", StringComparison.OrdinalIgnoreCase);
        int contentLength = ParseContentLength(headerText);

        int bodyStart = headerEnd + HeaderDelimiter.Length;

        // If Content-Length is specified, keep reading until the full body arrives.
        while (!isChunked && contentLength > (ms.Length - bodyStart) && ms.Length < MaxRequestSizeBytes)
        {
            int read = await client.ReceiveAsync(buffer, SocketFlags.None);
            if (read <= 0) break;
            ms.Write(buffer, 0, read);
        }

        if (!isChunked && contentLength > (ms.Length - bodyStart)) return null;

        rawBytes = ms.ToArray();
        string rawRequest = Encoding.UTF8.GetString(rawBytes, 0, rawBytes.Length);

        // Handle chunked transfer encoding: keep reading until the terminating 0-length chunk arrives.
        if (isChunked)
        {
            while (!rawRequest.Contains("\r\n0\r\n\r\n", StringComparison.Ordinal) && ms.Length < MaxRequestSizeBytes)
            {
                int read = await client.ReceiveAsync(buffer, SocketFlags.None);
                if (read <= 0) break;
                ms.Write(buffer, 0, read);

                rawBytes = ms.ToArray();
                rawRequest = Encoding.UTF8.GetString(rawBytes, 0, rawBytes.Length);
            }

            if (!rawRequest.Contains("\r\n0\r\n\r\n", StringComparison.Ordinal)) return null;

            int bodyIndex = rawRequest.IndexOf("\r\n\r\n", StringComparison.Ordinal);
            if (bodyIndex >= 0)
            {
                string headerPart = rawRequest[..bodyIndex];
                string chunkedBody = rawRequest[(bodyIndex + 4)..];
                string decodedBody = DecodeChunkedBody(chunkedBody);
                rawRequest = $"{headerPart}\r\n\r\n{decodedBody}";
            }
        }

        return rawRequest;
    }

    private static int FindHeaderEnd(byte[] buffer, int length)
    {
        for (int i = 0; i <= length - HeaderDelimiter.Length; i++)
        {
            bool match = true;
            for (int j = 0; j < HeaderDelimiter.Length; j++)
            {
                if (buffer[i + j] != HeaderDelimiter[j])
                {
                    match = false;
                    break;
                }
            }

            if (match) return i;
        }

        return -1;
    }

    private static int ParseContentLength(string headers)
    {
        var match = Regex.Match(headers, @"(?im)^content-length:\s*(\d+)");
        if (match.Success && int.TryParse(match.Groups[1].Value, out int len))
            return len;
        return 0;
    }

    private static string DecodeChunkedBody(string chunkedBody)
    {
        var reader = new StringReader(chunkedBody);
        var sb = new StringBuilder();

        while (true)
        {
            string? sizeLine = reader.ReadLine();
            if (sizeLine == null) break;

            if (!int.TryParse(sizeLine.Trim(), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int chunkSize))
                break;

            if (chunkSize == 0) break;

            var chunkBuffer = new char[chunkSize];
            int read = reader.ReadBlock(chunkBuffer, 0, chunkSize);
            sb.Append(chunkBuffer, 0, read);

            reader.ReadLine(); // consume trailing CRLF
        }

        return sb.ToString();
    }

    /// <summary>
    /// Main request flow:
    /// 1. Is connectivity check (OS/server)? → Respond based on auth
    /// 2. API endpoints → Delegate to ApiRouter
    /// 3. Localhost (non-admin) → Redirect to admin dashboard
    /// 4. Admin pages → Serve HTML from HtmlPages
    /// 5. Portal pages → Serve login/success pages
    /// 6. User request → Check session and respond accordingly
    /// </summary>
    private async Task<string> ProcessRequestAsync(HttpPetition request)
    {
        string path = request.Path.ToLower();

        // Handle CORS preflight requests
        if (request.Method.ToUpper() == "OPTIONS")
        {
            return ApiResponseBuilder.HttpOptions();
        }

        // 1. Connectivity check (OS trying to detect captive portal)
        if (IsConnectivityCheck(request))
        {
            bool auth = await IsAuthenticatedAsync(request.ClientIP);
            return auth ? ApiResponseBuilder.Http204() : ApiResponseBuilder.RedirectToPortal(GatewayIP, ServerPort);
        }

        // 2. API endpoints - Delegate to ApiRouter
        if (path.StartsWith("/api/"))
        {
            return await _apiRouter.HandleAsync(request);
        }

        // 3. Localhost requests (non-admin) should go to admin dashboard
        if (request.IsFromLocalhost() && !path.StartsWith("/admin"))
        {
            string adminUrl = $"http://localhost:{ServerPort}/admin";
            return ApiResponseBuilder.Redirect(adminUrl);
        }

        // 4. Admin pages - Serve directly from HtmlPages (local access only)
        if (path.StartsWith("/admin"))
        {
            if (!request.IsFromLocalhost())
            {
                return ApiResponseBuilder.HttpError(403, "Admin access restricted to localhost");
            }

            return path switch
            {
                "/admin" or "/admin/" => ApiResponseBuilder.HttpHtml(DashboardPage.GenerateDashboard()),
                "/admin/users" => await GenerateUsersPageAsync(),
                "/admin/devices" => await GenerateDevicesPageAsync(),
                "/admin/network" => ApiResponseBuilder.HttpHtml(NetworkControlPage.GenerateNetworkControl(
                    isNetworkActive: _networkConfiguration.IsNetworkActive,
                    upstreamInterface: _networkConfiguration.UpstreamInterface,
                    isVpnInterface: _networkConfiguration.IsVpnInterface,
                    gatewayIp: _networkConfiguration.GatewayIp,
                    defaultPort: _networkConfiguration.DefaultPort,
                    wifiInterface: _networkConfiguration.Interface,
                    ssid: _networkConfiguration.Ssid,
                    password: _networkConfiguration.Password,
                    dhcpRange: _networkConfiguration.DhcpRange
                )),
                "/admin/settings" => ApiResponseBuilder.HttpHtml("<h1>Settings Page - Coming Soon</h1>"),
                _ => ApiResponseBuilder.HttpHtml("<h1>404 - Admin Page Not Found</h1>")
            };
        }

        // 5. Portal pages - Handle login
        if (path == "/portal/login" && request.Method == "POST")
            return await HandleLoginAsync(request);

        if (path == "/portal/terms")
        {
            return ApiResponseBuilder.HttpHtml(TermsPage.GenerateTermsPage());
        }

        if (path.StartsWith("/portal") || path == "/")
        {
            string? portalMac = await GetMacAddressAsync(request.ClientIP);
            bool portalHasSession = !string.IsNullOrEmpty(portalMac) && 
                await _sessionManagementService.IsActiveSession(new SessionDto { MacAddress = portalMac, Username = "" });

            if (portalHasSession && path.StartsWith("/portal"))
                return ApiResponseBuilder.HttpHtml(SuccessPage.GenerateSuccessPageEnglish());
            
            return ApiResponseBuilder.HttpHtml(LoginPage.GenerateCleanLoginPage(request.ClientIP, portalMac ?? "Not detected"));
        }

        // 6. User request - check if has active session
        string? mac = await GetMacAddressAsync(request.ClientIP);
        bool hasSession = !string.IsNullOrEmpty(mac) && 
            await _sessionManagementService.IsActiveSession(new SessionDto { MacAddress = mac, Username = "" });

        if (hasSession)
        {
            // User authenticated - allow traffic
            return ApiResponseBuilder.Http204();
        }

        // No session - redirect to portal
        return ApiResponseBuilder.RedirectToPortal(GatewayIP, ServerPort);
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
                return ApiResponseBuilder.HttpHtml(LoginPage.GenerateLoginPageWithError(loginResult.Message, username, request.ClientIP, clientMac));
            }
            // Redirect to success page
            return ApiResponseBuilder.Redirect($"http://{GatewayIP}:{ServerPort}/portal/success");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Login error: {ex.Message}");
            return ApiResponseBuilder.HttpHtml(LoginPage.GenerateLoginPageWithError("An error occurred during login. Please try again.", "", request.ClientIP, ""));
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

    #region Admin Page Generators

    private async Task<string> GenerateUsersPageAsync()
    {
        try
        {
            var users = await _userManagementService.GetAllUsersAsync();
            var usersList = users.Select(u => (u.Name, u.Username, u.IsActive)).ToList();
            return ApiResponseBuilder.HttpHtml(UserManagementPage.GenerateUserManagement(usersList));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading users: {ex.Message}");
            return ApiResponseBuilder.HttpHtml(UserManagementPage.GenerateUserManagement());
        }
    }

    private async Task<string> GenerateDevicesPageAsync()
    {
        try
        {
            var devices = await _deviceManagement.GetConnectedDevicesAsync();
            var devicesList = devices.Select(d => (d.IpAddress, d.MacAddress, d.Username)).ToList();
            return ApiResponseBuilder.HttpHtml(DevicesPage.GenerateDevices(devicesList));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading devices: {ex.Message}");
            return ApiResponseBuilder.HttpHtml(DevicesPage.GenerateDevices());
        }
    }

    #endregion
}
