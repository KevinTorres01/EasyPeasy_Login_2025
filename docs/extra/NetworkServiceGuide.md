# 📚 Explicación Detallada de Servicios y su Integración

## **Servicios Propuestos vs Existentes**

---

## **1. NetworkConfigurationService** 🌐

### **Responsabilidad:**
Gestionar la configuración de bajo nivel del sistema operativo Linux para crear el hotspot y controlar el tráfico de red.

### **Tareas Específicas:**
1. **Configurar la interfaz de red WiFi** (`wlan0`)
   - Asignarle una IP estática (192.168.100.1)
   - Activar la interfaz

2. **Habilitar IP Forwarding**
   - Permitir que el servidor actúe como router
   - Reenviar paquetes entre interfaces

3. **Configurar NAT (Network Address Translation)**
   - Permitir que dispositivos conectados accedan a Internet
   - Enmascarar IPs internas detrás de la IP del servidor

4. **Establecer reglas de firewall (iptables)**
   - Por defecto: BLOQUEAR todo el tráfico
   - Posteriormente: permitir IPs específicas (usuarios autenticados)

5. **Configurar servidor DHCP (dnsmasq)**
   - Asignar IPs automáticamente a dispositivos
   - Configurar el gateway (apuntando al servidor)
   - Configurar DNS (para redirección)

6. **Guardar y Restaurar configuración**
   - Hacer backup de la configuración original del sistema
   - Restaurar todo al estado previo cuando la app se detenga

### **Integración con Servicios Existentes:**

```
NetworkConfigurationService
    ↓
    ├─ Se registra como Singleton en Program.cs
    ├─ Es usado por NetworkSetupHostedService
    └─ Será usado por NetworkService (propuesto en docs)
         para desbloquear IPs específicas
```

**Diferencia con NetworkService (de tus docs):**
- `NetworkConfigurationService`: Configuración INICIAL del sistema (una sola vez al arrancar)
- `NetworkService`: Operaciones DINÁMICAS durante ejecución (permitir/bloquear IPs individuales)

### **Ejemplo de Uso:**
````csharp
// Al iniciar la aplicación (automático)
NetworkConfigurationService.SetupNetwork()
    → Configura interfaz, DHCP, iptables, etc.

// Al detener la aplicación (automático)
NetworkConfigurationService.RestoreConfiguration()
    → Restaura sistema al estado original

// Durante ejecución (lo implementaremos después)
NetworkService.AllowClient("192.168.100.50")
    → Agrega regla de iptables para permitir SOLO esa IP
```

---

## **2. NetworkSetupHostedService** 🚀

### **Responsabilidad:**
Orquestar el inicio y detención de la configuración de red en el ciclo de vida de la aplicación ASP.NET Core.

### **Tareas Específicas:**
1. **Al iniciar la app** (`StartAsync`):
   - Llamar a `NetworkConfigurationService.SetupNetwork()`
   - Validar que la configuración fue exitosa
   - Si falla: abortar la aplicación (no tiene sentido continuar sin red)

2. **Al detener la app** (`StopAsync`):
   - Llamar a `NetworkConfigurationService.RestoreConfiguration()`
   - Limpiar recursos
   - Dejar el sistema en estado original

### **Integración:**

```
ASP.NET Core Application Lifecycle
    ↓
    ├─ Startup
    │   └─ IHostedService.StartAsync()
    │       └─ NetworkSetupHostedService.StartAsync()
    │           └─ NetworkConfigurationService.SetupNetwork()
    │
    └─ Shutdown
        └─ IHostedService.StopAsync()
            └─ NetworkSetupHostedService.StopAsync()
                └─ NetworkConfigurationService.RestoreConfiguration()
