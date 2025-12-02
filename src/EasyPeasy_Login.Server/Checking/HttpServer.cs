using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using EasyPeasy_Login.Application.DTOs;
using EasyPeasy_Login.Application.Services.SessionManagement;

namespace EasyPeasy_Login.Server.Checking;

/// <summary>
/// HTTP Server for Captive Portal - Supports: Chrome, Safari, Firefox, Edge
/// Operating Systems: Android, iOS, macOS, Windows, Linux
/// </summary>
public class HttpServer
{
    private readonly ISessionManagementService _sessionManagementService;
    private Socket? _listener;
    private readonly ConcurrentDictionary<string, Channel<(Socket, string)>> _clientQueues = new();

    // Configuration
    private const string GatewayIP = "192.168.100.1";
    private const int ServerPort = 8080;
    private const string PortalLoginPage = "/portal/login";
    private const string AdminPage = "/admin";

    public HttpServer(ISessionManagementService sessionManagementService)
    {
        _sessionManagementService = sessionManagementService;
    }

    #region Server Lifecycle

    public void Start()
    {
        _listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        _listener.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        _listener.Bind(new IPEndPoint(IPAddress.Any, ServerPort));
        _listener.Listen(254);
        Console.WriteLine($"🚀 HttpServer listening on port {ServerPort}");

        Task.Run(AcceptConnectionsAsync);
    }

    public void Stop()
    {
        _listener?.Close();
        Console.WriteLine("🛑 HttpServer stopped");
    }

    private async Task AcceptConnectionsAsync()
    {
        while (_listener != null)
        {
            try
            {
                var client = await _listener.AcceptAsync();
                _ = HandleConnectionAsync(client);
            }
            catch (ObjectDisposedException) { break; }
        }
    }

    private async Task HandleConnectionAsync(Socket client)
    {
        try
        {
            var buffer = new byte[8192];
            int bytesRead = await client.ReceiveAsync(buffer, SocketFlags.None);
            if (bytesRead == 0) { client.Close(); return; }

            string rawRequest = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            string clientIP = ((IPEndPoint)client.RemoteEndPoint!).Address.ToString();
            
            await ProcessRequestAsync(client, rawRequest, clientIP);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Connection error: {ex.Message}");
        }
        finally
        {
            try { client.Shutdown(SocketShutdown.Both); } catch { }
            client.Close();
        }
    }

    #endregion

    #region Request Router

    private async Task ProcessRequestAsync(Socket client, string rawRequest, string clientIP)
    {
        var petition = HttpPetition.Parse(rawRequest, clientIP);
        string? response;

        // 1. Browser automatic requests
        response = HandleBrowserRequests(petition);
        if (response != null) { await SendAsync(client, response); return; }

        // 2. Captive portal connectivity checks
        response = await HandleCaptivePortalAsync(petition);
        if (response != null) { await SendAsync(client, response); return; }

        // 3. Admin or Portal requests
        response = petition.IsFromLocalhost()
            ? await HandleAdminAsync(petition)
            : await HandlePortalAsync(petition);

        await SendAsync(client, response);
    }

    private async Task SendAsync(Socket client, string response)
    {
        await client.SendAsync(Encoding.UTF8.GetBytes(response), SocketFlags.None);
    }

    #endregion

    #region Browser Automatic Requests

    private string? HandleBrowserRequests(HttpPetition petition)
    {
        string path = petition.Path.ToLower();

        // Favicon, icons
        if (path.Contains("favicon") || path.Contains("apple-touch-icon"))
            return Http204();

        // Config files
        if (path is "/robots.txt")
            return HttpText("User-agent: *\nDisallow: /api/\nDisallow: /admin/");

        if (path is "/manifest.json" or "/site.webmanifest" or "/sw.js" or "/service-worker.js")
            return Http204();

        // Security probes (block)
        if (path.EndsWith(".map") || path.StartsWith("/.well-known/") || 
            path.Contains("wp-") || path.Contains("php") || path is "/.env" or "/.git/HEAD")
            return Http404();

        // CORS preflight
        if (petition.Method == "OPTIONS")
            return HttpCors();

        return null;
    }

    #endregion

    #region Captive Portal Detection (Android, iOS, macOS, Windows, Linux + Chrome, Safari, Firefox, Edge)

    private enum ConnectivityCheck { Apple, Google, Microsoft, Firefox, Linux }

    private async Task<string?> HandleCaptivePortalAsync(HttpPetition petition)
    {
        string path = petition.Path.ToLower();
        string host = petition.Host.ToLower();

        var check = DetectConnectivityCheck(path, host);
        if (check == null) return null;

        Console.WriteLine($"📡 {check} connectivity check from {petition.ClientIP}");

        bool authenticated = await IsAuthenticatedAsync(petition.ClientIP);
        return authenticated ? SuccessResponse(check.Value, path) : RedirectToPortal();
    }

