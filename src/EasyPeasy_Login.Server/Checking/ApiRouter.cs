using System.Diagnostics;
using System.Text.Json;
using EasyPeasy_Login.Application.DTOs;
using EasyPeasy_Login.Application.Services;
using EasyPeasy_Login.Application.Services.DeviceManagement;
using EasyPeasy_Login.Application.Services.SessionManagement;
using EasyPeasy_Login.Application.Services.UserManagement;
using EasyPeasy_Login.Infrastructure.Network.Configuration;
using EasyPeasy_Login.Shared;

namespace EasyPeasy_Login.Server.Checking;

/// <summary>
/// Router for handling all API requests (/api/*).
/// Centralizes REST API logic for users, devices, sessions, and network control.
/// </summary>
public class ApiRouter
{
    private readonly ISessionManagementService _sessionManagementService;
    private readonly IAuthenticationService _authenticationService;
    private readonly IUserManagementService _userManagementService;
    private readonly IDeviceManagement _deviceManagement;
    private readonly INetworkOrchestrator _networkOrchestrator;
    private readonly INetworkConfiguration _networkConfiguration;
    private readonly ILogger _logger;

    public ApiRouter(
        ISessionManagementService sessionManagementService,
        IAuthenticationService authenticationService,
        IUserManagementService userManagementService,
        IDeviceManagement deviceManagement,
        INetworkOrchestrator networkOrchestrator,
        INetworkConfiguration networkConfiguration,
        ILogger logger)
    {
        _sessionManagementService = sessionManagementService;
        _authenticationService = authenticationService;
        _userManagementService = userManagementService;
        _deviceManagement = deviceManagement;
        _networkOrchestrator = networkOrchestrator;
        _networkConfiguration = networkConfiguration;
        _logger = logger;
    }

