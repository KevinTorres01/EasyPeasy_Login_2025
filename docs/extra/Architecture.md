 Cautivo

## **Arquitectura Propuesta: N-Layered Architecture**

Propongo una arquitectura en capas (N-Layered) adaptada específicamente para este portal cautivo, con una clara separación de responsabilidades y servicios bien definidos.

```
┌─────────────────────────────────────────────────────────────┐
│                    PRESENTATION LAYER                        │
│  ┌──────────────────┐              ┌──────────────────┐     │
│  │  Portal Web UI   │              │   Admin Web UI   │     │
│  │  (Blazor Pages)  │              │  (Blazor Pages)  │     │
│  └──────────────────┘              └──────────────────┘     │
└─────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────┐
│                      API/ROUTING LAYER                       │
│  ┌──────────────────────────────────────────────────────┐   │
│  │           Request Router Middleware                   │   │
│  │  - IP/MAC Detection                                   │   │
│  │  - Authentication State Check                         │   │
│  │  - UI Selection (Portal vs Admin)                     │   │
│  └──────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────┐
│                     BUSINESS LOGIC LAYER                     │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐      │
│  │ Auth Service │  │ User Service │  │ Session Mgmt │      │
│  └──────────────┘  └──────────────┘  └──────────────┘      │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐      │
│  │Device Service│  │ Network Svc  │  │Captive Portal│      │
│  │              │  │              │  │  Detection   │      │
│  └──────────────┘  └──────────────┘  └──────────────┘      │
└─────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────┐
│                    DATA ACCESS LAYER                         │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐      │
│  │User Repository│ │Device Repo   │  │Session Repo  │      │
│  └──────────────┘  └──────────────┘  └──────────────┘      │
└─────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────┐
│                   INFRASTRUCTURE LAYER                       │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐      │
│  │   JSON DB    │  │  iptables    │  │   dnsmasq    │      │
│  │   Storage    │  │   Manager    │  │   Manager    │      │
│  └──────────────┘  └──────────────┘  └──────────────┘      │
└─────────────────────────────────────────────────────────────┘
```

---

## **Capas Detalladas**

### **1. Presentation Layer (Capa de Presentación)**

**Responsabilidad**: Renderizar las interfaces de usuario y manejar interacciones

**Componentes**:
- **Portal Web UI**: Interfaz para usuarios no autenticados
  - Login Page
  - Terms & Conditions Page
  - Error Pages
  - Avatares animados y elementos interactivos
  
- **Admin Web UI**: Interfaz para administración local
  - Dashboard con estadísticas
  - User Management Interface
  - Device Monitoring Panel
  - System Configuration Panel

**Tecnología**: Blazor Server Components

---

### **2. API/Routing Layer (Capa de Enrutamiento)**

**Responsabilidad**: Decidir qué interfaz mostrar y gestionar redirecciones

**Componente Principal**: `RequestRouterMiddleware`

**Funciones clave**:
```csharp
// Pseudocódigo de lógica de enrutamiento
if (IsLocalRequest(context)) {
    // Mostrar Admin UI
    return AdminInterface;
} 
else if (IsAuthenticatedDevice(context)) {
    // Permitir paso a Internet
    return AllowThrough;
} 
else {
    // Redirigir a Portal de Login
    return CaptivePortalInterface;
}
```

**Decisiones que toma**:
- ¿Es una petición desde localhost/IP del servidor? → Admin UI
- ¿Es un dispositivo autenticado? → Permitir navegación normal
- ¿Es un endpoint de detección de portal cautivo? → Responder apropiadamente
- Caso contrario → Redirigir a Portal de Login

---

### **3. Business Logic Layer (Capa de Lógica de Negocio)**

**Servicios Definidos**:

#### **AuthenticationService**
```csharp
public interface IAuthenticationService
{
    Task<AuthResult> ValidateCredentials(string username, string password);
    Task<bool> AuthorizeDevice(string ipAddress, string macAddress);
    Task<bool> RevokeDeviceAccess(string identifier);
    Task<SessionInfo> GetActiveSession(string deviceId);
}
```

**Responsabilidades**:
- Validar credenciales de usuario
- Autorizar dispositivos en el firewall
- Gestionar sesiones activas
- Revocar accesos

---

