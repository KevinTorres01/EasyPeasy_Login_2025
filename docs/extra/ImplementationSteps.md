# 🎯 Orden de Implementación N-Layered (Sin EF Core, Sin Clean/DDD)

## **Arquitectura N-Layered Tradicional para este Proyecto**

```
┌─────────────────────────────────────────────────────────────┐
│  PRESENTATION LAYER (EasyPeasy_Login.Web)                   │
│  - Blazor Components (Pages/Components)                     │
│  - Middlewares                                               │
│  - HostedServices                                            │
└─────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────┐
│  APPLICATION LAYER (EasyPeasy_Login.Application)            │
│  - Services (Business Logic)                                │
│  - DTOs                                                      │
│  - Validators                                                │
└─────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────┐
│  DOMAIN LAYER (EasyPeasy_Login.Domain)                      │
│  - Entities (POCOs)                                          │
│  - Interfaces (Contracts)                                    │
│  - Exceptions                                                │
└─────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────┐
│  INFRASTRUCTURE LAYER (EasyPeasy_Login.Infrastructure)      │
│  - Data (JSON Repositories)                                 │
│  - External Services (Network, System)                      │
└─────────────────────────────────────────────────────────────┘
```

---

## **📋 ORDEN DE IMPLEMENTACIÓN (Bottom-Up Approach)**

### **ETAPA 1: Domain Layer** 🧱
*Comenzar desde el núcleo del negocio*

#### **1.1 Crear el proyecto Domain**
```bash
cd src/
dotnet new classlib -n EasyPeasy_Login.Domain
cd EasyPeasy_Login.Domain
rm Class1.cs
```

#### **1.2 Definir Entidades**

````csharp
namespace EasyPeasy_Login.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
}
````

````csharp
namespace EasyPeasy_Login.Domain.Entities;

public class Session
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string ClientIpAddress { get; set; } = string.Empty;
    public string MacAddress { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsActive { get; set; }
}
````

````csharp
namespace EasyPeasy_Login.Domain.Entities;

public class Device
{
    public Guid Id { get; set; }
    public string MacAddress { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public string? Hostname { get; set; }
    public DateTime FirstSeenAt { get; set; }
    public DateTime LastSeenAt { get; set; }
    public bool IsBlocked { get; set; }
}
````

#### **1.3 Definir Interfaces (Contratos de Repositorio)**

````csharp
using EasyPeasy_Login.Domain.Entities;

namespace EasyPeasy_Login.Domain.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByUsernameAsync(string username);
    Task<IEnumerable<User>> GetAllAsync();
    Task AddAsync(User user);
    Task UpdateAsync(User user);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(string email);
}
````

````csharp
using EasyPeasy_Login.Domain.Entities;

namespace EasyPeasy_Login.Domain.Interfaces;

public interface ISessionRepository
{
    Task<Session?> GetByIdAsync(Guid id);
    Task<Session?> GetActiveByIpAsync(string ipAddress);
    Task<IEnumerable<Session>> GetActiveSessionsAsync();
    Task<IEnumerable<Session>> GetByUserIdAsync(Guid userId);
    Task AddAsync(Session session);
    Task UpdateAsync(Session session);
    Task DeleteAsync(Guid id);
    Task DeleteExpiredSessionsAsync();
}
````

````csharp
using EasyPeasy_Login.Domain.Entities;

namespace EasyPeasy_Login.Domain.Interfaces;

public interface IDeviceRepository
{
    Task<Device?> GetByMacAddressAsync(string macAddress);
    Task<Device?> GetByIpAddressAsync(string ipAddress);
    Task<IEnumerable<Device>> GetAllAsync();
    Task AddAsync(Device device);
    Task UpdateAsync(Device device);
    Task DeleteAsync(Guid id);
}
````

#### **1.4 Definir Excepciones Personalizadas**

````csharp
namespace EasyPeasy_Login.Domain.Exceptions;

public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
}

public class UserNotFoundException : DomainException
{
    public UserNotFoundException(string email) 
        : base($"User with email '{email}' not found") { }
}

public class InvalidCredentialsException : DomainException
{
    public InvalidCredentialsException() 
        : base("Invalid username or password") { }
}

public class SessionExpiredException : DomainException
{
    public SessionExpiredException() 
        : base("Session has expired") { }
}
````

---

### **ETAPA 2: Infrastructure Layer** 🔧
*Implementar la persistencia y servicios externos*

