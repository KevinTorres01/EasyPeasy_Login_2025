using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using EasyPeasy_Login.Application.Services.SessionManagement;
using EasyPeasy_Login.Application.DTOs;

namespace EasyPeasy_Login.Server.Checking;

/// <summary>
/// HttpServer - Core: Main server logic, connection handling, and request routing
/// </summary>
public partial class HttpServer
{
    private readonly ISessionManagementService _sessionManagementService;
    private Socket? _listener;

    // Dictionary of queues by client IP
    private readonly ConcurrentDictionary<string, Channel<(Socket client, string rawRequest)>> _clientQueues = new();

    // Server configuration
    private const string GatewayIP = "192.168.100.1";
    private const int ServerPort = 8080;
    private const string PortalLoginPage = "/portal/login";
    private const string AdminPage = "/admin";

    public HttpServer(ISessionManagementService sessionManagementService)
    {
        _sessionManagementService = sessionManagementService;
    }

    #region Server Startup

    public void Start()
    {
        _listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        _listener.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        _listener.Bind(new IPEndPoint(IPAddress.Any, ServerPort));
        _listener.Listen(100);
        Console.WriteLine($"🚀 Server listening on port {ServerPort}...");

        Task.Run(async () =>
        {
            while (true)
            {
                var client = await _listener.AcceptAsync();
                _ = EnqueueClientAsync(client);
            }
        });
    }

    public void Stop()
    {
        _listener?.Close();
        _listener?.Dispose();
        Console.WriteLine("🛑 Server stopped");
    }

    #endregion

    #region Connection Queue Management

    private async Task EnqueueClientAsync(Socket client)
    {
        try
        {
            var buffer = new byte[8192]; // Increased buffer for larger POST requests
            int bytesRead = await client.ReceiveAsync(buffer, SocketFlags.None);
            if (bytesRead == 0) { client.Close(); return; }

            string rawRequest = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            string clientIP = ((IPEndPoint)client.RemoteEndPoint!).Address.ToString();

            var queue = _clientQueues.GetOrAdd(clientIP, ip =>
            {
                var ch = Channel.CreateUnbounded<(Socket, string)>();
                Task.Run(() => ProcessQueueAsync(ip, ch));
                return ch;
            });

            await queue.Writer.WriteAsync((client, rawRequest));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error queuing client: {ex.Message}");
            client.Close();
        }
    }

    private async Task ProcessQueueAsync(string clientIP, Channel<(Socket client, string rawRequest)> queue)
    {
        Console.WriteLine($"🧵 Thread started for IP: {clientIP}");

        try
        {
            while (await queue.Reader.WaitToReadAsync())
            {
                while (queue.Reader.TryRead(out var item))
                {
                    await HandleClientAsync(item.client, item.rawRequest, clientIP);
                }

                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
                try
                {
                    if (!await queue.Reader.WaitToReadAsync(cts.Token))
                        break;
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }
        finally
        {
            _clientQueues.TryRemove(clientIP, out _);
            Console.WriteLine($"🧹 Thread ended for IP: {clientIP}");
        }
    }

    #endregion

    #region Main Request Router

    private async Task HandleClientAsync(Socket client, string rawRequest, string clientIP)
    {
        try
        {
            var petition = HttpPetition.Parse(rawRequest, clientIP);

            // 1. Browser common requests (favicon, robots, etc.)
            string? browserResponse = HandleBrowserCommonRequests(petition);
            if (browserResponse != null)
            {
                await SendResponseAsync(client, browserResponse);
                return;
            }

            // 2. Captive portal detection requests (OS connectivity checks)
            string? captiveResponse = await HandleCaptivePortalDetectionAsync(petition);
            if (captiveResponse != null)
            {
                await SendResponseAsync(client, captiveResponse);
                return;
            }

            // 3. Route based on origin
            string response;
            if (petition.IsFromLocalhost())
            {
                // Admin requests from localhost
                response = await HandleAdminRequestAsync(petition);
            }
            else
            {
                // Remote client requests (captive portal users)
                response = await HandleRemoteRequestAsync(petition);
            }

            await SendResponseAsync(client, response);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error handling client: {ex.Message}");
            await SendResponseAsync(client, Build500ErrorResponse(ex.Message));
        }
        finally
        {
            try { client.Shutdown(SocketShutdown.Both); } catch { }
            client.Close();
        }
    }

    private async Task SendResponseAsync(Socket client, string response)
    {
        byte[] responseBytes = Encoding.UTF8.GetBytes(response);
        await client.SendAsync(responseBytes, SocketFlags.None);
    }

    #endregion
}