#### **UserManagementService**
```csharp
public interface IUserManagementService
{
    Task<User> CreateUser(UserDto userDto);
    Task<User> GetUser(string username);
    Task<IEnumerable<User>> GetAllUsers();
    Task<bool> UpdateUser(string username, UserDto updates);
    Task<bool> DeleteUser(string username);
    Task<bool> ChangePassword(string username, string newPassword);
}
```

**Responsabilidades**:
- CRUD de usuarios
- Gestión de credenciales
- Validación de datos de usuario

---

#### **DeviceManagementService**
```csharp
public interface IDeviceManagementService
{
    Task<Device> RegisterDevice(string ipAddress, string macAddress);
    Task<Device> GetDevice(string identifier);
    Task<IEnumerable<Device>> GetAllConnectedDevices();
    Task<IEnumerable<Device>> GetAuthenticatedDevices();
    Task<bool> UpdateDeviceInfo(string deviceId, DeviceDto updates);
    Task<ConnectionHistory> GetDeviceHistory(string macAddress);
}
```

**Responsabilidades**:
- Registro y tracking de dispositivos
- Asociación IP-MAC
- Historial de conexiones
- Estado de autenticación por dispositivo

---

#### **NetworkControlService**
```csharp
public interface INetworkControlService
{
    Task<bool> AllowDeviceInternet(string ipAddress, string macAddress);
    Task<bool> BlockDeviceInternet(string identifier);
    Task<bool> RedirectToPortal(string ipAddress);
    Task<FirewallStatus> GetFirewallStatus();
    Task<bool> RestoreNetworkConfiguration();
}
```

**Responsabilidades**:
- Gestión de reglas de iptables
- Control de NAT y forwarding
- Redirecciones HTTP/HTTPS
- Restauración de configuración de red

---

#### **SessionManagementService**
```csharp
public interface ISessionManagementService
{
    Task<Session> CreateSession(string deviceId, string userId);
    Task<Session> GetActiveSession(string deviceId);
    Task<bool> ExtendSession(string sessionId, TimeSpan extension);
    Task<bool> TerminateSession(string sessionId);
    Task<IEnumerable<Session>> GetAllActiveSessions();
    Task CleanupExpiredSessions();
}
```

**Responsabilidades**:
- Creación y gestión de sesiones
- Control de tiempo de acceso
- Limpieza de sesiones expiradas
- Asociación usuario-dispositivo

---

#### **CaptivePortalDetectionService**
```csharp
public interface ICaptivePortalDetectionService
{
    Task<DetectionResponse> HandleDetectionRequest(DetectionRequest request);
    Task<bool> RegisterDetectionEndpoint(string endpoint, OSType osType);
    Task<IEnumerable<string>> GetSupportedEndpoints();
}
```

**Responsabilidades**:
- Responder a peticiones de detección de portales cautivos
- Soportar múltiples endpoints (iOS, Android, Windows, etc.)
- Generar respuestas específicas por sistema operativo

Ejemplos de endpoints:
- `/hotspot-detect.html` (iOS/macOS)
- `/generate_204` (Android)
- `/connecttest.txt` (Windows)

---

### **4. Data Access Layer (Capa de Acceso a Datos)**

**Repositorios**:

#### **UserRepository**
```csharp
public interface IUserRepository
{
    Task<User> GetByUsername(string username);
    Task<IEnumerable<User>> GetAll();
    Task<bool> Add(User user);
    Task<bool> Update(User user);
    Task<bool> Delete(string username);
    Task<bool> Exists(string username);
}
```

---

#### **DeviceRepository**
```csharp
public interface IDeviceRepository
{
    Task<Device> GetByIp(string ipAddress);
    Task<Device> GetByMac(string macAddress);
    Task<IEnumerable<Device>> GetAll();
    Task<bool> Add(Device device);
    Task<bool> Update(Device device);
    Task<IEnumerable<Device>> GetAuthenticatedDevices();
}
```

---

#### **SessionRepository**
```csharp
public interface ISessionRepository
{
    Task<Session> GetById(string sessionId);
    Task<Session> GetActiveByDevice(string deviceId);
    Task<IEnumerable<Session>> GetAllActive();
    Task<bool> Add(Session session);
    Task<bool> Update(Session session);
    Task<bool> Delete(string sessionId);
    Task<int> DeleteExpired();
}
```

---

### **5. Infrastructure Layer (Capa de Infraestructura)**

**Componentes**:

#### **JSONDatabaseManager**
```csharp
public interface IDataStore
{
    Task<T> Read<T>(string key);
    Task<bool> Write<T>(string key, T data);
    Task<bool> Delete(string key);
    Task<IEnumerable<string>> GetAllKeys();
}
```

