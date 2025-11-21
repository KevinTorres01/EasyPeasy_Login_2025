# Explicación Detallada de la Estructura de Archivos

## **Proyecto: PortalCautivo.Web** (Aplicación Principal)

### **Program.cs**
- **Propósito**: Punto de entrada de la aplicación
- **Contenido**:
  - Configuración del builder de la aplicación web
  - Registro de todos los servicios en el contenedor de DI (Dependency Injection)
  - Configuración del pipeline de middleware
  - Configuración de Blazor Server
  - Mapeo de rutas y endpoints
  - Configuración de logging con Serilog
  - Configuración de políticas CORS si es necesario
  - Registro del `CaptivePortalHostedService` para inicialización

### **appsettings.json**
- **Propósito**: Configuración general de la aplicación
- **Contenido**:
  - Configuración de logging (niveles, proveedores)
  - Cadenas de conexión (si aplica)
  - URLs permitidas para CORS
  - Configuración de red (interfaces, rangos IP, gateway)
  - Rutas de archivos de datos JSON
  - Configuración de sesiones (tiempo de expiración)
  - IPs administrativas permitidas
  - Configuración de DHCP (rango, lease time)
  - Configuración DNS (servidores upstream)

### **appsettings.Development.json**
- **Propósito**: Configuración específica para desarrollo
- **Contenido**:
  - Logging más verboso (Debug level)
  - Configuración de red local para pruebas
  - Deshabilitar características de producción
  - URLs de desarrollo
  - Configuración de hot reload

---

## **Pages/Portal/** (Interfaz de Usuario - Clientes)

### **_Host.cshtml**
- **Propósito**: Página host principal para el portal de clientes
- **Contenido**:
  - HTML básico de la página
  - Referencias a CSS específico del portal
  - Tag `<component>` que renderiza el componente Login
  - Scripts de Blazor Server
  - Meta tags para detección de portal cautivo
  - Base href configurado para `/portal/`

### **Login.razor**
- **Propósito**: Componente visual de la página de login
- **Contenido**:
  - Markup HTML/Razor del formulario de login
  - Referencias a componentes hijos (AnimatedAvatar, LoginForm)
  - Estructura visual de la página
  - Mensajes de error/éxito
  - Links a términos y condiciones

### **Login.razor.cs**
- **Propósito**: Code-behind del componente Login
- **Contenido**:
  - Lógica de manejo del evento de login
  - Inyección de dependencias (IAuthenticationService)
  - Estado del componente (loading, error messages)
  - Métodos para validación del lado cliente
  - Navegación post-autenticación
  - Manejo de detección del dispositivo (IP/MAC)

### **Terms.razor**
- **Propósito**: Página de términos y condiciones
- **Contenido**:
  - Markup HTML/Razor con el texto de términos
  - Elementos humorísticos si aplica
  - Botón de aceptar/rechazar
  - Scroll tracking para verificar lectura

### **Terms.razor.cs**
- **Propósito**: Lógica de términos y condiciones
- **Contenido**:
  - Manejo del evento de aceptación
  - Tracking de scroll
  - Navegación de regreso al login
  - Validación de lectura completa

### **Success.razor**
- **Propósito**: Página mostrada después de autenticación exitosa
- **Contenido**:
  - Mensaje de bienvenida
  - Información de la sesión (tiempo restante)
  - Redirección automática al destino original
  - Botón de logout

### **Error.razor**
- **Propósito**: Página de error genérica
- **Contenido**:
  - Mensaje de error amigable
  - Código de error si aplica
  - Botón para regresar al login
  - Información de contacto con admin

---

## **Pages/Admin/** (Interfaz de Administración)

### **_AdminHost.cshtml**
- **Propósito**: Página host para el panel de administración
- **Contenido**:
  - HTML básico de la página admin
  - Referencias a CSS específico del admin
  - Tag `<component>` que renderiza Dashboard
  - Scripts de Blazor Server
  - Base href configurado para `/admin/`

### **Dashboard.razor**
- **Propósito**: Panel principal de administración
- **Contenido**:
  - Layout del dashboard
  - Tarjetas de estadísticas (usuarios activos, dispositivos conectados)
  - Gráficos si aplica
  - Lista de sesiones activas
  - Accesos rápidos a otras secciones

### **Dashboard.razor.cs**
- **Propósito**: Lógica del dashboard
- **Contenido**:
  - Inyección de servicios necesarios
  - Carga de datos estadísticos
  - Actualización en tiempo real (SignalR)
  - Manejo de navegación a otras páginas
  - Métodos para refrescar datos

### **Users/Index.razor**
- **Propósito**: Lista de usuarios registrados
- **Contenido**:
  - Tabla con todos los usuarios
  - Búsqueda y filtrado
  - Botones de acciones (editar, eliminar)
  - Paginación
  - Link para crear nuevo usuario

### **Users/Create.razor**
- **Propósito**: Formulario para crear usuario
- **Contenido**:
  - Campos del formulario (username, password, email, role)
  - Validación en tiempo real
  - Botón de guardar/cancelar
  - Mensajes de error/éxito

### **Users/Edit.razor**
- **Propósito**: Formulario para editar usuario existente
- **Contenido**:
  - Similar a Create pero pre-poblado
  - Opción de cambiar contraseña
  - Activar/desactivar usuario
  - Cambiar rol

### **Users/Delete.razor**
- **Propósito**: Confirmación de eliminación de usuario
- **Contenido**:
  - Información del usuario a eliminar
  - Advertencias sobre la acción
  - Botones de confirmar/cancelar
  - Verificación de dependencias (sesiones activas)

