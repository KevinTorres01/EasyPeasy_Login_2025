using EasyPeasy_Login.Application.Services;
using EasyPeasy_Login.Application.Services.DeviceManagement;
using EasyPeasy_Login.Application.Services.NetworkControl;
using EasyPeasy_Login.Application.Services.UserManagement;
using EasyPeasy_Login.Infrastructure.Data.Repositories;
using EasyPeasy_Login.Infrastructure.Network.Configuration;
using EasyPeasy_Login.Application.Services.SessionManagement;
using EasyPeasy_Login.Web.Middleware;
using EasyPeasy_Login.Infrastructure.Network;
using EasyPeasy_Login.Domain.Helper;
using EasyPeasy_Login.Domain.Interfaces;
using EasyPeasy_Login.Web.Components;
using EasyPeasy_Login.Shared;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddControllers();

// Register network configuration (singleton to share state across the portal session)
builder.Services.AddSingleton<INetworkConfiguration, NetworkConfiguration>();

// Register logger as singleton to share logs across all components
builder.Services.AddSingleton<EasyPeasy_Login.Shared.ILogger, Logger>();

// Register infrastructure services as Singleton - they don't have per-request state
builder.Services.AddSingleton<ICommandExecutor, CommandExecutor>();
builder.Services.AddSingleton<IDnsmasqManager, DnsmasqManager>();
builder.Services.AddSingleton<IHostapdManager, HostapdManager>();
builder.Services.AddSingleton<ICaptivePortalControlManager, CaptivePortalControlManager>();
builder.Services.AddSingleton<INetworkManager, NetworkManager>();

// Network orchestration and control - Singleton because they manage global system state (iptables, network config)
builder.Services.AddSingleton<INetworkOrchestrator, NetworkOrchestrator>();
builder.Services.AddSingleton<IFirewallService, FirewallService>();
builder.Services.AddSingleton<INetworkControlService, NetworkControlService>();

// Utilities
builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IMacAddressResolver, MacAddressResolver>();

builder.Services.AddHttpContextAccessor();

// Repositories as Singleton - they manage in-memory data that should persist during app lifetime
builder.Services.AddSingleton<ISessionRepository, SessionRepository>();
builder.Services.AddSingleton<IDeviceRepository, DeviceRepository>();
builder.Services.AddSingleton<IUserRepository, UserRepository>();

// Application services - Scoped because they handle business logic per request
builder.Services.AddScoped<IDeviceManagement, DeviceManagement>();
builder.Services.AddScoped<ISessionManagementService, SessionManagementService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IUserManagementService, UserManagementService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

// Middleware before UseStaticFiles
app.UseMiddleware<RequestRouterMiddleware>();

app.UseStaticFiles();
app.UseAntiforgery();

// Endpoints
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapControllers();

app.Run("http://0.0.0.0:8080");