**Archivos**:
- `users.json` - Credenciales y datos de usuarios
- `devices.json` - Información de dispositivos conectados
- `sessions.json` - Sesiones activas
- `config.json` - Configuración del sistema

---

#### **IptablesManager**
```csharp
public interface IFirewallManager
{
    Task<bool> AddAllowRule(string ipAddress, string macAddress);
    Task<bool> RemoveAllowRule(string identifier);
    Task<bool> AddRedirectRule(string ipAddress);
    Task<bool> EnableNAT(string interface);
    Task<bool> EnableForwarding();
    Task<bool> FlushRules();
    Task<string> GetRulesList();
}
```

**Comandos ejecutados**:
```bash
# Permitir dispositivo autenticado
iptables -I FORWARD -s <IP> -m mac --mac-source <MAC> -j ACCEPT
iptables -t nat -A POSTROUTING -s <IP> -o <WAN_IF> -j MASQUERADE

# Redirigir a portal
iptables -t nat -A PREROUTING -s <IP> -p tcp --dport 80 -j DNAT --to-destination <SERVER_IP>:80
iptables -t nat -A PREROUTING -s <IP> -p tcp --dport 443 -j DNAT --to-destination <SERVER_IP>:443
```

---

#### **DnsmasqManager**
```csharp
public interface IDnsManager
{
    Task<bool> StartDHCP(DHCPConfig config);
    Task<bool> StartDNS(DNSConfig config);
    Task<bool> Stop();
    Task<bool> Restart();
    Task<IEnumerable<Lease>> GetActiveLeases();
}
```

**Configuración generada**:
```conf
# DHCP
dhcp-range=192.168.10.50,192.168.10.150,12h
dhcp-option=option:router,192.168.10.1
dhcp-option=option:dns-server,192.168.10.1

# DNS - Redirigir todo al servidor
address=/#/192.168.10.1
```

---

## **SECCIÓN CRÍTICA: Diferenciación Admin UI vs Portal UI**

### **Estrategia de Implementación**

Para que la laptop servidor muestre el Admin UI mientras los clientes ven el Portal de Login, implementamos un **middleware de enrutamiento inteligente** que decide qué interfaz servir basándose en el origen de la petición.

---

### **Componente: RequestRouterMiddleware**

```csharp
public class RequestRouterMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;
    
    public RequestRouterMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _configuration = configuration;
    }
    
    public async Task InvokeAsync(HttpContext context, 
                                   IDeviceManagementService deviceService,
                                   ISessionManagementService sessionService)
    {
        var clientIp = context.Connection.RemoteIpAddress?.ToString();
        var requestPath = context.Request.Path.Value;
        
        // 1. DETECTAR SI ES PETICIÓN LOCAL (ADMIN)
        if (IsLocalRequest(context))
        {
            // Redirigir a Admin UI
            if (!requestPath.StartsWith("/admin"))
            {
                context.Response.Redirect("/admin");
                return;
            }
            await _next(context);
            return;
        }
        
        // 2. DETECTAR SI ES ENDPOINT DE DETECCIÓN DE PORTAL CAUTIVO
        if (IsCaptiveDetectionEndpoint(requestPath))
        {
            await HandleCaptiveDetection(context);
            return;
        }
        
        // 3. VERIFICAR SI EL DISPOSITIVO ESTÁ AUTENTICADO
        var device = await deviceService.GetDevice(clientIp);
        var session = await sessionService.GetActiveSession(device?.Id);
        
        if (session != null && session.IsActive)
        {
            // Dispositivo autenticado - permitir navegación normal
            await _next(context);
            return;
        }
        
        // 4. DISPOSITIVO NO AUTENTICADO - MOSTRAR PORTAL
        if (!requestPath.StartsWith("/portal"))
        {
            context.Response.Redirect("/portal/login");
            return;
        }
        
        await _next(context);
    }
    
    private bool IsLocalRequest(HttpContext context)
    {
        var remoteIp = context.Connection.RemoteIpAddress;
        var localIp = context.Connection.LocalIpAddress;
        
        // Verificar localhost
        if (IPAddress.IsLoopback(remoteIp))
            return true;
        
        // Verificar si la IP remota es la misma que la local
        if (remoteIp.Equals(localIp))
            return true;
        
        // Verificar IPs configuradas como administrativas
        var adminIps = _configuration.GetSection("AdminIPs").Get<string[]>();
        if (adminIps != null && adminIps.Contains(remoteIp.ToString()))
            return true;
        
        return false;
    }
    
    private bool IsCaptiveDetectionEndpoint(string path)
    {
        var detectionEndpoints = new[]
        {
            "/hotspot-detect.html",
            "/generate_204",
            "/connecttest.txt",
            "/ncsi.txt",
            "/success.txt"
        };
        
        return detectionEndpoints.Any(e => path.Equals(e, StringComparison.OrdinalIgnoreCase));
    }
    
    private async Task HandleCaptiveDetection(HttpContext context)
    {
        var path = context.Request.Path.Value;
        
        // Responder de forma que active la notificación de portal cautivo
        context.Response.StatusCode = 302; // Redirect
        context.Response.Headers["Location"] = $"http://{context.Request.Host}/portal/login";
        await context.Response.WriteAsync("");
    }
}
```