#### **2.1 Crear el proyecto Infrastructure**
```bash
cd src/
dotnet new classlib -n EasyPeasy_Login.Infrastructure
cd EasyPeasy_Login.Infrastructure
dotnet add reference ../EasyPeasy_Login.Domain/EasyPeasy_Login.Domain.csproj
dotnet add package BCrypt.Net-Next
```

#### **2.2 Implementar Repositorios JSON**

````csharp
using System.Text.Json;

namespace EasyPeasy_Login.Infrastructure.Data;

public abstract class JsonRepository<T> where T : class
{
    protected readonly string FilePath;
    protected List<T> Items = new();
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    protected JsonRepository(string fileName)
    {
        var dataDirectory = Path.Combine(AppContext.BaseDirectory, "Data");
        Directory.CreateDirectory(dataDirectory);
        FilePath = Path.Combine(dataDirectory, fileName);
        LoadData();
    }

    protected void LoadData()
    {
        if (File.Exists(FilePath))
        {
            var json = File.ReadAllText(FilePath);
            Items = JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
        }
    }

    protected async Task SaveDataAsync()
    {
        await _semaphore.WaitAsync();
        try
        {
            var json = JsonSerializer.Serialize(Items, new JsonSerializerOptions 
            { 
                WriteIndented = true 
            });
            await File.WriteAllTextAsync(FilePath, json);
        }
        finally
        {
            _semaphore.Release();
        }
    }
}
````

````csharp
using EasyPeasy_Login.Domain.Entities;
using EasyPeasy_Login.Domain.Interfaces;

namespace EasyPeasy_Login.Infrastructure.Data.Repositories;

public class UserRepository : JsonRepository<User>, IUserRepository
{
    public UserRepository() : base("users.json")
    {
        // Crear usuario por defecto si no existe
        if (!Items.Any())
        {
            var defaultUser = new User
            {
                Id = Guid.NewGuid(),
                Username = "admin",
                Email = "admin@portal.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            Items.Add(defaultUser);
            SaveDataAsync().Wait();
        }
    }

    public Task<User?> GetByIdAsync(Guid id)
    {
        return Task.FromResult(Items.FirstOrDefault(u => u.Id == id));
    }

    public Task<User?> GetByEmailAsync(string email)
    {
        return Task.FromResult(Items.FirstOrDefault(u => 
            u.Email.Equals(email, StringComparison.OrdinalIgnoreCase)));
    }

    public Task<User?> GetByUsernameAsync(string username)
    {
        return Task.FromResult(Items.FirstOrDefault(u => 
            u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)));
    }

    public Task<IEnumerable<User>> GetAllAsync()
    {
        return Task.FromResult<IEnumerable<User>>(Items);
    }

    public async Task AddAsync(User user)
    {
        user.Id = Guid.NewGuid();
        user.CreatedAt = DateTime.UtcNow;
        Items.Add(user);
        await SaveDataAsync();
    }

    public async Task UpdateAsync(User user)
    {
        var index = Items.FindIndex(u => u.Id == user.Id);
        if (index >= 0)
        {
            Items[index] = user;
            await SaveDataAsync();
        }
    }

    public async Task DeleteAsync(Guid id)
    {
        Items.RemoveAll(u => u.Id == id);
        await SaveDataAsync();
    }

    public Task<bool> ExistsAsync(string email)
    {
        return Task.FromResult(Items.Any(u => 
            u.Email.Equals(email, StringComparison.OrdinalIgnoreCase)));
    }
}
````

````csharp
using EasyPeasy_Login.Domain.Entities;
using EasyPeasy_Login.Domain.Interfaces;

namespace EasyPeasy_Login.Infrastructure.Data.Repositories;

public class SessionRepository : JsonRepository<Session>, ISessionRepository
{
    public SessionRepository() : base("sessions.json") { }

    public Task<Session?> GetByIdAsync(Guid id)
    {
        return Task.FromResult(Items.FirstOrDefault(s => s.Id == id));
    }

    public Task<Session?> GetActiveByIpAsync(string ipAddress)
    {
        var now = DateTime.UtcNow;
        return Task.FromResult(Items.FirstOrDefault(s => 
            s.ClientIpAddress == ipAddress && 
            s.IsActive && 
            s.ExpiresAt > now));
    }

    public Task<IEnumerable<Session>> GetActiveSessionsAsync()
    {
        var now = DateTime.UtcNow;
        return Task.FromResult<IEnumerable<Session>>(
            Items.Where(s => s.IsActive && s.ExpiresAt > now));
    }