### **Devices/Index.razor**
- **Propósito**: Lista de dispositivos conectados
- **Contenido**:
  - Tabla con dispositivos (IP, MAC, hostname, estado)
  - Filtros (autenticados, no autenticados)
  - Acciones (desconectar, ver historial)
  - Actualización en tiempo real

### **Devices/Details.razor**
- **Propósito**: Detalles de un dispositivo específico
- **Contenido**:
  - Información completa del dispositivo
  - Historial de conexiones
  - Usuario asociado si está autenticado
  - Acción de desconectar/bloquear

### **Settings/Index.razor**
- **Propósito**: Configuración del sistema
- **Contenido**:
  - Configuración de red (interfaces, IPs)
  - Configuración DHCP/DNS
  - Tiempo de expiración de sesiones
  - Opciones de seguridad
  - Botón de reiniciar servicios

---

## **Components/** (Componentes Reutilizables)

### **Shared/MainLayout.razor**
- **Propósito**: Layout principal para el portal de clientes
- **Contenido**:isual Studio y .NET
  - Estructura HTML común (header, footer)
  - @Body para contenido dinámico
  - Sin navegación compleja (portal simple)
  - Estilos base

### **Shared/AdminLayout.razor**
- **Propósito**: Layout para el panel de administración
- **Contenido**:
  - Estructura con sidebar y main content
  - Header con información del admin
  - Menú de navegación lateral
  - @Body para contenido de páginas
  - Breadcrumbs

### **Shared/NavMenu.razor**
- **Propósito**: Menú de navegación del admin
- **Contenido**:
  - Links a Dashboard, Users, Devices, Settings
  - Indicadores visuales de página activa
  - Iconos para cada sección
  - Contador de notificaciones si aplica

### **Shared/LoadingSpinner.razor**
- **Propósito**: Indicador de carga
- **Contenido**:
  - Animación de spinner
  - Texto opcional de carga
  - Overlay si es necesario
  - Parámetros para personalizar apariencia

### **Portal/AnimatedAvatar.razor**
- **Propósito**: Avatar animado en página de login
- **Contenido**:
  - SVG o imagen del avatar
  - Animaciones CSS
  - Parámetros para controlar estado (feliz, triste, escribiendo)
  - Transiciones suaves

### **Portal/LoginForm.razor**
- **Propósito**: Formulario de login reutilizable
- **Contenido**:
  - Campos de usuario y contraseña
  - Validación incorporada
  - Botón de submit
  - Checkbox "recordarme" si aplica
  - Events para OnSubmit

### **Portal/TermsContent.razor**
- **Propósito**: Contenido de términos y condiciones
- **Contenido**:
  - Texto completo de términos
  - Secciones colapsables
  - Elementos humorísticos
  - Puede ser reutilizado en diferentes contextos

### **Admin/UserTable.razor**
- **Propósito**: Tabla de usuarios reutilizable
- **Contenido**:
  - Renderizado de tabla de usuarios
  - Ordenamiento de columnas
  - Acciones por fila
  - Parámetros para datos y callbacks

### **Admin/DeviceTable.razor**
- **Propósito**: Tabla de dispositivos reutilizable
- **Contenido**:
  - Similar a UserTable pero para dispositivos
  - Indicadores de estado (conectado, autenticado)
  - Acciones específicas de dispositivos

### **Admin/StatisticsCard.razor**
- **Propósito**: Tarjeta de estadística en dashboard
- **Contenido**:
  - Layout de tarjeta
  - Título, valor, icono
  - Indicador de cambio (arriba/abajo)
  - Colores configurables

### **Admin/SessionMonitor.razor**
- **Propósito**: Monitor de sesiones activas
- **Contenido**:
  - Lista de sesiones en tiempo real
  - Tiempo restante por sesión
  - Acción de terminar sesión
  - Actualización automática

---

## **Middleware/**

### **RequestRouterMiddleware.cs**
- **Propósito**: Enrutador principal de peticiones
- **Contenido**:
  - Lógica de detección de origen (local vs remoto)
  - Decisión de qué UI mostrar
  - Manejo de endpoints de detección captiva
  - Verificación de autenticación de dispositivos
  - Redirecciones apropiadas

### **ExceptionHandlingMiddleware.cs**
- **Propósito**: Manejo centralizado de excepciones
- **Contenido**:
  - Captura de excepciones no manejadas
  - Logging de errores
  - Generación de respuestas de error apropiadas
  - Diferenciación entre errores de cliente y servidor

### **RequestLoggingMiddleware.cs**
- **Propósito**: Logging de peticiones HTTP
- **Contenido**:
  - Registro de cada petición (IP, URL, método)
  - Tiempo de respuesta
  - Código de estado
  - Información útil para debugging y auditoría

---

## **Controllers/**

### **AuthController.cs**
- **Propósito**: API endpoints para autenticación
- **Contenido**:
  - POST /api/auth/login - Procesar login
  - POST /api/auth/logout - Cerrar sesión
  - GET /api/auth/status - Verificar estado de autenticación
  - Validación de entrada
  - Manejo de errores

### **DetectionController.cs**
- **Propósito**: Endpoints de detección de portal cautivo
- **Contenido**:
  - GET /hotspot-detect.html - iOS/macOS
  - GET /generate_204 - Android
  - GET /connecttest.txt - Windows
  - Respuestas específicas por plataforma
  - Redirecciones apropiadas

### **AdminApiController.cs**
- **Propósito**: API para operaciones administrativas
- **Contenido**:
  - CRUD de usuarios
  - Operaciones de dispositivos
  - Gestión de sesiones
  - Verificación de permisos de admin
  - Respuestas JSON