    /// <summary>
    /// Route and handle API requests.
    /// </summary>
    public async Task<string> HandleAsync(HttpPetition request)
    {
        string path = request.Path.ToLower();
        string method = request.Method.ToUpper();

        try
        {
            // Users API
            if (path == "/api/users" && method == "GET")
            {
                var users = await _userManagementService.GetAllUsersAsync();
                return ApiResponseBuilder.HttpJson(users);
            }

            if (path == "/api/users" && method == "POST")
            {
                var body = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(request.Body);
                if (body == null) return ApiResponseBuilder.HttpError(400, "Invalid request body");

                var result = await _userManagementService.CreateUserAsync(
                    body["username"].GetString() ?? "",
                    body["name"].GetString() ?? "",
                    body["password"].GetString() ?? ""
                );

                return result.Success ? ApiResponseBuilder.HttpJson(result) : ApiResponseBuilder.HttpError(400, result.Message);
            }

            if (path.StartsWith("/api/users/") && method == "PUT")
            {
                string username = path.Replace("/api/users/", "");
                var body = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(request.Body);
                if (body == null) return ApiResponseBuilder.HttpError(400, "Invalid request body");

                var updateDto = new UpdateUserRequestDto
                {
                    Username = username,
                    Name = body.ContainsKey("name") ? body["name"].GetString() : null,
                    Password = body.ContainsKey("password") ? body["password"].GetString() : null,
                    IsActive = body.ContainsKey("isActive") ? body["isActive"].GetBoolean() : null
                };

                var result = await _userManagementService.UpdateUserAsync(updateDto);
                return result.Success ? ApiResponseBuilder.HttpJson(result) : ApiResponseBuilder.HttpError(400, result.Message);
            }

            if (path.StartsWith("/api/users/") && method == "DELETE")
            {
                string username = path.Replace("/api/users/", "");
                await _userManagementService.DeleteUserAsync(username);
                return ApiResponseBuilder.HttpJson(new { success = true, message = "User deleted successfully" });
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
                return ApiResponseBuilder.HttpJson(deviceList);
            }

            if (path.StartsWith("/api/device/") && method == "DELETE")
            {
                string macAddress = path.Replace("/api/device/", "");
                await _deviceManagement.DisconnectDeviceAsync(macAddress);
                return ApiResponseBuilder.HttpJson(new { success = true, message = "Device removed successfully" });
            }

            // Sessions API
            if (path == "/api/session" && method == "GET")
            {
                var sessions = await _sessionManagementService.GetAllSessionsAsync();
                return ApiResponseBuilder.HttpJson(sessions);
            }

            if (path.StartsWith("/api/session/") && method == "DELETE")
            {
                string macAddress = path.Replace("/api/session/", "");
                var session = await _sessionManagementService.GetSessionByMacAsync(macAddress);
                if (session != null)
                {
                    await _sessionManagementService.InvalidateSession(session);
                    return ApiResponseBuilder.HttpJson(new { success = true, message = "Session terminated successfully" });
                }
                return ApiResponseBuilder.HttpError(404, "Session not found");
            }

            // Network API
            if (path == "/api/network/start" && method == "POST")
            {
                _logger.LogInfo("Network start requested");
                var logs = new List<string> { "Network start requested" };
                if (!TryApplyNetworkConfigFromRequest(request.Body, logs, out var configError))
                {
                    return ApiResponseBuilder.HttpError(400, configError ?? "Invalid network configuration");
                }
                _logger.LogInfo(logs.Last());

                // Fire-and-forget to keep HTTP response fast
                _ = Task.Run(async () =>
                {
                    try
                    {
                        var success = await _networkOrchestrator.SetUpNetwork();
                        _logger.LogInfo(success ? "Network started successfully" : "Network failed to start");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Network start failed: {ex.Message}");
                    }
                });

                logs.AddRange(_logger.GetLogs().Select(l => $"[{l.Timestamp:HH:mm:ss}] {l.Level.ToString().ToUpper()} {l.Message}"));

                return ApiResponseBuilder.HttpJson(new
                {
                    success = true,
                    message = "Network start initiated",
                    logs
                });
            }

            if (path == "/api/network/logs" && method == "GET")
            {
                var logs = _logger.GetLogs()
                    .Select(l => $"[{l.Timestamp:HH:mm:ss}] {l.Level.ToString().ToUpper()} {l.Message}")
                    .ToList();
                return ApiResponseBuilder.HttpJson(new { success = true, logs });
            }

            if (path == "/api/network/stop" && method == "POST")
            {
                _logger.LogInfo("Network stop requested");
                var logs = new List<string> { "Network stop requested" };
                await _networkOrchestrator.RestoreConfiguration();
                logs.Add("Network services stopped and configuration restored");
                logs.AddRange(_logger.GetLogs().Select(l => $"[{l.Timestamp:HH:mm:ss}] {l.Level.ToString().ToUpper()} {l.Message}"));

                return ApiResponseBuilder.HttpJson(new
                {
                    success = true,
                    message = "Network stopped",
                    logs
                });
            }

            return ApiResponseBuilder.HttpError(404, "API endpoint not found");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"API Error: {ex.Message}");
            return ApiResponseBuilder.HttpError(500, ex.Message);
        }
    }

    private bool TryApplyNetworkConfigFromRequest(string body, List<string> logs, out string? error)
    {
        error = null;

        if (string.IsNullOrWhiteSpace(body))
        {
            logs.Add($"Using existing network configuration (interface {_networkConfiguration.Interface})");
            return true;
        }

        NetworkConfigPayload? payload;
        try
        {
            payload = JsonSerializer.Deserialize<NetworkConfigPayload>(body, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
        catch (Exception ex)
        {
            error = $"Invalid JSON: {ex.Message}";
            logs.Add(error);
            return false;
        }

        if (payload == null)
        {
            error = "Invalid network configuration payload.";
            logs.Add(error);
            return false;
        }

        if (string.IsNullOrWhiteSpace(payload.Interface))
        {
            error = "Interface is required to start the network.";
            logs.Add(error);
            return false;
        }

        _networkConfiguration.Interface = payload.Interface.Trim();

        if (!string.IsNullOrWhiteSpace(payload.Gateway))
            _networkConfiguration.GatewayIp = payload.Gateway.Trim();

        if (!string.IsNullOrWhiteSpace(payload.DhcpRange))
            _networkConfiguration.DhcpRange = payload.DhcpRange.Trim();

        if (!string.IsNullOrWhiteSpace(payload.Ssid))
            _networkConfiguration.Ssid = payload.Ssid.Trim();

        if (!string.IsNullOrWhiteSpace(payload.Password))
            _networkConfiguration.Password = payload.Password.Trim();

        if (payload.Port.HasValue)
        {
            if (payload.Port.Value <= 0 || payload.Port.Value > 65535)
            {
                error = "Port must be between 1 and 65535.";
                logs.Add(error);
                return false;
            }
            _networkConfiguration.DefaultPort = payload.Port.Value;
        }

        logs.Add($"Config applied: iface={_networkConfiguration.Interface}, gateway={_networkConfiguration.GatewayIp}, dhcp={_networkConfiguration.DhcpRange}, ssid={_networkConfiguration.Ssid}, port={_networkConfiguration.DefaultPort}");
        return true;
    }

    private class NetworkConfigPayload
    {
        public string? Interface { get; set; }
        public string? Gateway { get; set; }
        public string? DhcpRange { get; set; }
        public string? Ssid { get; set; }
        public string? Password { get; set; }
        public int? Port { get; set; }
    }
}