    public Task<IEnumerable<Session>> GetByUserIdAsync(Guid userId)
    {
        return Task.FromResult<IEnumerable<Session>>(
            Items.Where(s => s.UserId == userId));
    }

    public async Task AddAsync(Session session)
    {
        session.Id = Guid.NewGuid();
        session.CreatedAt = DateTime.UtcNow;
        Items.Add(session);
        await SaveDataAsync();
    }

    public async Task UpdateAsync(Session session)
    {
        var index = Items.FindIndex(s => s.Id == session.Id);
        if (index >= 0)
        {
            Items[index] = session;
            await SaveDataAsync();
        }
    }

    public async Task DeleteAsync(Guid id)
    {
        Items.RemoveAll(s => s.Id == id);
        await SaveDataAsync();
    }

    public async Task DeleteExpiredSessionsAsync()
    {
        var now = DateTime.UtcNow;
        Items.RemoveAll(s => s.ExpiresAt <= now);
        await SaveDataAsync();
    }
}
````

````csharp
using EasyPeasy_Login.Domain.Entities;
using EasyPeasy_Login.Domain.Interfaces;

namespace EasyPeasy_Login.Infrastructure.Data.Repositories;

public class DeviceRepository : JsonRepository<Device>, IDeviceRepository
{
    public DeviceRepository() : base("devices.json") { }

    public Task<Device?> GetByMacAddressAsync(string macAddress)
    {
        return Task.FromResult(Items.FirstOrDefault(d => 
            d.MacAddress.Equals(macAddress, StringComparison.OrdinalIgnoreCase)));
    }

    public Task<Device?> GetByIpAddressAsync(string ipAddress)
    {
        return Task.FromResult(Items.FirstOrDefault(d => d.IpAddress == ipAddress));
    }

    public Task<IEnumerable<Device>> GetAllAsync()
    {
        return Task.FromResult<IEnumerable<Device>>(Items);
    }

    public async Task AddAsync(Device device)
    {
        device.Id = Guid.NewGuid();
        device.FirstSeenAt = DateTime.UtcNow;
        device.LastSeenAt = DateTime.UtcNow;
        Items.Add(device);
        await SaveDataAsync();
    }

    public async Task UpdateAsync(Device device)
    {
        var index = Items.FindIndex(d => d.Id == device.Id);
        if (index >= 0)
        {
            Items[index] = device;
            await SaveDataAsync();
        }
    }

    public async Task DeleteAsync(Guid id)
    {
        Items.RemoveAll(d => d.Id == id);
        await SaveDataAsync();
    }
}
````

#### **2.3 Implementar Servicios de Sistema (Red)**

````csharp
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace EasyPeasy_Login.Infrastructure.Services;

public class NetworkConfigurationService : IDisposable
{
    private readonly ILogger<NetworkConfigurationService> _logger;
    private readonly string _interface = "wlan0";
    private readonly string _gatewayIp = "192.168.100.1";
    private readonly string _dhcpRange = "192.168.100.50,192.168.100.150";
    private string? _originalIptablesRules;
    private bool _isConfigured = false;

    public NetworkConfigurationService(ILogger<NetworkConfigurationService> logger)
    {
        _logger = logger;
    }

    public async Task<bool> SetupNetworkAsync()
    {
        try
        {
            _logger.LogInformation("🚀 Iniciando configuración de red...");

            // Guardar configuración actual
            await BackupCurrentConfigurationAsync();

            // 1. Configurar interfaz
            await ExecuteCommandAsync($"ip addr add {_gatewayIp}/24 dev {_interface}");
            await ExecuteCommandAsync($"ip link set {_interface} up");

            // 2. IP Forwarding
            await ExecuteCommandAsync("sysctl -w net.ipv4.ip_forward=1");

            // 3. NAT
            await ExecuteCommandAsync("iptables -t nat -A POSTROUTING -o eth0 -j MASQUERADE");

            // 4. Bloquear tráfico por defecto
            await ExecuteCommandAsync($"iptables -A FORWARD -i {_interface} -j DROP");

            // 5. DHCP
            await ConfigureDnsmasqAsync();

            _isConfigured = true;
            _logger.LogInformation("✅ Red configurada exitosamente");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error configurando red");
            await RestoreConfigurationAsync();
            return false;
        }
    }

