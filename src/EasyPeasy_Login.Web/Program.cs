using EasyPeasy_Login.Web.Components;
using EasyPeasy_Login.Application.Services;
using EasyPeasy_Login.Infrastructure.Data.Repositories;
using EasyPeasy_Login.Infrastructure.Network.Configuration;
using EasyPeasy_Login.Application.Services.SessionManagement;
using EasyPeasy_Login.Web.Middleware;
using EasyPeasy_Login.Infrastructure.Network;
using EasyPeasy_Login.Domain.Helper;
using EasyPeasy_Login.Domain.Interfaces;
using EasyPeasy_Login.Infrastructure.Network.Configuration.CommandExecutor;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddControllers();

// Register presentation services
builder.Services.AddScoped<ILogger, Logger>();
builder.Services.AddScoped<ICommandExecutor, CommandExecutor>();
builder.Services.AddScoped<IDnsmasqManager, DnsmasqManager>();
builder.Services.AddScoped<IHostapdManager, HostapdManager>();
builder.Services.AddScoped<ICaptivePortalControlManager, CaptivePortalControlManager>();
builder.Services.AddScoped<INetworkManager, NetworkManager>();
builder.Services.AddScoped<INetworkOrchestrator, NetworkOrchestrator>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IMacAddressResolver, MacAddressResolver>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<ISessionRepository, SessionRepository>();
builder.Services.AddScoped<IDeviceRepository, DeviceRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<ISessionManagementService, SessionManagementService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

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