```

**Integración con otros HostedServices (de tus docs):**

Según `FilesAndFolders.md`, proponías estos HostedServices:
- `SessionCleanupHostedService`: Limpiar sesiones expiradas
- `NetworkMonitoringHostedService`: Monitorear estado de la red
- `CaptivePortalDetectionHostedService`: Responder a detección de portal cautivo

**Orden de ejecución sugerido:**
````csharp
// Program.cs
builder.Services.AddHostedService<NetworkSetupHostedService>();        // 1º - Configurar red
builder.Services.AddHostedService<CaptivePortalDetectionHostedService>(); // 2º - Detección
builder.Services.AddHostedService<SessionCleanupHostedService>();      // 3º - Limpieza
builder.Services.AddHostedService<NetworkMonitoringHostedService>();   // 4º - Monitoreo
````

---

## **3. RequestRouterMiddleware** 🔀

### **Responsabilidad:**
Interceptar TODAS las peticiones HTTP y decidir qué interfaz mostrar (Portal para clientes, Admin para localhost).

### **Tareas Específicas:**
1. **Detectar origen de la petición**
   - Obtener IP del cliente
   - Determinar si es local (127.0.0.1, ::1, 192.168.100.1) o remoto

2. **Decidir ruta apropiada**
   - Localhost → `/admin/*`
   - Clientes remotos → `/portal/*`

3. **Redirigir si es necesario**
   - Si localhost accede a `/portal` → redirigir a `/admin`
   - Si cliente remoto accede a `/admin` → redirigir a `/portal`

### **Integración en el Pipeline de ASP.NET Core:**

```
HTTP Request
    ↓
1. Kestrel (servidor web)
    ↓
2. ExceptionHandlerMiddleware
    ↓
3. RequestRouterMiddleware ← AQUÍ SE INTEGRA
    │   ├─ Detecta IP
    │   ├─ Decide ruta
    │   └─ Redirige si es necesario
    ↓
4. StaticFilesMiddleware
    ↓
5. RoutingMiddleware
    ↓
6. AntiforgeryMiddleware
    ↓
7. Blazor Endpoints
    ↓
Response
```

**Orden de middlewares en Program.cs:**
````csharp
var app = builder.Build();

// 1º - Manejo de excepciones
app.UseExceptionHandler("/Error");

// 2º - ENRUTAMIENTO PERSONALIZADO (antes de routing)
app.UseRequestRouter();  // ← NUEVO

// 3º - Archivos estáticos
app.UseStaticFiles();

// 4º - Routing de ASP.NET
app.UseRouting();

// 5º - Antiforgery
app.UseAntiforgery();

// 6º - Endpoints
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();
````

### **Integración con otros Middlewares (de tus docs):**

Según `FilesAndFolders.md`, proponías:
- `ExceptionHandlingMiddleware`: Capturar errores
- `RequestLoggingMiddleware`: Registrar peticiones

**Pipeline completo sugerido:**
````csharp
app.UseExceptionHandler("/Error");           // 1º
app.UseMiddleware<RequestLoggingMiddleware>(); // 2º - Loggear TODO
app.UseRequestRouter();                        // 3º - Enrutar según IP
app.UseStaticFiles();                          // 4º
// ... resto
````

---

## **4. JsonUserRepository** 📦

### **Responsabilidad:**
Gestionar el almacenamiento y recuperación de usuarios en un archivo JSON (persistencia simple).

### **Tareas Específicas:**
1. **Cargar usuarios al iniciar**
   - Leer archivo `Data/users.json`
   - Deserializar a objetos `User`
   - Si no existe: crear usuarios por defecto

2. **Buscar usuarios**
   - Por email
   - Por username
   - Por ID

3. **Guardar cambios**
   - Serializar lista de usuarios
   - Escribir al archivo JSON

4. **Operaciones CRUD**
   - Create: Agregar nuevo usuario
   - Read: Obtener usuario(s)
   - Update: Modificar usuario existente
   - Delete: Eliminar usuario

### **Integración con tu Arquitectura:**

Según `Architecture.md`, tu arquitectura propone:

```
DATA ACCESS LAYER
    ├─ IUserRepository (interfaz)
    ├─ JsonUserRepository (implementación JSON)
    └─ SqliteUserRepository (implementación futura)
```

**Mi propuesta simplificada:**
````csharp
// Fase 1: Implementación directa (sin interfaz)
public class JsonUserRepository
{
    // Implementación concreta
}

// Registrar en Program.cs
builder.Services.AddSingleton<JsonUserRepository>();

// Fase 2: Abstraer con interfaz (cuando necesites cambiar a SQLite)
public interface IUserRepository
{
    User? GetByEmail(string email);
    void Add(User user);
    void Update(User user);
    void Delete(string email);
}

public class JsonUserRepository : IUserRepository { /* ... */ }

