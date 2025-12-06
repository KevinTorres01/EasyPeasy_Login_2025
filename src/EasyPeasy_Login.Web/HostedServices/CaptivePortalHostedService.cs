using EasyPeasy_Login.Server.Checking;
using EasyPeasy_Login.Application.Services;
using EasyPeasy_Login.Application.Services.SessionManagement;
using EasyPeasy_Login.Application.Services.UserManagement;
using EasyPeasy_Login.Application.Services.DeviceManagement;

namespace EasyPeasy_Login.Web.HostedServices;

/// <summary>
/// Background service that runs the HttpServer for captive portal functionality.
/// </summary>
public class CaptivePortalHostedService : IHostedService
{
    private readonly HttpServer _httpServer;
    private readonly ILogger<CaptivePortalHostedService> _logger;

    public CaptivePortalHostedService(
        ISessionManagementService sessionManagementService,
        IAuthenticationService authenticationService,
        IUserManagementService userManagementService,
        IDeviceManagement deviceManagement,
        ILogger<CaptivePortalHostedService> logger)
    {
        _httpServer = new HttpServer(
            sessionManagementService, 
            authenticationService,
            userManagementService,
            deviceManagement);
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting Captive Portal HTTP Server...");
        _httpServer.Start();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping Captive Portal HTTP Server...");
        _httpServer.Stop();
        return Task.CompletedTask;
    }
}
