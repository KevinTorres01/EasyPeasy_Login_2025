using EasyPeasy_Login.Web.Components;
using EasyPeasy_Login.Application.Services;
using EasyPeasy_Login.Domain.Helper;
using EasyPeasy_Login.Infrastructure.Data.Repositories;
using EasyPeasy_Login.Infrastructure.Network.Configuration;
using EasyPeasy_Login.Application.Services.SessionManagement;
using EasyPeasy_Login.Web.Middleware;
using EasyPeasy_Login.Domain.Interfaces;
//using EasyPeasy_Login.Web.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddControllers();

// Register presentation services

builder.Services.AddScoped<INetworkConfigurationService, NetworkConfigurationService>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();

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
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

// customized Middleware
app.UseMiddleware<RequestRouterMiddleware>();


// Endpoints
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapControllers(); // for API controllers

app.Run("http://0.0.0.0:8080");
