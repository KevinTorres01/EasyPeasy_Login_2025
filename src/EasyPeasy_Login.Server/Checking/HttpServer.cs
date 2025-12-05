using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using EasyPeasy_Login.Application.DTOs;
using EasyPeasy_Login.Application.Services.SessionManagement;

namespace EasyPeasy_Login.Server.Checking;

/// <summary>
/// HTTP Server for Captive Portal - Simplified version.
/// Flow: Parse → Connectivity check? → Localhost? → Has session? → Respond
/// </summary>
public class HttpServer
{
    private readonly ISessionManagementService _sessionManagementService;
    private Socket? _listener;
    private volatile bool _isRunning;

    private const string GatewayIP = "192.168.100.1";
    private const int ServerPort = 8080;
    private const string PortalUrl = "/portal/login";

    public HttpServer(ISessionManagementService sessionManagementService)
    {
        _sessionManagementService = sessionManagementService;
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
    /// 2. Is from localhost? → Handle as admin
    /// 3. User request → Check session and respond accordingly
    /// </summary>
    private async Task<string> ProcessRequestAsync(HttpPetition request)
    {
        string path = request.Path.ToLower();

        // 1. Connectivity check (OS trying to detect captive portal)
        if (IsConnectivityCheck(request))
        {
            bool auth = await IsAuthenticatedAsync(request.ClientIP);
            return auth ? Http204() : RedirectToPortal();
        }

        // 2. Localhost = Admin
        if (request.IsFromLocalhost())
            return HandleAdmin(request);

        // 3. User request - check if has active session
        string? mac = await GetMacAddressAsync(request.ClientIP);
        bool hasSession = !string.IsNullOrEmpty(mac) && 
            await _sessionManagementService.IsActiveSession(new SessionDto { MacAddress = mac, Username = "" });

        if (hasSession)
        {
            // User authenticated - if asking for portal, show success; otherwise allow traffic
            if (path.StartsWith("/portal"))
                return HttpHtml(SuccessPage(mac!));
            return Http204(); // Allow traffic
        }

        // No session - redirect to portal
        return RedirectToPortal();
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

    private string HandleAdmin(HttpPetition request)
    {
        string path = request.Path.ToLower();

        if (path.StartsWith("/api/"))
            return HttpJson(new { status = "ok", path, method = request.Method });

        return HttpHtml($"<html><body><h1>Admin Panel</h1><p>Path: {path}</p></body></html>");
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

    private string RedirectToPortal()
    {
        string url = $"http://{GatewayIP}:{ServerPort}{PortalUrl}";
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
        return $"HTTP/1.1 200 OK\r\nContent-Type: application/json\r\nContent-Length: {len}\r\n\r\n{json}";
    }

    private string SuccessPage(string mac) => $@"<!DOCTYPE html>
<html><head><title>Connected</title></head>
<body><h1>✅ Connected!</h1><p>MAC: {mac}</p></body></html>";

    #endregion
}