// Registrar en Program.cs
builder.Services.AddSingleton<IUserRepository, JsonUserRepository>();
```

**Integración con otros Repositorios (de tus docs):**

Según `FilesAndFolders.md`:
- `IUserRepository` / `UserRepository`
- `IDeviceRepository` / `DeviceRepository`
- `ISessionRepository` / `SessionRepository`

**Estructura sugerida:**
````
Infrastructure/
└─ Data/
    ├─ users.json          ← Datos de usuarios
    ├─ devices.json        ← Dispositivos conectados
    ├─ sessions.json       ← Sesiones activas
    │
    └─ Repositories/
        ├─ JsonUserRepository.cs
        ├─ JsonDeviceRepository.cs
        └─ JsonSessionRepository.cs
````

---

## **5. AuthenticationService** 🔐

### **Responsabilidad:**
Ejecutar la lógica de negocio de autenticación (verificar credenciales, crear sesiones, desbloquear red).

### **Tareas Específicas:**
1. **Validar credenciales**
   - Buscar usuario por email
   - Verificar password con BCrypt
   - Registrar intentos fallidos

2. **Crear sesión**
   - Generar token de sesión
   - Almacenar en `sessions.json`
   - Asociar IP del cliente

3. **Desbloquear acceso a red**
   - Llamar a `NetworkService.AllowClient(ip)`
   - Agregar regla de iptables

4. **Gestionar estado de autenticación**
   - Verificar si IP ya está autenticada
   - Validar sesiones activas
   - Cerrar sesión

### **Integración en tu Arquitectura:**

Según `Architecture.md`:

```
APPLICATION LAYER (Services/)
    ├─ AuthenticationService     ← ESTE SERVICIO
    ├─ UserManagementService
    ├─ SessionManagementService
    └─ DeviceManagementService
         ↓
DOMAIN LAYER (Entities/)
    ├─ User
    ├─ Device
    └─ Session
         ↓
INFRASTRUCTURE LAYER (Repositories/)
    ├─ JsonUserRepository
    ├─ JsonDeviceRepository
    └─ JsonSessionRepository