    private ConnectivityCheck? DetectConnectivityCheck(string path, string host)
    {
        // Apple: iOS, macOS, Safari
        if (host.Contains("apple.com") || path is "/hotspot-detect.html" or "/library/test/success.html")
            return ConnectivityCheck.Apple;

        // Google: Android, Chrome
        if (host.Contains("gstatic.com") || host.Contains("google.com") || 
            host.Contains("android.com") || path.Contains("generate_204") || path.Contains("gen_204"))
            return ConnectivityCheck.Google;

        // Microsoft: Windows, Edge
        if (host.Contains("msft") || path is "/ncsi.txt" or "/connecttest.txt" or "/redirect")
            return ConnectivityCheck.Microsoft;

        // Firefox
        if (host.Contains("firefox.com") || path is "/success.txt" or "/canonical.html")
            return ConnectivityCheck.Firefox;

        // Linux: Ubuntu, GNOME, Debian
        if (host.Contains("ubuntu.com") || host.Contains("gnome.org") || host.Contains("debian.org"))
            return ConnectivityCheck.Linux;

        return null;
    }

    private string SuccessResponse(ConnectivityCheck check, string path) => check switch
    {
        ConnectivityCheck.Apple => HttpHtml("<HTML><HEAD><TITLE>Success</TITLE></HEAD><BODY>Success</BODY></HTML>"),
        ConnectivityCheck.Microsoft when path == "/ncsi.txt" => HttpText("Microsoft NCSI"),
        ConnectivityCheck.Microsoft when path == "/connecttest.txt" => HttpText("Microsoft Connect Test"),
        ConnectivityCheck.Microsoft => HttpHtml("Microsoft NCSI"),
        ConnectivityCheck.Firefox when path == "/success.txt" => HttpText("success\n"),
        ConnectivityCheck.Firefox => HttpHtml("<html><body>success</body></html>"),
        ConnectivityCheck.Linux => HttpHtml("NetworkManager is online"),
        _ => Http204() // Google/Android/Chrome
    };

    private string RedirectToPortal()
    {
        string url = $"http://{GatewayIP}:{ServerPort}{PortalLoginPage}";
        string html = $"<!DOCTYPE html><html><head><meta http-equiv='refresh' content='0;url={url}'></head><body><a href='{url}'>Login</a></body></html>";
        return $"HTTP/1.1 302 Found\r\nLocation: {url}\r\nCache-Control: no-cache\r\nContent-Type: text/html\r\nContent-Length: {Encoding.UTF8.GetByteCount(html)}\r\n\r\n{html}";
    }

    private async Task<bool> IsAuthenticatedAsync(string clientIP)
    {
        string? mac = await GetMacAddressAsync(clientIP);
        if (string.IsNullOrEmpty(mac)) return false;
        return await _sessionManagementService.IsActiveSession(new SessionDto { MacAddress = mac, Username = "" });
    }

    #endregion

    #region Admin Panel (localhost only)

    private async Task<string> HandleAdminAsync(HttpPetition petition)
    {
        string path = petition.Path.ToLower();
        Console.WriteLine($"🔐 Admin: {petition.Method} {path}");

        // API routes
        if (path.StartsWith("/api/"))
            return await HandleAdminApiAsync(petition);

        // Admin pages
        if (path.StartsWith("/admin"))
            return HttpHtml(AdminPageHtml(path));

        return HttpRedirect(AdminPage);
    }

    private async Task<string> HandleAdminApiAsync(HttpPetition petition)
    {
        string path = petition.Path.ToLower();
        string method = petition.Method;

        // Users API
        if (path.StartsWith("/api/users"))
            return HttpJson(new { endpoint = "users", method, message = "TODO: Implement UserManagementService" });

        // Sessions API
        if (path.StartsWith("/api/sessions"))
            return HttpJson(new { endpoint = "sessions", method, message = "TODO: Implement SessionManagementService" });

        // Devices API
        if (path.StartsWith("/api/devices"))
            return HttpJson(new { endpoint = "devices", method, message = "TODO: Implement DeviceManagementService" });

        // Stats
        if (path.StartsWith("/api/stats"))
            return HttpJson(new { users = 0, sessions = 0, devices = 0 });

        return Http404();
    }

    private string AdminPageHtml(string path) => $@"<!DOCTYPE html>
<html><head><title>Admin - EasyPeasy</title></head>
<body>
<h1>Admin Panel</h1>
<nav><a href='/admin'>Dashboard</a> | <a href='/admin/users'>Users</a> | <a href='/admin/sessions'>Sessions</a> | <a href='/admin/devices'>Devices</a></nav>
<p>Current: {path}</p>
</body></html>";

    #endregion

    #region Portal (remote clients)