    private async Task ConfigureDnsmasqAsync()
    {
        var config = $@"interface={_interface}
dhcp-range={_dhcpRange},12h
dhcp-option=3,{_gatewayIp}
dhcp-option=6,{_gatewayIp}
address=/#/{_gatewayIp}";

        await File.WriteAllTextAsync("/etc/dnsmasq.d/captive-portal.conf", config);
        await ExecuteCommandAsync("systemctl restart dnsmasq");
    }

    private async Task BackupCurrentConfigurationAsync()
    {
        _originalIptablesRules = await ExecuteCommandAsync("iptables-save");
    }

    public async Task RestoreConfigurationAsync()
    {
        if (!_isConfigured) return;

        _logger.LogInformation("🔄 Restaurando configuración de red...");

        await ExecuteCommandAsync("iptables -F");
        await ExecuteCommandAsync("iptables -t nat -F");
        await ExecuteCommandAsync("sysctl -w net.ipv4.ip_forward=0");
        await ExecuteCommandAsync($"ip addr del {_gatewayIp}/24 dev {_interface}");
        await ExecuteCommandAsync("rm -f /etc/dnsmasq.d/captive-portal.conf");
        await ExecuteCommandAsync("systemctl restart dnsmasq");

        _logger.LogInformation("✅ Configuración restaurada");
    }

    private async Task<string> ExecuteCommandAsync(string command)
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
        string error = await process.StandardError.ReadToEndAsync();
        await process.WaitForExitAsync();

        if (process.ExitCode != 0 && !string.IsNullOrEmpty(error))
        {
            throw new Exception($"Command failed: {command}\nError: {error}");
        }

        return output;
    }

    public void Dispose()
    {
        RestoreConfigurationAsync().Wait();
    }
}
````

````csharp
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace EasyPeasy_Login.Infrastructure.Services;

public class NetworkService
{
    private readonly ILogger<NetworkService> _logger;
    private readonly HashSet<string> _allowedIps = new();

    public NetworkService(ILogger<NetworkService> logger)
    {
        _logger = logger;
    }

    public async Task AllowClientAsync(string ipAddress)
    {
        if (_allowedIps.Contains(ipAddress))
        {
            _logger.LogWarning("IP {IpAddress} ya estaba permitida", ipAddress);
            return;
        }

        try
        {
            // Permitir tráfico de esta IP
            await ExecuteCommandAsync($"iptables -I FORWARD -s {ipAddress} -j ACCEPT");
            _allowedIps.Add(ipAddress);
            _logger.LogInformation("✅ IP {IpAddress} desbloqueada", ipAddress);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error desbloqueando IP {IpAddress}", ipAddress);
            throw;
        }
    }

    public async Task BlockClientAsync(string ipAddress)
    {
        if (!_allowedIps.Contains(ipAddress))
        {
            return;
        }

        try
        {
            await ExecuteCommandAsync($"iptables -D FORWARD -s {ipAddress} -j ACCEPT");
            _allowedIps.Remove(ipAddress);
            _logger.LogInformation("🔒 IP {IpAddress} bloqueada", ipAddress);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error bloqueando IP {IpAddress}", ipAddress);
            throw;
        }
    }

    private async Task<string> ExecuteCommandAsync(string command)
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
}
````

---

### **ETAPA 3: Application Layer** 💼
*Implementar la lógica de negocio*

#### **3.1 Crear el proyecto Application**
```bash
cd src/
dotnet new classlib -n EasyPeasy_Login.Application
cd EasyPeasy_Login.Application
dotnet add reference ../EasyPeasy_Login.Domain/EasyPeasy_Login.Domain.csproj
dotnet add reference ../EasyPeasy_Login.Infrastructure/EasyPeasy_Login.Infrastructure.csproj
```

#### **3.2 Crear DTOs**

````csharp
namespace EasyPeasy_Login.Application.DTOs;

public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool AcceptTerms { get; set; }
}
````

````csharp
namespace EasyPeasy_Login.Application.DTOs;

public class LoginResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? Username { get; set; }
    public Guid? SessionId { get; set; }
}
````

#### **3.3 Implementar Servicios de Aplicación**

````csharp
using EasyPeasy_Login.Application.DTOs;
using EasyPeasy_Login.Domain.Entities;
using EasyPeasy_Login.Domain.Interfaces;
using EasyPeasy_Login.Domain.Exceptions;
using EasyPeasy_Login.Infrastructure.Services;
using Microsoft.Extensions.Logging;