---

### **Estructura de Rutas en Blazor**

```
/portal
├── /login                    → Página de login (clientes)
├── /terms                    → Términos y condiciones
└── /success                  → Página post-autenticación

/admin                        → Solo accesible localmente
├── /dashboard                → Panel principal
├── /users                    → Gestión de usuarios
├── /devices                  → Monitoreo de dispositivos
└── /settings                 → Configuración del sistema

/api
├── /auth/login               → Endpoint de autenticación
├── /auth/logout              → Endpoint de cierre de sesión
└── /detection/*              → Endpoints de detección captiva
```

---

### **Configuración en Program.cs**

```csharp
var builder = WebApplication.CreateBuilder(args);

// Servicios
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Registrar servicios del sistema
builder.Services.AddSingleton<IAuthenticationService, AuthenticationService>();
builder.Services.AddSingleton<IDeviceManagementService, DeviceManagementService>();
builder.Services.AddSingleton<ISessionManagementService, SessionManagementService>();
builder.Services.AddSingleton<INetworkControlService, NetworkControlService>();
builder.Services.AddSingleton<IUserManagementService, UserManagementService>();
builder.Services.AddSingleton<ICaptivePortalDetectionService, CaptivePortalDetectionService>();

// Repositorios
builder.Services.AddSingleton<IUserRepository, UserRepository>();
builder.Services.AddSingleton<IDeviceRepository, DeviceRepository>();
builder.Services.AddSingleton<ISessionRepository, SessionRepository>();

// Infraestructura
builder.Services.AddSingleton<IDataStore, JSONDataStore>();
builder.Services.AddSingleton<IFirewallManager, IptablesManager>();
builder.Services.AddSingleton<IDnsManager, DnsmasqManager>();

var app = builder.Build();

// Middleware de enrutamiento ANTES de otros middlewares
app.UseMiddleware<RequestRouterMiddleware>();

app.UseStaticFiles();
app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/portal/_Host"); // Para clientes
app.MapFallbackToPage("/admin/_AdminHost"); // Para admin

app.Run();
```

---

### **Páginas Blazor Separadas**

**Para Clientes** (`Pages/Portal/_Host.cshtml`):
```cshtml
@page "/portal"
@namespace CaptivePortal.Pages.Portal
@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers

<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Portal de Acceso</title>
    <base href="/portal/" />
    <link href="css/portal.css" rel="stylesheet" />
</head>
<body>
    <component type="typeof(Portal.Login)" render-mode="ServerPrerendered" />
    <script src="_framework/blazor.server.js"></script>
</body>
</html>
```

**Para Admin** (`Pages/Admin/_AdminHost.cshtml`):
```cshtml
@page "/admin"
@namespace CaptivePortal.Pages.Admin
@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers

<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Panel de Administración</title>
    <base href="/admin/" />
    <link href="css/admin.css" rel="stylesheet" />
</head>
<body>
    <component type="typeof(Admin.Dashboard)" render-mode="ServerPrerendered" />
    <script src="_framework/blazor.server.js"></script>
</body>
</html>
```

---

### **Flujo de Decisión Visual**