---

## **HostedServices/**

### **CaptivePortalHostedService.cs**
- **Propósito**: Inicialización y finalización del sistema
- **Contenido**:
  - StartAsync: Configurar red, firewall, DNS al iniciar
  - StopAsync: Restaurar configuración al cerrar
  - Verificación de permisos (sudo)
  - Manejo de errores críticos de inicialización

### **SessionCleanupService.cs**
- **Propósito**: Limpieza periódica de sesiones expiradas
- **Contenido**:
  - Timer que ejecuta cada X minutos
  - Eliminación de sesiones expiradas
  - Revocar acceso de dispositivos de sesiones expiradas
  - Actualizar firewall
  - Logging de operaciones

### **NetworkMonitoringService.cs**
- **Propósito**: Monitoreo continuo de la red
- **Contenido**:
  - Verificación de dispositivos conectados
  - Detección de nuevos dispositivos
  - Actualización de estado (last seen)
  - Verificación de salud de servicios (dnsmasq, iptables)
  - Alertas si algo falla

---

## **wwwroot/** (Archivos Estáticos)

### **css/portal.css**
- **Propósito**: Estilos para el portal de clientes
- **Contenido**:
  - Estilos del formulario de login
  - Animaciones de avatares
  - Diseño responsive
  - Tema de colores del portal
  - Transiciones y efectos

### **css/admin.css**
- **Propósito**: Estilos para el panel de administración
- **Contenido**:
  - Estilos del sidebar y layout
  - Tablas y formularios
  - Dashboard y tarjetas de estadísticas
  - Tema profesional
  - Estilos de botones y acciones

### **css/shared.css**
- **Propósito**: Estilos compartidos
- **Contenido**:
  - Reset CSS
  - Variables CSS (colores, fuentes)
  - Utilidades (margins, paddings, text)
  - Estilos de componentes comunes
  - Responsive breakpoints

### **js/portal.js**
- **Propósito**: JavaScript para el portal
- **Contenido**:
  - Animaciones del avatar
  - Validación del lado cliente
  - Manejo de eventos del formulario
  - Efectos visuales interactivos

### **js/admin.js**
- **Propósito**: JavaScript para el admin
- **Contenido**:
  - Confirmaciones de acciones
  - Manejo de tablas (ordenamiento)
  - Gráficos si aplica
  - Actualización en tiempo real (SignalR)

### **images/avatars/**
- **Propósito**: Imágenes de avatares
- **Contenido**:
  - Diferentes estados del avatar (neutral, feliz, triste, etc.)
  - SVG o PNG
  - Sprites si es animación por frames

### **images/logos/**
- **Propósito**: Logos de la aplicación
- **Contenido**:
  - Logo principal
  - Favicon
  - Logo para diferentes tamaños
  - Logo en modo oscuro si aplica

---

## **Extensions/**

### **ServiceCollectionExtensions.cs**
- **Propósito**: Métodos de extensión para configurar servicios
- **Contenido**:
  - `AddApplicationServices()` - Registrar servicios de Application layer
  - `AddInfrastructureServices()` - Registrar servicios de Infrastructure
  - `AddRepositories()` - Registrar repositorios
  - Configuración agrupada y organizada

### **ApplicationBuilderExtensions.cs**
- **Propósito**: Métodos de extensión para configurar middleware
- **Contenido**:
  - `UseCustomMiddleware()` - Registrar middleware personalizado
  - `UseCaptivePortalRouting()` - Configurar enrutamiento especial
  - Orden correcto de middleware

---

## **Proyecto: PortalCautivo.Application** (Lógica de Negocio)

### **Services/Authentication/IAuthenticationService.cs**
- **Propósito**: Interfaz del servicio de autenticación
- **Contenido**:
  - Firmas de métodos de autenticación
  - Documentación XML de cada método
  - Tipos de retorno y parámetros
  - Excepciones que puede lanzar

### **Services/Authentication/AuthenticationService.cs**
- **Propósito**: Implementación del servicio de autenticación
- **Contenido**:
  - Validación de credenciales contra repositorio
  - Hash de contraseñas
  - Creación de sesión
  - Autorización de dispositivo en firewall
  - Logging de intentos de login
  - Manejo de errores

### **Services/Authentication/AuthResult.cs**
- **Propósito**: Resultado de operación de autenticación
- **Contenido**:
  - Propiedad Success (bool)
  - Mensaje de error si falló
  - Token de sesión si tuvo éxito
  - Información adicional (user, device)

### **Services/UserManagement/IUserManagementService.cs**
- **Propósito**: Interfaz del servicio de gestión de usuarios
- **Contenido**:
  - Firmas de métodos CRUD
  - Métodos de búsqueda y filtrado
  - Validación de usuarios
  - Cambio de contraseña

### **Services/UserManagement/UserManagementService.cs**
- **Propósito**: Implementación de gestión de usuarios
- **Contenido**:
  - Llamadas al repositorio de usuarios
  - Validación de datos (username único, email válido)
  - Hash de contraseñas en creación/actualización
  - Verificación de permisos
  - Logging de operaciones

### **Services/UserManagement/UserValidator.cs**
- **Propósito**: Validador de entidad User
- **Contenido**:
  - Reglas de validación (username no vacío, email válido)
  - Longitud de contraseña
  - Formato de datos
  - Validaciones de negocio

### **Services/DeviceManagement/IDeviceManagementService.cs**
- **Propósito**: Interfaz del servicio de dispositivos
- **Contenido**:
  - Registro de nuevos dispositivos
  - Obtención de dispositivos
  - Actualización de estado
  - Historial de conexiones

