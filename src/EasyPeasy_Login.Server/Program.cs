using Microsoft.Extensions.DependencyInjection;
using EasyPeasy_Login.Application.Services;
using EasyPeasy_Login.Application.Services.DeviceManagement;
using EasyPeasy_Login.Application.Services.NetworkControl;
using EasyPeasy_Login.Application.Services.SessionManagement;
using EasyPeasy_Login.Application.Services.UserManagement;
using EasyPeasy_Login.Domain.Helper;
using EasyPeasy_Login.Domain.Interfaces;
using EasyPeasy_Login.Infrastructure.Data.Repositories;
using EasyPeasy_Login.Infrastructure.Network;
using EasyPeasy_Login.Infrastructure.Network.Configuration;
using EasyPeasy_Login.Server.Checking;
using EasyPeasy_Login.Shared;

Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
Console.WriteLine("║          EasyPeasy Login - Captive Portal Server           ║");
Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
Console.WriteLine();

// Configure services
var services = new ServiceCollection();

// Logger
services.AddSingleton<EasyPeasy_Login.Shared.ILogger, Logger>();

// Infrastructure - Network Configuration
services.AddSingleton<INetworkConfiguration, NetworkConfiguration>();
services.AddSingleton<ICommandExecutor, CommandExecutor>();
services.AddSingleton<IDnsmasqManager, DnsmasqManager>();
services.AddSingleton<IHostapdManager, HostapdManager>();
services.AddSingleton<ICaptivePortalControlManager, CaptivePortalControlManager>();
services.AddSingleton<INetworkManager, NetworkManager>();
services.AddSingleton<INetworkOrchestrator, NetworkOrchestrator>();
services.AddSingleton<IFirewallService, FirewallService>();

// Utilities
services.AddSingleton<IPasswordHasher, PasswordHasher>();
services.AddSingleton<IMacAddressResolver, MacAddressResolver>();

// Repositories
services.AddSingleton<ISessionRepository, SessionRepository>();
services.AddSingleton<IDeviceRepository, DeviceRepository>();
services.AddSingleton<IUserRepository, UserRepository>();

// Application Services
services.AddSingleton<INetworkControlService, NetworkControlService>();
services.AddSingleton<IDeviceManagement, DeviceManagement>();
services.AddSingleton<ISessionManagementService, SessionManagementService>();
services.AddSingleton<IAuthenticationService, AuthenticationService>();
services.AddSingleton<IUserManagementService, UserManagementService>();

// HTTP Server
services.AddSingleton<HttpServer>();

var serviceProvider = services.BuildServiceProvider();

// Get and start the server
var httpServer = serviceProvider.GetRequiredService<HttpServer>();

Console.WriteLine("[INFO] Starting HTTP Server on port 8080...");
Console.WriteLine("[INFO] Gateway IP: 192.168.100.1");
Console.WriteLine();
Console.WriteLine("[INFO] Portal URL:  http://192.168.100.1:8080/portal/login");
Console.WriteLine("[INFO] Admin URL:   http://localhost:8080/admin");
Console.WriteLine();
Console.WriteLine("Press Ctrl+C to stop the server.");
Console.WriteLine();

// Handle graceful shutdown
var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true;
    Console.WriteLine("\n[INFO] Shutting down...");
    httpServer.Stop();
    cts.Cancel();
};

try
{
    httpServer.Start();
    
    // Keep running until cancelled
    await Task.Delay(Timeout.Infinite, cts.Token);
}
catch (OperationCanceledException)
{
    // Expected on shutdown
}
catch (Exception ex)
{
    Console.WriteLine($"[ERROR] Server failed: {ex.Message}");
}

Console.WriteLine("[INFO] Server stopped.");