```
┌─────────────────────┐
│  Petición HTTP      │
│  llega al servidor  │
└──────────┬──────────┘
           │
           ▼
┌─────────────────────┐
│ RequestRouter       │
│ Middleware          │
└──────────┬──────────┘
           │
           ├──────────────────────────────┐
           │                              │
           ▼                              ▼
    ┌─────────────┐              ┌──────────────┐
    │ ¿Es local?  │              │ ¿Autenticado?│
    │  (127.0.0.1 │              │  (Session    │
    │  o IP srv)  │              │   activa?)   │
    └──────┬──────┘              └──────┬───────┘
           │                             │
     SÍ    │    NO               SÍ     │    NO
           │                             │
           ▼                             ▼
    ┌─────────────┐              ┌─────────────┐
    │ Admin UI    │              │ Portal UI   │
    │ /admin/*    │              │ /portal/*   │
    └─────────────┘              └─────────────┘
           │                             │
           ▼                             ▼
    ┌─────────────┐              ┌─────────────┐
    │ Dashboard   │              │ Login Page  │
    │ User Mgmt   │              │ + Avatares  │
    │ Device Mon  │              │ + Términos  │
    └─────────────┘              └─────────────┘
```

---

### **Configuración de Red Integrada**

Para que todo funcione correctamente, el `NetworkControlService` debe configurar:

**1. DNS (dnsmasq.conf)**
```conf
# Capturar todas las consultas DNS
address=/#/192.168.10.1

# Excepto para el servidor mismo
server=8.8.8.8
```

**2. Firewall (iptables)**
```bash
# Permitir tráfico local (admin)
iptables -A INPUT -i lo -j ACCEPT
iptables -A INPUT -s 192.168.10.1 -j ACCEPT

# Redirigir HTTP/HTTPS de no autenticados
iptables -t nat -A PREROUTING -i wlan0 -p tcp --dport 80 -j DNAT --to-destination 192.168.10.1:5000
iptables -t nat -A PREROUTING -i wlan0 -p tcp --dport 443 -j DNAT --to-destination 192.168.10.1:5001
```

**3. Servidor web escuchando en**:
- `http://192.168.10.1:5000` (Portal para clientes)
- `http://localhost:5000` o `http://127.0.0.1:5000` (Admin local)

---

### **Ventajas de Esta Arquitectura**

1. **Separación Clara**: El middleware decide la ruta antes de que Blazor renderice
2. **Seguridad**: Admin UI inaccesible desde la red de clientes
3. **Flexibilidad**: Fácil añadir más reglas de enrutamiento
4. **Mantenibilidad**: Cada interfaz tiene sus propios componentes y estilos
5. **Escalabilidad**: Los servicios son inyectables y reemplazables

---

## **Modelos de Datos**

### **User**
```csharp
public class User
{
    public string Id { get; set; }
    public string Username { get; set; }
    public string PasswordHash { get; set; }
    public string Email { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLogin { get; set; }
    public bool IsActive { get; set; }
    public UserRole Role { get; set; }
}

public enum UserRole
{
    Guest,
    User,
    Admin
}
```

### **Device**
```csharp
public class Device
{
    public string Id { get; set; }
    public string IpAddress { get; set; }
    public string MacAddress { get; set; }
    public string Hostname { get; set; }
    public DateTime FirstSeen { get; set; }
    public DateTime LastSeen { get; set; }
    public bool IsAuthenticated { get; set; }
    public string CurrentUserId { get; set; }
}
```

### **Session**
```csharp
public class Session
{
    public string Id { get; set; }
    public string DeviceId { get; set; }
    public string UserId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsActive { get; set; }
    public string OriginalDestination { get; set; }
}
```

---

## **Integración de Componentes**

### **Inicialización del Sistema**

```csharp
public class CaptivePortalHostedService : IHostedService
{
    private readonly INetworkControlService _networkService;
    private readonly IDnsManager _dnsManager;
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        // 1. Configurar red
        await _networkService.EnableForwarding();
        await _networkService.EnableNAT("eth0");
        
        // 2. Iniciar DHCP/DNS
        await _dnsManager.StartDHCP(new DHCPConfig { ... });
        await _dnsManager.StartDNS(new DNSConfig { ... });
        
        // 3. Configurar reglas de firewall base
        await _networkService.SetupBaseFirewallRules();
    }
    
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        // Restaurar configuración original
        await _networkService.RestoreNetworkConfiguration();
        await _dnsManager.Stop();
    }
}
```

---

Esta arquitectura proporciona:
- ✅ Separación clara entre Portal y Admin
- ✅ Servicios bien definidos y cohesivos
- ✅ Capas con responsabilidades únicas
- ✅ Fácil mantenimiento y testing
- ✅ Seguridad por diseño

¿Quieres que profundice en alguna capa o servicio específico?