### **Services/DeviceManagement/DeviceManagementService.cs**
- **Propósito**: Implementación de gestión de dispositivos
- **Contenido**:
  - Registro automático al detectar dispositivo
  - Asociación IP-MAC
  - Actualización de last seen
  - Obtención de hostname
  - Tracking de autenticación

### **Services/DeviceManagement/DeviceTracker.cs**
- **Propósito**: Tracker de dispositivos en red
- **Contenido**:
  - Escaneo de ARP table
  - Detección de nuevos dispositivos
  - Actualización de estado online/offline
  - Integración con dnsmasq leases

### **Services/SessionManagement/ISessionManagementService.cs**
- **Propósito**: Interfaz del servicio de sesiones
- **Contenido**:
  - Creación de sesiones
  - Verificación de sesión activa
  - Extensión de tiempo
  - Terminación de sesión

### **Services/SessionManagement/SessionManagementService.cs**
- **Propósito**: Implementación de gestión de sesiones
- **Contenido**:
  - Crear sesión asociada a usuario y dispositivo
  - Verificar expiración
  - Renovar tiempo de sesión
  - Terminar sesión y revocar acceso en firewall
  - Limpieza de sesiones expiradas

### **Services/SessionManagement/SessionValidator.cs**
- **Propósito**: Validador de sesiones
- **Contenido**:
  - Verificar que sesión no esté expirada
  - Verificar que dispositivo y usuario existan
  - Validar tiempos de inicio y fin
  - Reglas de negocio de sesiones

### **Services/NetworkControl/INetworkControlService.cs**
- **Propósito**: Interfaz del servicio de control de red
- **Contenido**:
  - Métodos para permitir/bloquear dispositivos
  - Configuración de NAT y forwarding
  - Estado del firewall
  - Restauración de configuración

### **Services/NetworkControl/NetworkControlService.cs**
- **Propósito**: Implementación de control de red
- **Contenido**:
  - Orquestación de IptablesManager y DnsmasqManager
  - Lógica de alto nivel para autorizar dispositivos
  - Verificación de estado de red
  - Manejo de errores de red
  - Logging de cambios de configuración

### **Services/NetworkControl/NetworkValidator.cs**
- **Propósito**: Validador de operaciones de red
- **Contenido**:
  - Validar formato de IP y MAC
  - Verificar que interfaces existan
  - Validar rangos de red
  - Verificar permisos del sistema

### **Services/CaptiveDetection/ICaptivePortalDetectionService.cs**
- **Propósito**: Interfaz del servicio de detección
- **Contenido**:
  - Manejo de peticiones de detección
  - Registro de endpoints
  - Respuestas por tipo de OS

### **Services/CaptiveDetection/CaptivePortalDetectionService.cs**
- **Propósito**: Implementación de detección de portal
- **Contenido**:
  - Generación de respuestas específicas (204, 302, contenido HTML)
  - Detección del tipo de dispositivo/OS
  - Logging de detecciones
  - Personalización de respuestas

### **Services/CaptiveDetection/DetectionEndpoints.cs**
- **Propósito**: Definición de endpoints de detección
- **Contenido**:
  - Constantes para cada endpoint
  - Mapeo de endpoint a OS
  - Tipo de respuesta esperada por endpoint
  - Documentación de cada endpoint

---

## **DTOs/** (Data Transfer Objects)

### **Authentication/LoginRequestDto.cs**
- **Propósito**: DTO para petición de login
- **Contenido**:
  - Propiedades: Username, Password
  - Atributos de validación ([Required], [StringLength])
  - Sin lógica, solo datos

### **Authentication/LoginResponseDto.cs**
- **Propósito**: DTO para respuesta de login
- **Contenido**:
  - Propiedades: Success, Message, SessionToken, UserId
  - Datos que se envían al cliente
  - Sin información sensible

### **Authentication/AuthResultDto.cs**
- **Propósito**: DTO para resultado de autenticación
- **Contenido**:
  - Similar a LoginResponse pero más completo
  - Incluye información de sesión
  - Tiempo de expiración

### **User/UserDto.cs**
- **Propósito**: DTO completo de usuario
- **Contenido**:
  - Todas las propiedades públicas del usuario
  - Sin PasswordHash (seguridad)
  - Información de fechas formateada

### **User/CreateUserDto.cs**
- **Propósito**: DTO para crear usuario
- **Contenido**:
  - Username, Password, Email, Role
  - Validaciones necesarias
  - Sin Id ni fechas (se generan en servidor)

### **User/UpdateUserDto.cs**
- **Propósito**: DTO para actualizar usuario
- **Contenido**:
  - Propiedades opcionales (solo las que se actualizan)
  - Sin password (método separado)
  - Email, Role, IsActive

### **User/UserListDto.cs**
- **Propósito**: DTO ligero para listas de usuarios
- **Contenido**:
  - Solo propiedades esenciales (Id, Username, IsActive)
  - Para mejorar performance en listados
  - Sin información detallada

### **Device/DeviceDto.cs**
- **Propósito**: DTO completo de dispositivo
- **Contenido**:
  - Id, IP, MAC, Hostname, estado, fechas
  - Usuario asociado si aplica
  - Información de sesión activa

### **Device/DeviceInfoDto.cs**
- **Propósito**: DTO con información resumida de dispositivo
- **Contenido**:
  - Datos básicos para mostrar en listas
  - Estado de autenticación
  - Último acceso

### **Device/DeviceHistoryDto.cs**
- **Propósito**: DTO con historial de dispositivo
- **Contenido**:
  - Lista de conexiones pasadas
  - Usuarios que lo han usado
  - Estadísticas de uso

