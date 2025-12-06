using EasyPeasy_Login.Application.Services;
using EasyPeasy_Login.Application.Services.DeviceManagement;
using EasyPeasy_Login.Application.Services.NetworkControl;
using EasyPeasy_Login.Application.Services.UserManagement;
using EasyPeasy_Login.Infrastructure.Data.Repositories;
using EasyPeasy_Login.Infrastructure.Network.Configuration;
using EasyPeasy_Login.Application.Services.SessionManagement;
using EasyPeasy_Login.Infrastructure.Network;
using EasyPeasy_Login.Domain.Helper;
using EasyPeasy_Login.Domain.Interfaces;
using EasyPeasy_Login.Shared;
using EasyPeasy_Login.Web.HostedServices;

var builder = WebApplication.CreateBuilder(args);

// Add CORS policy to allow requests from captive portal (port 8080)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowCaptivePortal", policy =>
    {
        policy.WithOrigins("http://localhost:8080", "http://192.168.100.1:8080")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add controllers only (no Blazor/Razor)
builder.Services.AddControllers();

// Register logger as singleton
builder.Services.AddSingleton<EasyPeasy_Login.Shared.ILogger, Logger>();

// Network configuration
builder.Services.AddSingleton<INetworkConfiguration, NetworkConfiguration>();

// Register infrastructure services as Singleton
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

// Application services - Singleton porque se usan en el HttpServer (servicio de fondo)
builder.Services.AddSingleton<IDeviceManagement, DeviceManagement>();
builder.Services.AddSingleton<ISessionManagementService, SessionManagementService>();
builder.Services.AddSingleton<IAuthenticationService, AuthenticationService>();
builder.Services.AddSingleton<IUserManagementService, UserManagementService>();

// HttpServer for captive portal (port 8080)
builder.Services.AddHostedService<CaptivePortalHostedService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

// Enable CORS before other middleware
app.UseCors("AllowCaptivePortal");

app.UseStaticFiles();

// Map controllers for API endpoints
app.MapControllers();

Console.WriteLine("🌐 ASP.NET API Server Configuration:");
Console.WriteLine($"   - API Endpoints: http://localhost:5000/api/*");
Console.WriteLine($"   - API Endpoints: http://192.168.100.1:5000/api/*");
Console.WriteLine("");

// API server runs on port 5000
app.Run("http://0.0.0.0:5000");