    private async Task<string> HandlePortalAsync(HttpPetition petition)
    {
        string? mac = await GetMacAddressAsync(petition.ClientIP);
        if (string.IsNullOrEmpty(mac)) return HttpRedirect(PortalLoginPage);

        bool authenticated = await _sessionManagementService.IsActiveSession(new SessionDto { MacAddress = mac, Username = "" });
        string path = petition.Path.ToLower();

        Console.WriteLine($"🌐 Portal: {petition.Method} {path} | MAC: {mac} | Auth: {authenticated}");

        // Static files (CSS, JS, images)
        if (IsStaticFile(path)) return Http404(); // TODO: serve actual files

        // Authenticated users
        if (authenticated)
        {
            if (path == "/portal/logout") return HttpRedirect(PortalLoginPage); // TODO: terminate session
            if (path.StartsWith("/portal")) return HttpHtml(SuccessPageHtml(mac));
            return HttpHtml("<html><body><h1>Connected</h1></body></html>");
        }

        // Unauthenticated users
        if (path == "/portal/login" && petition.Method == "POST")
            return HttpJson(new { success = true, message = "TODO: Implement login" });

        if (path.StartsWith("/portal"))
            return HttpHtml(LoginPageHtml(mac));

        return HttpRedirect(PortalLoginPage);
    }

    private bool IsStaticFile(string path) =>
        path.EndsWith(".css") || path.EndsWith(".js") || path.EndsWith(".png") || 
        path.EndsWith(".jpg") || path.EndsWith(".svg") || path.EndsWith(".woff2");

    private string LoginPageHtml(string mac) => $@"<!DOCTYPE html>
<html><head><title>WiFi Login</title><meta name='viewport' content='width=device-width,initial-scale=1'>
<style>body{{font-family:Arial;max-width:400px;margin:50px auto;padding:20px}}input,button{{width:100%;padding:10px;margin:5px 0;box-sizing:border-box}}button{{background:#007bff;color:white;border:none;cursor:pointer}}</style></head>
<body><h1>WiFi Login</h1>
<form method='POST'><input type='hidden' name='mac' value='{mac}'><input name='username' placeholder='Username' required><input type='password' name='password' placeholder='Password' required><button>Connect</button></form>
</body></html>";

    private string SuccessPageHtml(string mac) => $@"<!DOCTYPE html>
<html><head><title>Connected</title></head>
<body><h1>✅ Connected!</h1><p>Device: {mac}</p><a href='/portal/logout'>Disconnect</a></body></html>";

    #endregion

    #region Utilities

    private async Task<string?> GetMacAddressAsync(string ip)
    {
        try
        {
            await RunCommandAsync($"ping -c 1 -W 1 {ip}");
            string arp = await RunCommandAsync($"ip neigh show {ip}");
            var match = Regex.Match(arp, @"([0-9A-Fa-f]{2}[:-]){5}[0-9A-Fa-f]{2}");
            if (match.Success) return match.Value.ToLower();

            // Fallback: DHCP leases
            if (File.Exists("/var/lib/misc/dnsmasq.leases"))
            {
                foreach (var line in await File.ReadAllLinesAsync("/var/lib/misc/dnsmasq.leases"))
                {
                    var parts = line.Split(' ');
                    if (parts.Length >= 3 && parts[2] == ip) return parts[1].ToLower();
                }
            }
        }
        catch { }
        return null;
    }

    private async Task<string> RunCommandAsync(string cmd)
    {
        var p = new Process { StartInfo = new() { FileName = "/bin/bash", Arguments = $"-c \"{cmd}\"", RedirectStandardOutput = true, UseShellExecute = false, CreateNoWindow = true } };
        p.Start();
        string output = await p.StandardOutput.ReadToEndAsync();
        await p.WaitForExitAsync();
        return output;
    }

    #endregion

    #region HTTP Response Builders

    private string Http204() => "HTTP/1.1 204 No Content\r\nCache-Control: no-cache\r\nContent-Length: 0\r\n\r\n";
    private string Http404() => "HTTP/1.1 404 Not Found\r\nContent-Length: 0\r\n\r\n";
    private string HttpCors() => "HTTP/1.1 204 No Content\r\nAccess-Control-Allow-Origin: *\r\nAccess-Control-Allow-Methods: GET,POST,PUT,DELETE,OPTIONS\r\nAccess-Control-Allow-Headers: Content-Type,Authorization\r\nContent-Length: 0\r\n\r\n";
    
    private string HttpRedirect(string location)
    {
        string url = location.StartsWith("http") ? location : $"http://{GatewayIP}:{ServerPort}{location}";
        return $"HTTP/1.1 302 Found\r\nLocation: {url}\r\nContent-Length: 0\r\n\r\n";
    }

    private string HttpText(string text)
    {
        byte[] body = Encoding.UTF8.GetBytes(text);
        return $"HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\nCache-Control: no-cache\r\nContent-Length: {body.Length}\r\n\r\n{text}";
    }

    private string HttpHtml(string html)
    {
        byte[] body = Encoding.UTF8.GetBytes(html);
        return $"HTTP/1.1 200 OK\r\nContent-Type: text/html; charset=utf-8\r\nContent-Length: {body.Length}\r\n\r\n{html}";
    }

    private string HttpJson(object data)
    {
        string json = JsonSerializer.Serialize(data, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        byte[] body = Encoding.UTF8.GetBytes(json);
        return $"HTTP/1.1 200 OK\r\nContent-Type: application/json\r\nCache-Control: no-cache\r\nContent-Length: {body.Length}\r\n\r\n{json}";
    }

    #endregion
}