### **Session/SessionDto.cs**
- **Propósito**: DTO completo de sesión
- **Contenido**:
  - Id, DeviceId, UserId, tiempos, estado
  - Información del usuario asociado
  - Información del dispositivo

### **Session/SessionInfoDto.cs**
- **Propósito**: DTO con información de sesión activa
- **Contenido**:
  - Tiempo restante
  - Username
  - Device info
  - Destino original guardado

### **Session/ActiveSessionDto.cs**
- **Propósito**: DTO ligero para sesiones activas
- **Contenido**:
  - Datos esenciales para monitoreo
  - Username, IP, tiempo inicio, tiempo restante
  - Para mostrar en dashboard

---

## **Validators/**

### **UserDtoValidator.cs**
- **Propósito**: Validador para UserDto usando FluentValidation
- **Contenido**:
  - Reglas de validación: Username no vacío, min/max length
  - Email válido y único
  - Role válido
  - Mensajes de error personalizados

### **LoginRequestValidator.cs**
- **Propósito**: Validador para LoginRequestDto
- **Contenido**:
  - Username y Password requeridos
  - Longitud mínima
  - Formato válido
  - Prevención de inyección

### **DeviceDtoValidator.cs**
- **Propósito**: Validador para DeviceDto
- **Contenido**:
  - Formato válido de IP
  - Formato válido de MAC
  - Hostname permitido
  - Verificación de duplicados

---

## **Mappings/**

### **UserMappingProfile.cs**
- **Propósito**: Perfil de AutoMapper para User
- **Contenido**:
  - Mapeo de User a UserDto
  - Mapeo de CreateUserDto a User
  - Mapeo de UpdateUserDto a User
  - Configuraciones especiales (ignorar propiedades, transformaciones)

### **DeviceMappingProfile.cs**
- **Propósito**: Perfil de AutoMapper para Device
- **Contenido**:
  - Mapeos entre Device y sus DTOs
  - Transformaciones de datos
  - Mapeos personalizados

### **SessionMappingProfile.cs**
- **Propósito**: Perfil de AutoMapper para Session
- **Contenido**:
  - Mapeos de Session a DTOs
  - Inclusión de datos relacionados (User, Device)
  - Cálculo de tiempo restante

---

## **Exceptions/**

### **AuthenticationException.cs**
- **Propósito**: Excepción personalizada para errores de autenticación
- **Contenido**:
  - Hereda de Exception
  - Constructores con mensaje y inner exception
  - Código de error específico
  - Información adicional del error

### **UserNotFoundException.cs**
- **Propósito**: Excepción cuando no se encuentra usuario
- **Contenido**:
  - Hereda de Exception
  - Username que se buscó
  - Mensaje descriptivo

### **DeviceNotFoundException.cs**
- **Propósito**: Excepción cuando no se encuentra dispositivo
- **Contenido**:
  - Identificador buscado (IP o MAC)
  - Mensaje descriptivo
  - Tipo de búsqueda realizada

### **SessionExpiredException.cs**
- **Propósito**: Excepción cuando sesión está expirada
- **Contenido**:
  - SessionId afectada
  - Tiempo de expiración
  - Mensaje para el usuario

---

## **Proyecto: PortalCautivo.Domain** (Modelos de Dominio)

### **Entities/User.cs**
- **Propósito**: Entidad de usuario del dominio
- **Contenido**:
  - Propiedades: Id, Username, PasswordHash, Email, CreatedAt, LastLogin, IsActive, Role
  - Métodos de negocio si aplica (ej: CanLogin())
  - Validaciones básicas en setters
  - Sin dependencias externas

### **Entities/Device.cs**
- **Propósito**: Entidad de dispositivo
- **Contenido**:
  - Id, IpAddress, MacAddress, Hostname, FirstSeen, LastSeen, IsAuthenticated, CurrentUserId
  - Métodos: UpdateLastSeen(), Authenticate(), etc.
  - Lógica de dominio

### **Entities/Session.cs**
- **Propósito**: Entidad de sesión
- **Contenido**:
  - Id, DeviceId, UserId, StartTime, ExpiresAt, IsActive, OriginalDestination
  - Métodos: IsExpired(), Extend(), Terminate()
  - Cálculo de tiempo restante
  - Validaciones de negocio

### **Entities/ConnectionHistory.cs**
- **Propósito**: Historial de conexión de dispositivo
- **Contenido**:
  - Id, DeviceId, UserId, ConnectedAt, DisconnectedAt
  - Duración de conexión
  - Resultado (exitoso, error)

### **Entities/FirewallRule.cs**
- **Propósito**: Representación de regla de firewall
- **Contenido**:
  - Id, IpAddress, MacAddress, Action (ACCEPT/DROP)
  - CreatedAt, IsActive
  - Comando iptables generado
  - Prioridad

---

## **ValueObjects/**

### **IpAddress.cs**
- **Propósito**: Value Object para direcciones IP
- **Contenido**:
  - Propiedad Value (string)
  - Validación de formato en constructor
  - Métodos de comparación (equals, hashcode)
  - Conversión a string
  - Inmutable

### **MacAddress.cs**
- **Propósito**: Value Object para direcciones MAC
- **Contenido**:
  - Similar a IpAddress
  - Validación de formato MAC
  - Normalización (mayúsculas, separadores)
  - Inmutable

### **SessionToken.cs**
- **Propósito**: Value Object para tokens de sesión
- **Contenido**:
  - Generación segura de token
  - Validación de formato
  - Expiración asociada
  - Inmutable

