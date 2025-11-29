using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using EasyPeasy_Login.Application.Services.SessionManagement;
using EasyPeasy_Login.Application.DTOs;
using EasyPeasy_Login.Server.Checking;

public class HttpServer
{
    private readonly ISessionManagementService _sessionManagementService;
    private TcpListener? _listener;

    // Dictionary of queues by client IP
    private readonly ConcurrentDictionary<string, Channel<(TcpClient client, string rawRequest)>> _clientQueues = new();

    // Redirect URLs
    private const string PortalLoginPage = "/portal/login";
    private const string AdminPage = "/admin";

    public HttpServer(ISessionManagementService sessionManagementService)
    {
        _sessionManagementService = sessionManagementService;
    }

    public void Start()
    {
        _listener = new TcpListener(IPAddress.Any, 8080);
        _listener.Start();
        Console.WriteLine("🚀 Servidor escuchando en puerto 8080...");

        Task.Run(async () =>
        {
            while (true)
            {
                var client = await _listener.AcceptTcpClientAsync();
                _ = EnqueueClientAsync(client);
            }
        });
    }

    private async Task EnqueueClientAsync(TcpClient client)
    {
        try
        {
            var stream = client.GetStream();
            var buffer = new byte[4096];
            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
            if (bytesRead == 0) { client.Close(); return; }

            string rawRequest = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            string clientIP = ((IPEndPoint)client.Client.RemoteEndPoint!).Address.ToString();

            var queue = _clientQueues.GetOrAdd(clientIP, _ =>
            {
                var ch = Channel.CreateUnbounded<(TcpClient, string)>();
                var process = ProcessQueueAsync(clientIP, ch); // Starts dedicated thread for this client
                return ch;
            });

            await queue.Writer.WriteAsync((client, rawRequest));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error encolando cliente: {ex.Message}");
            client.Close();
        }
    }

    private async Task ProcessQueueAsync(string clientIP, Channel<(TcpClient client, string rawRequest)> queue)
    {
        Console.WriteLine($"🧵 Hilo iniciado para IP: {clientIP}");

        try
        {
            while (await queue.Reader.WaitToReadAsync())
            {
                while (queue.Reader.TryRead(out var item))
                {
                    await HandleClientAsync(item.client, item.rawRequest, clientIP);
                }

                // If there is no activity in 30 sec, end and clear
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
                try
                {
                    if (!await queue.Reader.WaitToReadAsync(cts.Token))
                        break;
                }
                catch (OperationCanceledException)
                {
                    break; // Timeout - no more requests, exit
                }
            }
        }
        finally
        {
            _clientQueues.TryRemove(clientIP, out _);
            Console.WriteLine($"🧹 Hilo terminado para IP: {clientIP}");
        }
    }

    private async Task HandleClientAsync(TcpClient client, string rawRequest, string clientIP)
    {
        try
        {
            var petition = HttpPetition.Parse(rawRequest, clientIP);

            string response = petition.IsFromLocalhost()
                ? HandleLocalRequest(petition)
                : await HandleRemoteRequestAsync(petition);

            byte[] responseBytes = Encoding.UTF8.GetBytes(response);
            await client.GetStream().WriteAsync(responseBytes, 0, responseBytes.Length);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error manejando cliente: {ex.Message}");
        }
        finally
        {
            client.Close();
        }
    }