```

**Flujo de autenticación completo:**

````csharp
// 1. Usuario envía credenciales
Login.razor.HandleLogin()
    ↓
// 2. Llamada al servicio de aplicación
AuthenticationService.LoginAsync(email, password, clientIp)
    ↓
    ├─ 2.1. Validar con repositorio
    │   JsonUserRepository.GetByEmail(email)
    │       ↓
    │   BCrypt.Verify(password, user.PasswordHash)
    │
    ├─ 2.2. Crear sesión
    │   JsonSessionRepository.Create(new Session { ... })
    │
    └─ 2.3. Desbloquear red
        NetworkService.AllowClient(clientIp)
            ↓
        iptables -A FORWARD -s 192.168.100.50 -j ACCEPT
````

**Dependencias del servicio:**
````csharp
public class AuthenticationService
{
    private readonly JsonUserRepository _userRepository;
    private readonly JsonSessionRepository _sessionRepository;
    private readonly NetworkService _networkService;
    private readonly ILogger<AuthenticationService> _logger;

    public AuthenticationService(
        JsonUserRepository userRepository,
        JsonSessionRepository sessionRepository,
        NetworkService networkService,
        ILogger<AuthenticationService> logger)
    {
        // Inyección de dependencias
    }
}
````

---

## **Integración Global de Todos los Servicios** 🧩

### **Diagrama de Dependencias:**

```
┌─────────────────────────────────────────────────────────────┐
│                    PRESENTATION LAYER                        │
│                                                               │
│  Login.razor → AuthenticationService.LoginAsync()            │
│  Admin.razor → UserManagementService.GetAllUsers()           │
│                                                               │
└───────────────────────────┬─────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────┐
│                   APPLICATION LAYER                          │
│                                                               │
│  ┌──────────────────────┐  ┌──────────────────────┐         │
│  │ AuthenticationService├──► SessionManagementSvc  │         │
│  └──────────┬───────────┘  └──────────────────────┘         │
│             │                                                 │
│             ├──► NetworkService.AllowClient()                │
│             └──► JsonUserRepository.GetByEmail()             │
│                                                               │
└───────────────────────────┬─────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────┐
│                  INFRASTRUCTURE LAYER                        │
│                                                               │
│  ┌─────────────────┐  ┌─────────────────┐                   │
│  │ JsonUserRepo    │  │ NetworkService  │                   │
│  │                 │  │                 │                   │
│  │ - users.json    │  │ - iptables      │                   │
│  └─────────────────┘  └─────────────────┘                   │
│                                                               │
└─────────────────────────────────────────────────────────────┘
```

### **Registro de Servicios en Program.cs:**

````csharp
var builder = WebApplication.CreateBuilder(args);

// ============================================
// INFRASTRUCTURE LAYER (Singleton - una sola instancia)
// ============================================
builder.Services.AddSingleton<NetworkConfigurationService>();
builder.Services.AddSingleton<JsonUserRepository>();
builder.Services.AddSingleton<JsonSessionRepository>();
builder.Services.AddSingleton<JsonDeviceRepository>();

// ============================================
// APPLICATION LAYER (Scoped - una por petición HTTP)
// ============================================
builder.Services.AddScoped<AuthenticationService>();
builder.Services.AddScoped<UserManagementService>();
builder.Services.AddScoped<SessionManagementService>();
builder.Services.AddScoped<NetworkService>();

// ============================================
// HOSTED SERVICES (Background tasks)
// ============================================
builder.Services.AddHostedService<NetworkSetupHostedService>();
builder.Services.AddHostedService<SessionCleanupHostedService>();

// ============================================
// BLAZOR SERVICES
// ============================================
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// ============================================
// MIDDLEWARE PIPELINE
// ============================================
app.UseExceptionHandler("/Error");
app.UseMiddleware<RequestLoggingMiddleware>();  // Loggear peticiones
app.UseRequestRouter();                          // Enrutar según IP
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run("http://0.0.0.0:8080");
````

---

## **Resumen de Responsabilidades**

| Servicio | Capa | Responsabilidad | Ciclo de Vida |
|----------|------|-----------------|---------------|
| **NetworkConfigurationService** | Infrastructure | Configurar sistema operativo (red, iptables) | Singleton |
| **NetworkSetupHostedService** | Infrastructure | Inicializar/limpiar red en app lifecycle | Hosted |
| **RequestRouterMiddleware** | Presentation | Enrutar peticiones (portal vs admin) | Middleware |
| **JsonUserRepository** | Infrastructure | Persistencia de usuarios | Singleton |
| **AuthenticationService** | Application | Lógica de autenticación | Scoped |
| **NetworkService** | Infrastructure | Operaciones dinámicas de red (permitir IPs) | Scoped |
| **SessionManagementService** | Application | Gestionar sesiones activas | Scoped |

---

¿Necesitas que profundice en algún servicio específico o en cómo interactúan en un escenario concreto (ej: flujo completo de login)?---

## **Resumen de Responsabilidades**

| Servicio | Capa | Responsabilidad | Ciclo de Vida |
|----------|------|-----------------|---------------|
| **NetworkConfigurationService** | Infrastructure | Configurar sistema operativo (red, iptables) | Singleton |
| **NetworkSetupHostedService** | Infrastructure | Inicializar/limpiar red en app lifecycle | Hosted |
| **RequestRouterMiddleware** | Presentation | Enrutar peticiones (portal vs admin) | Middleware |
| **JsonUserRepository** | Infrastructure | Persistencia de usuarios | Singleton |
| **AuthenticationService** | Application | Lógica de autenticación | Scoped |
| **NetworkService** | Infrastructure | Operaciones dinámicas de red (permitir IPs) | Scoped |
| **SessionManagementService** | Application | Gestionar sesiones activas | Scoped |

---

¿Necesitas que profundice en algún servicio específico o en cómo interactúan en un escenario concreto (ej: flujo completo de login)?