### **Credentials.cs**
- **Propósito**: Value Object para credenciales
- **Contenido**:
  - Username y Password
  - Validación básica
  - No almacena, solo para transferencia
  - Inmutable

---

## **Enums/**

### **UserRole.cs**
- **Propósito**: Roles de usuario
- **Contenido**:
  - Guest = 0
  - User = 1
  - Admin = 2
  - Descripción de cada rol

### **DeviceStatus.cs**
- **Propósito**: Estados de dispositivo
- **Contenido**:
  - Disconnected, Connected, Authenticated, Blocked
  - Valores numéricos

### **SessionStatus.cs**
- **Propósito**: Estados de sesión
- **Contenido**:
  - Active, Expired, Terminated, Invalid
  - Transiciones permitidas

### **OSType.cs**
- **Propósito**: Tipos de sistema operativo
- **Contenido**:
  - Unknown, iOS, Android, Windows, macOS, Linux
  - Para detección de portal

---

## **Interfaces/IAuditableEntity.cs**
- **Propósito**: Interfaz para entidades auditables
- **Contenido**:
  - Propiedades: CreatedAt, CreatedBy, ModifiedAt, ModifiedBy
  - Para tracking de cambios

### **Common/BaseEntity.cs**
- **Propósito**: Clase base para entidades
- **Contenido**:
  - Propiedad Id (string o Guid)
  - Implementación de IAuditableEntity
  - Métodos comunes (Equals, GetHashCode)

### **Common/Constants.cs**
- **Propósito**: Constantes del dominio
- **Contenido**:
  - Tiempos por defecto (session timeout)
  - Valores máximos/mínimos
  - Expresiones regulares de validación
  - Mensajes de error estándar

---

## **Proyecto: PortalCautivo.Infrastructure**

### **Persistence/Repositories/IUserRepository.cs**
- **Propósito**: Interfaz del repositorio de usuarios
- **Contenido**:
  - Firma de métodos de acceso a datos
  - CRUD operations
  - Queries específicas
  - Sin implementación

### **Persistence/Repositories/UserRepository.cs**
- **Propósito**: Implementación del repositorio de usuarios
- **Contenido**:
  - Uso de IDataStore para leer/escribir JSON
  - Búsqueda de usuarios
  - Manejo de concurrencia
  - Conversión entre JSON y entidades
  - Manejo de errores de IO

### **Persistence/Repositories/IDeviceRepository.cs**
- **Propósito**: Interfaz del repositorio de dispositivos
- **Contenido**:
  - Métodos de acceso a datos de dispositivos
  - Búsqueda por IP, MAC, ID

### **Persistence/Repositories/DeviceRepository.cs**
- **Propósito**: Implementación del repositorio de dispositivos
- **Contenido**:
  - Similar a UserRepository pero para dispositivos
  - Actualización de last seen
  - Queries optimizadas

### **Persistence/Repositories/ISessionRepository.cs**
- **Propósito**: Interfaz del repositorio de sesiones
- **Contenido**:
  - Métodos para gestionar sesiones
  - Obtención de sesiones activas
  - Limpieza de expiradas

### **Persistence/Repositories/SessionRepository.cs**
- **Propósito**: Implementación del repositorio de sesiones
- **Contenido**:
  - Manejo de sesiones en JSON
  - Verificación de expiración
  - Eliminación en batch

### **Persistence/Repositories/BaseRepository.cs**
- **Propósito**: Clase base para repositorios
- **Contenido**:
  - Funcionalidad común (logging, error handling)
  - Métodos auxiliares
  - Configuración de IDataStore

---

### **Persistence/DataStore/IDataStore.cs**
- **Propósito**: Interfaz para almacenamiento de datos
- **Contenido**:
  - Métodos genéricos de lectura/escritura
  - CRUD operations
  - Manejo de colecciones

### **Persistence/DataStore/JSONDataStore.cs**
- **Propósito**: Implementación de almacenamiento en JSON
- **Contenido**:
  - Lectura y escritura de archivos JSON
  - Serialización/deserialización
  - Lock de archivos para concurrencia
  - Creación de backups
  - Manejo de corrupción de datos

### **Persistence/DataStore/DataStoreOptions.cs**
- **Propósito**: Opciones de configuración del data store
- **Contenido**:
  - Ruta de almacenamiento
  - Configuración de backup
  - Opciones de serialización
  - Timeout de lock

---

### **Network/Firewall/IFirewallManager.cs**
- **Propósito**: Interfaz para gestión de firewall
- **Contenido**:
  - Métodos para agregar/eliminar reglas
  - Control de NAT y forwarding
  - Obtención de estado

### **Network/Firewall/IptablesManager.cs**
- **Propósito**: Implementación de gestión de iptables
- **Contenido**:
  - Ejecución de comandos iptables
  - Construcción de reglas
  - Verificación de reglas existentes
  - Restauración de configuración
  - Manejo de errores de ejecución
  - Logging de cambios

### **Network/Firewall/FirewallRule.cs**
- **Propósito**: Representación de regla de firewall
- **Contenido**:
  - Propiedades de la regla
  - Generación del comando iptables
  - Comparación de reglas
  - Serialización

### **Network/Firewall/FirewallCommandBuilder.cs**
- **Propósito**: Constructor de comandos iptables
- **Contenido**:
  - Métodos fluent para construir comandos
  - Validación de parámetros
  - Generación de comandos complejos
  - Templates de comandos comunes

---

### **Network/DNS/IDnsManager.cs**
- **Propósito**: Interfaz para gestión de DNS/DHCP
- **Contenido**:
  - Iniciar/detener dnsmasq
  - Configurar DHCP
  - Configurar DNS
  - Obtener leases