namespace EasyPeasy_Login.Application.Services;

public class AuthenticationService
{
    private readonly IUserRepository _userRepository;
    private readonly ISessionRepository _sessionRepository;
    private readonly NetworkService _networkService;
    private readonly ILogger<AuthenticationService> _logger;

    public AuthenticationService(
        IUserRepository userRepository,
        ISessionRepository sessionRepository,
        NetworkService networkService,
        ILogger<AuthenticationService> logger)
    {
        _userRepository = userRepository;
        _sessionRepository = sessionRepository;
        _networkService = networkService;
        _logger = logger;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request, string clientIp, string macAddress)
    {
        try
        {
            // 1. Validar usuario
            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user == null)
            {
                _logger.LogWarning("Login fallido: usuario no encontrado - {Email}", request.Email);
                return new LoginResponse 
                { 
                    Success = false, 
                    Message = "Invalid credentials" 
                };
            }

            // 2. Verificar contraseña
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                _logger.LogWarning("Login fallido: contraseña incorrecta - {Email}", request.Email);
                return new LoginResponse 
                { 
                    Success = false, 
                    Message = "Invalid credentials" 
                };
            }

            // 3. Verificar usuario activo
            if (!user.IsActive)
            {
                _logger.LogWarning("Login fallido: usuario inactivo - {Email}", request.Email);
                return new LoginResponse 
                { 
                    Success = false, 
                    Message = "Account is disabled" 
                };
            }

            // 4. Crear sesión
            var session = new Session
            {
                UserId = user.Id,
                ClientIpAddress = clientIp,
                MacAddress = macAddress,
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                IsActive = true
            };
            await _sessionRepository.AddAsync(session);

            // 5. Desbloquear acceso a red
            await _networkService.AllowClientAsync(clientIp);

            // 6. Actualizar último login
            user.LastLoginAt = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);

            _logger.LogInformation("✅ Login exitoso: {Email} desde {IP}", request.Email, clientIp);

            return new LoginResponse
            {
                Success = true,
                Message = "Login successful",
                Username = user.Username,
                SessionId = session.Id
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error durante login para {Email}", request.Email);
            return new LoginResponse 
            { 
                Success = false, 
                Message = "An error occurred during login" 
            };
        }
    }

    public async Task<bool> IsAuthenticatedAsync(string ipAddress)
    {
        var session = await _sessionRepository.GetActiveByIpAsync(ipAddress);
        return session != null;
    }

    public async Task LogoutAsync(string ipAddress)
    {
        var session = await _sessionRepository.GetActiveByIpAsync(ipAddress);
        if (session != null)
        {
            session.IsActive = false;
            await _sessionRepository.UpdateAsync(session);
            await _networkService.BlockClientAsync(ipAddress);
            _logger.LogInformation("Logout exitoso para IP {IP}", ipAddress);
        }
    }
}
````

````csharp
using EasyPeasy_Login.Domain.Entities;
using EasyPeasy_Login.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace EasyPeasy_Login.Application.Services;

public class UserManagementService
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UserManagementService> _logger;

    public UserManagementService(
        IUserRepository userRepository,
        ILogger<UserManagementService> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await _userRepository.GetAllAsync();
    }

    public async Task<User?> GetUserByIdAsync(Guid id)
    {
        return await _userRepository.GetByIdAsync(id);
    }

    public async Task<bool> CreateUserAsync(string username, string email, string password)
    {
        // Verificar si ya existe
        if (await _userRepository.ExistsAsync(email))
        {
            _logger.LogWarning("Intento de crear usuario duplicado: {Email}", email);
            return false;
        }

        var user = new User
        {
            Username = username,
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            IsActive = true
        };

        await _userRepository.AddAsync(user);
        _logger.LogInformation("✅ Usuario creado: {Email}", email);
        return true;
    }

    public async Task<bool> UpdateUserAsync(Guid id, string username, string email, bool isActive)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
        {
            return false;
        }

        user.Username = username;
        user.Email = email;
        user.IsActive = isActive;

        await _userRepository.UpdateAsync(user);
        _logger.LogInformation("Usuario actualizado: {Email}", email);
        return true;
    }

    public async Task<bool> DeleteUserAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
        {
            return false;
        }

        await _userRepository.DeleteAsync(id);
        _logger.LogInformation("Usuario eliminado: {Email}", user.Email);
        return true;
    }

    public async Task<bool> ChangePasswordAsync(Guid id, string newPassword)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
        {
            return false;
        }

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
        await _userRepository.UpdateAsync(user);
        _logger.LogInformation("Contraseña cambiada para: {Email}", user.Email);
        return true;
    }
}
````