    /// <summary>
    /// Handles local requests (from the same server) - serves the admin page
    /// </summary>
    private string HandleLocalRequest(HttpPetition petition)
    {
        Console.WriteLine($"🏠 LOCAL request: {petition.Method} {petition.Path}");

        // If the request is under /admin, serve the admin page
        if (petition.Path.StartsWith("/admin"))
        {
            // TODO: Replace with your real admin HTML page
            string adminHtml = "<html><body>Here goes the HTML</body></html>";
            return BuildHtmlResponse(200, "OK", adminHtml);
        }

        // Redirect to /admin if not there
        return BuildRedirectResponse(AdminPage);
    }

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
            Console.WriteLine($"⚠️ Can't obtain Mac for IP: {petition.ClientIP}");
            // If we can not obtain MAC, redirect to login portal
            return BuildRedirectResponse(PortalLoginPage);
        }

        Console.WriteLine($"🔍 MAC detected: {macAddress} for IP: {petition.ClientIP}");

        // 2. Check if there is an active session for this MAC
        var sessionDto = new SessionDto
        {
            MacAddress = macAddress,
            Username = string.Empty // No need to provide username for this check
        };

        bool isAuthenticated = await _sessionManagementService.IsActiveSession(sessionDto);

        if (isAuthenticated)
        {
            // 3a. AUTHENTICATED: allow internet access
            Console.WriteLine($"✅ Device {macAddress} AUTHENTICATED - allowing traffic");

            // If requesting the portal and already authenticated, show success page
            if (petition.Path.StartsWith("/portal"))
            {
                return BuildSuccessPage(macAddress);
            }

            // For other routes, allow access (in a real captive portal you'd proxy or pass-through)
            return BuildAllowedResponse();
        }
        else
        {
            // 3b. NOT AUTHENTICATED: redirect to login portal
            Console.WriteLine($"🔒 Device {macAddress} NOT AUTHENTICATED - redirecting to login");

            // If already on the portal page, serve the login page
            if (petition.Path.StartsWith("/portal"))
            {
                return BuildLoginPage(macAddress, petition.ClientIP);
            }

            // Redirect to the login portal
            return BuildRedirectResponse(PortalLoginPage);
        }
    }

    /// <summary>
    /// Gets the MAC address from the system ARP table
    /// </summary>
    private async Task<string?> GetMacAddressFromIpAsync(string ipAddress)
    {
        try
        {
            // Ping first to ensure the entry exists in ARP
            await ExecuteCommandAsync($"ping -c 1 -W 1 {ipAddress}", ignoreErrors: true);

            // Query ARP table
            string arpOutput = await ExecuteCommandAsync($"ip neigh show {ipAddress}");

            // Search for MAC pattern: aa:bb:cc:dd:ee:ff
            var macMatch = Regex.Match(arpOutput, @"([0-9A-Fa-f]{2}[:-]){5}([0-9A-Fa-f]{2})");

            if (macMatch.Success)
            {
                return macMatch.Value.ToLower();
            }

            // Alternative method: read DHCP leases
            string leasesPath = "/var/lib/misc/dnsmasq.leases";
            if (File.Exists(leasesPath))
            {
                var leases = await File.ReadAllLinesAsync(leasesPath);
                foreach (var lease in leases)
                {
                    var parts = lease.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length >= 3 && parts[2] == ipAddress)
                    {
                        return parts[1].ToLower();
                    }
                }
            }

            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error getting MAC: {ex.Message}");
            return null;
        }
    }

    private async Task<string> ExecuteCommandAsync(string command, bool ignoreErrors = false)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "/bin/bash",
                Arguments = $"-c \"{command}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        process.Start();
        string output = await process.StandardOutput.ReadToEndAsync();
        await process.WaitForExitAsync();

        return output;
    }

    #region HTTP Response Builders

    private string BuildRedirectResponse(string location)
    {
        string fullUrl = $"http://192.168.100.1:8080{location}";
        return $"HTTP/1.1 302 Found\r\nLocation: {fullUrl}\r\nContent-Length: 0\r\n\r\n";
    }

    private string BuildHtmlResponse(int statusCode, string statusText, string html)
    {
        byte[] bodyBytes = Encoding.UTF8.GetBytes(html);
        return $"HTTP/1.1 {statusCode} {statusText}\r\n" +
               "Content-Type: text/html; charset=utf-8\r\n" +
               $"Content-Length: {bodyBytes.Length}\r\n\r\n{html}";
    }

    private string BuildAllowedResponse()
    {
        string html = "<html><body>Here goes the HTML</body></html>";
        return BuildHtmlResponse(200, "OK", html);
    }

    private string BuildSuccessPage(string macAddress)
    {
        string html = "<html><body>Here goes the HTML</body></html>";
        return BuildHtmlResponse(200, "OK", html);
    }

    private string BuildLoginPage(string macAddress, string clientIp)
    {
        // TODO: Replace with your real login HTML page
        string html = "<html><body>Here goes the HTML</body></html>";
        return BuildHtmlResponse(200, "OK", html);
    }

    #endregion
}