### **Network/DNS/DnsmasqManager.cs**
- **Propósito**: Implementación de gestión de dnsmasq
- **Contenido**:
  - Generación de archivo de configuración
  - Inicio/detención del proceso
  - Lectura de leases
  - Recarga de configuración
  - Verificación de estado del servicio

### **Network/DNS/DHCPConfig.cs**
- **Propósito**: Configuración de DHCP
- **Contenido**:
  - Rango de IPs
  - Gateway
  - DNS servers
  - Lease time
  - Opciones adicionales

### **Network/DNS/DNSConfig.cs**
- **Propósito**: Configuración de DNS
- **Contenido**:
  - Servidores upstream
  - Redirecciones
  - Cache settings
  - Address overrides

### **Network/DNS/DnsmasqConfigGenerator.cs**
- **Propósito**: Generador de archivo de configuración
- **Contenido**:
  - Construcción del archivo .conf
  - Templates de configuración
  - Validación de opciones
  - Documentación inline en config

---

### **Network/NetworkInterfaces/INetworkInterfaceManager.cs**
- **Propósito**: Interfaz para gestión de interfaces de red
- **Contenido**:
  - Listar interfaces
  - Configurar IP estática
  - Habilitar/deshabilitar interfaces

### **Network/NetworkInterfaces/NetworkInterfaceManager.cs**
- **Propósito**: Implementación de gestión de interfaces
- **Contenido**:
  - Ejecución de comandos ip/ifconfig
  - Lectura de configuración actual
  - Aplicación de cambios
  - Verificación de cambios

### **Network/NetworkInterfaces/InterfaceConfiguration.cs**
- **Propósito**: Configuración de interfaz de red
- **Contenido**:
  - Nombre de interfaz
  - IP address
  - Netmask
  - Estado (up/down)
  - MTU

---

### **System/ProcessExecutor/IProcessExecutor.cs**
- **Propósito**: Interfaz para ejecución de procesos
- **Contenido**:
  - Ejecutar comando
  - Ejecutar con sudo
  - Ejecución asíncrona

### **System/ProcessExecutor/ProcessExecutor.cs**
- **Propósito**: Implementación de ejecución de procesos
- **Contenido**:
  - Uso de Process class
  - Captura de stdout/stderr
  - Verificación de exit code
  - Timeout de ejecución
  - Logging de comandos ejecutados
  - Sanitización de parámetros

### **System/ProcessExecutor/ExecutionResult.cs**
- **Propósito**: Resultado de ejecución de proceso
- **Contenido**:
  - ExitCode
  - StandardOutput
  - StandardError
  - Success (bool)
  - ExecutionTime

---

### **System/FileSystem/IFileSystemService.cs**
- **Propósito**: Interfaz para operaciones de sistema de archivos
- **Contenido**:
  - Lectura/escritura de archivos
  - Creación de directorios
  - Verificación de existencia

### **System/FileSystem/FileSystemService.cs**
- **Propósito**: Implementación de servicio de archivos
- **Contenido**:
  - Wrapper sobre File/Directory classes
  - Manejo de permisos
  - Operaciones seguras
  - Logging

---

### **Security/Cryptography/IPasswordHasher.cs**
- **Propósito**: Interfaz para hashing de contraseñas
- **Contenido**:
  - Hash de password
  - Verificación de password
  - Generación de salt

### **Security/Cryptography/PasswordHasher.cs**
- **Propósito**: Implementación de hashing
- **Contenido**:
  - Uso de BCrypt
  - Configuración de work factor
  - Comparación segura
  - Validación de hash

### **Security/Cryptography/HashingOptions.cs**
- **Propósito**: Opciones de hashing
- **Contenido**:
  - Work factor
  - Salt size
  - Algoritmo usado

---

### **Security/TokenGeneration/ITokenGenerator.cs**
- **Propósito**: Interfaz para generación de tokens
- **Contenido**:
  - Generar token aleatorio
  - Validar formato de token

### **Security/TokenGeneration/TokenGenerator.cs**
- **Propósito**: Implementación de generación de tokens
- **Contenido**:
  - Uso de RNGCryptoServiceProvider
  - Generación segura
  - Codificación Base64
  - Longitud configurable

---

### **Extensions/InfrastructureServiceExtensions.cs**
- **Propósito**: Métodos de extensión para registrar servicios de infraestructura
- **Contenido**:
  - AddInfrastructure(IServiceCollection)
  - Configuración de opciones
  - Registro de implementaciones
  - Validación de configuración

---

## **Proyecto: PortalCautivo.Shared**

### **Constants/ApiRoutes.cs**
- **Propósito**: Constantes de rutas de API
- **Contenido**:
  - Rutas como constantes (ej: const string Login = "/api/auth/login")
  - Organizado por controlador
  - Fácil refactorización

### **Constants/ConfigurationKeys.cs**
- **Propósito**: Keys de configuración
- **Contenido**:
  - Nombres de secciones en appsettings
  - Keys de configuración específicas
  - Valores por defecto

### **Constants/ErrorMessages.cs**
- **Propósito**: Mensajes de error centralizados
- **Contenido**:
  - Mensajes de error estándar
  - Plantillas de mensajes
  - Organizado por categoría

### **Constants/NetworkConstants.cs**
- **Propósito**: Constantes de red
- **Contenido**:
  - Puertos por defecto
  - Timeouts de red
  - Tamaños de buffer
  - Nombres de interfaces comunes

---