---

### **ETAPA 4: Presentation Layer** 🎨
*Implementar la interfaz web*

#### **4.1 Crear el proyecto Web (Blazor Server)**
```bash
cd src/
dotnet new blazor -n EasyPeasy_Login.Web --interactivity Server
cd EasyPeasy_Login.Web
dotnet add reference ../EasyPeasy_Login.Application/EasyPeasy_Login.Application.csproj
dotnet add reference ../EasyPeasy_Login.Domain/EasyPeasy_Login.Domain.csproj
dotnet add reference ../EasyPeasy_Login.Infrastructure/EasyPeasy_Login.Infrastructure.csproj
```

#### **4.2 Configurar Program.cs**

````csharp
using EasyPeasy_Login.Web.Components;
using EasyPeasy_Login.Domain.Interfaces;
using EasyPeasy_Login.Infrastructure.Data.Repositories;
using EasyPeasy_Login.Infrastructure.Services;
using EasyPeasy_Login.Application.Services;
using EasyPeasy_Login.Web.Middleware;

var builder = WebApplication.CreateBuilder(args);

// ============================================
// INFRASTRUCTURE LAYER
// ============================================
builder.Services.AddSingleton<IUserRepository, UserRepository>();
builder.Services.AddSingleton<ISessionRepository, SessionRepository>();
builder.Services.AddSingleton<IDeviceRepository, DeviceRepository>();
builder.Services.AddSingleton<NetworkConfigurationService>();
builder.Services.AddScoped<NetworkService>();

// ============================================
// APPLICATION LAYER
// ============================================
builder.Services.AddScoped<AuthenticationService>();
builder.Services.AddScoped<UserManagementService>();

// ============================================
// BLAZOR
// ============================================
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// ============================================
// MIDDLEWARE
// ============================================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseRequestRouter();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run("http://0.0.0.0:8080");
````

#### **4.3 Implementar Middleware**

````csharp
namespace EasyPeasy_Login.Web.Middleware;

public class RequestRouterMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string _serverIp = "192.168.100.1";

    public RequestRouterMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var clientIp = context.Connection.RemoteIpAddress?.ToString();
        var isLocalhost = clientIp == "127.0.0.1" || clientIp == "::1" || clientIp == _serverIp;

        var path = context.Request.Path.Value ?? "/";

        // Si es localhost, redirigir a /admin
        if (isLocalhost && !path.StartsWith("/admin") && !path.StartsWith("/_"))
        {
            context.Response.Redirect("/admin");
            return;
        }

        // Si es cliente remoto, redirigir a /portal
        if (!isLocalhost && !path.StartsWith("/portal") && !path.StartsWith("/_"))
        {
            context.Response.Redirect("/portal");
            return;
        }

        await _next(context);
    }
}

public static class RequestRouterMiddlewareExtensions
{
    public static IApplicationBuilder UseRequestRouter(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RequestRouterMiddleware>();
    }
}
````

#### **4.4 Crear Páginas**

````cshtml
@page "/portal"
@page "/portal/login"
@using EasyPeasy_Login.Application.Services
@using EasyPeasy_Login.Application.DTOs
@inject AuthenticationService AuthService
@inject NavigationManager Navigation
@rendermode InteractiveServer

<PageTitle>Welcome back!</PageTitle>

<div class="login-container">
    <div class="left-panel">
        <div class="avatar-placeholder">
            <p style="font-size: 4rem;">🎭</p>
            <p>(Avatares animados aquí)</p>
        </div>
    </div>

    <div class="right-panel">
        <div class="login-card">
            <div class="crown-icon">👑</div>
            <h1>Welcome back!</h1>
            <p class="subtitle">Please enter your details</p>

            <EditForm Model="loginModel" OnValidSubmit="HandleLogin">
                <div class="form-group">
                    <label>Email</label>
                    <InputText @bind-Value="loginModel.Email" class="form-control" />
                </div>

                <div class="form-group">
                    <label>Password</label>
                    <InputText type="password" @bind-Value="loginModel.Password" class="form-control" />
                </div>

                <div class="form-check">
                    <InputCheckbox @bind-Value="loginModel.AcceptTerms" class="form-check-input" id="terms" />
                    <label class="form-check-label" for="terms">
                        I accept the terms and conditions
                    </label>
                </div>

                @if (!string.IsNullOrEmpty(errorMessage))
                {
                    <div class="alert alert-danger">@errorMessage</div>
                }

                <button type="submit" class="btn-login" disabled="@isLoading">
                    @if (isLoading)
                    {
                        <span>Logging in...</span>
                    }
                    else
                    {
                        <span>Log in</span>
                    }
                </button>
            </EditForm>
        </div>
    </div>