### **Helpers/NetworkHelper.cs**
- **Propósito**: Funciones auxiliares de red
- **Contenido**:
  - Validar formato de IP
  - Validar formato de MAC
  - Convertir notaciones
  - Obtener IP local

### **Helpers/ValidationHelper.cs**
- **Propósito**: Funciones de validación
- **Contenido**:
  - Validaciones comunes
  - Expresiones regulares compartidas
  - Sanitización de input

### **Helpers/StringHelper.cs**
- **Propósito**: Funciones auxiliares de strings
- **Contenido**:
  - Generación de IDs
  - Formateo de strings
  - Conversiones

---

### **Extensions/StringExtensions.cs**
- **Propósito**: Métodos de extensión para strings
- **Contenido**:
  - IsNullOrWhiteSpace check
  - Truncate
  - ToTitleCase
  - RemoveSpecialCharacters

### **Extensions/DateTimeExtensions.cs**
- **Propósito**: Métodos de extensión para DateTime
- **Contenido**:
  - ToUnixTimestamp
  - FromUnixTimestamp
  - IsExpired(DateTime expirationDate)
  - TimeUntilExpiration

### **Extensions/CollectionExtensions.cs**
- **Propósito**: Métodos de extensión para colecciones
- **Contenido**:
  - IsNullOrEmpty
  - ForEach
  - Distinct by property
  - Chunk

---

## **Proyectos de Tests**

### **Tests/UnitTests/Application/Services/AuthenticationServiceTests.cs**
- **Propósito**: Tests unitarios del servicio de autenticación
- **Contenido**:
  - Test de validación de credenciales correctas
  - Test de credenciales incorrectas
  - Test de usuario inexistente
  - Test de creación de sesión
  - Mocks de repositorios
  - Verificación de llamadas a dependencias

### **Tests/UnitTests/Application/Services/UserManagementServiceTests.cs**
- **Propósito**: Tests del servicio de usuarios
- **Contenido**:
  - CRUD completo
  - Validaciones
  - Casos edge
  - Manejo de errores

### **Tests/UnitTests/Domain/Entities/UserTests.cs**
- **Propósito**: Tests de la entidad User
- **Contenido**:
  - Validaciones de negocio
  - Métodos de la entidad
  - Estado válido/inválido

### **Tests/UnitTests/Infrastructure/Repositories/UserRepositoryTests.cs**
- **Propósito**: Tests del repositorio de usuarios
- **Contenido**:
  - Lectura/escritura de datos
  - Búsquedas
  - Manejo de datos corruptos
  - Mock de IDataStore

### **Tests/IntegrationTests/API/AuthControllerTests.cs**
- **Propósito**: Tests de integración del controlador de auth
- **Contenido**:
  - Peticiones HTTP reales
  - Verificación de respuestas
  - Tests end-to-end de flujo de auth
  - WebApplicationFactory

### **Tests/E2ETests/Portal/LoginFlowTests.cs**
- **Propósito**: Tests E2E del flujo de login
- **Contenido**:
  - Navegación del usuario
  - Interacción con UI
  - Verificación de redirecciones
  - Selenium/Playwright

---

## **Scripts**

### **scripts/setup.sh**
- **Propósito**: Script de instalación inicial
- **Contenido**:
  - Verificación de SO compatible
  - Instalación de dependencias
  - Configuración inicial
  - Creación de directorios
  - Asignación de permisos

### **scripts/install-dependencies.sh**
- **Propósito**: Instalar dependencias del sistema
- **Contenido**:
  - apt-get install iptables dnsmasq
  - Verificación de instalación
  - Configuración de arranque automático

### **scripts/create-certificates.sh**
- **Propósito**: Generar certificados SSL
- **Contenido**:
  - Generación con OpenSSL
  - Certificados autofirmados para desarrollo
  - Opción para Let's Encrypt en producción

### **scripts/configure-network.sh**
- **Propósito**: Configuración de red inicial
- **Contenido**:
  - Habilitar IP forwarding
  - Configurar interfaces
  - Backup de configuración actual

### **scripts/cleanup.sh**
- **Propósito**: Limpieza del sistema
- **Contenido**:
  - Restaurar reglas de firewall
  - Detener servicios
  - Eliminar configuración temporal
  - Restaurar configuración de red

### **scripts/run-dev.sh**
- **Propósito**: Ejecutar en modo desarrollo
- **Contenido**:
  - Compilar la aplicación
  - Configurar red de prueba
  - Ejecutar con hot reload
  - Logging verboso

---

## **Config**

### **config/dnsmasq/dnsmasq.conf.template**
- **Propósito**: Template de configuración de dnsmasq
- **Contenido**:
  - Placeholders para valores dinámicos
  - Comentarios explicativos
  - Configuración DHCP base
  - Configuración DNS base

### **config/iptables/rules.v4.template**
- **Propósito**: Template de reglas de iptables
- **Contenido**:
  - Reglas base del firewall
  - NAT configuration
  - Redirecciones
  - Placeholders para valores dinámicos

---

## **Data** (Generado en Runtime)

### **data/users.json**
- **Propósito**: Almacenamiento de usuarios
- **Contenido**: Array JSON de objetos User serializados

### **data/devices.json**
- **Propósito**: Almacenamiento de dispositivos
- **Contenido**: Array JSON de objetos Device

### **data/sessions.json**
- **Propósito**: Almacenamiento de sesiones
- **Contenido**: Array JSON de objetos Session

### **data/logs/**
- **Propósito**: Logs de la aplicación
- **Contenido**: Archivos de log rotados por fecha

---

Esta estructura proporciona una organización clara y profesional del proyecto, con separación de responsabilidades y fácil mantenimiento.