</div>

@code {
    private LoginRequest loginModel = new();
    private string? errorMessage;
    private bool isLoading;

    private async Task HandleLogin()
    {
        if (!loginModel.AcceptTerms)
        {
            errorMessage = "You must accept the terms and conditions";
            return;
        }

        isLoading = true;
        errorMessage = null;

        // TODO: Obtener IP y MAC reales
        var result = await AuthService.LoginAsync(loginModel, "192.168.100.X", "00:00:00:00:00:00");

        isLoading = false;

        if (result.Success)
        {
            Navigation.NavigateTo("/portal/success");
        }
        else
        {
            errorMessage = result.Message;
        }
    }
}
````

````cshtml
@page "/admin"
@using EasyPeasy_Login.Application.Services
@using EasyPeasy_Login.Domain.Entities
@inject UserManagementService UserService
@rendermode InteractiveServer

<PageTitle>Admin Panel</PageTitle>

<div class="admin-container">
    <h1>👨‍💼 Admin Panel</h1>
    <h2>User Account Management</h2>

    @if (users == null)
    {
        <p>Loading...</p>
    }
    else
    {
        <table class="table">
            <thead>
                <tr>
                    <th>Username</th>
                    <th>Email</th>
                    <th>Status</th>
                    <th>Created</th>
                    <th>Last Login</th>
                </tr>
            </thead>
            <tbody>
                @foreach (var user in users)
                {
                    <tr>
                        <td>@user.Username</td>
                        <td>@user.Email</td>
                        <td>
                            <span class="badge @(user.IsActive ? "bg-success" : "bg-secondary")">
                                @(user.IsActive ? "Active" : "Inactive")
                            </span>
                        </td>
                        <td>@user.CreatedAt.ToString("yyyy-MM-dd")</td>
                        <td>@(user.LastLoginAt?.ToString("yyyy-MM-dd HH:mm") ?? "Never")</td>
                    </tr>
                }
            </tbody>
        </table>
    }
</div>

@code {
    private IEnumerable<User>? users;

    protected override async Task OnInitializedAsync()
    {
        users = await UserService.GetAllUsersAsync();
    }
}
````

---

## **📊 RESUMEN DEL ORDEN DE IMPLEMENTACIÓN**

### **1️⃣ Domain** → Entidades, Interfaces, Excepciones
- ✅ Definir el modelo de datos
- ✅ Establecer contratos (interfaces)
- ✅ Sin dependencias externas

### **2️⃣ Infrastructure** → Repositorios, Servicios de Sistema
- ✅ Implementar persistencia JSON
- ✅ Implementar servicios de red
- ✅ Depende de Domain

### **3️⃣ Application** → Servicios de Negocio, DTOs
- ✅ Implementar lógica de negocio
- ✅ Orquestar Domain e Infrastructure
- ✅ Depende de Domain e Infrastructure

### **4️⃣ Presentation** → Blazor, Middlewares, HostedServices
- ✅ Implementar UI
- ✅ Configurar inyección de dependencias
- ✅ Depende de todas las capas

---

## **🔧 Comandos para Crear la Solución**

```bash
# 1. Crear solución
dotnet new sln -n EasyPeasy_Login

# 2. Agregar proyectos a la solución
dotnet sln add src/EasyPeasy_Login.Domain/EasyPeasy_Login.Domain.csproj
dotnet sln add src/EasyPeasy_Login.Infrastructure/EasyPeasy_Login.Infrastructure.csproj
dotnet sln add src/EasyPeasy_Login.Application/EasyPeasy_Login.Application.csproj
dotnet sln add src/EasyPeasy_Login.Web/EasyPeasy_Login.Web.csproj

# 3. Compilar
dotnet build

# 4. Ejecutar
sudo dotnet run --project src/EasyPeasy_Login.Web
```

---

¿Te parece bien este orden? ¿Empezamos implementando la capa Domain?