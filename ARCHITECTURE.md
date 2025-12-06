# Arquitectura del Sistema - EasyPeasy Login

## Flujo de Puertos

```
Usuario/Cliente                 Servidor
     |                             |
     |------ Puerto 8080 -------> HttpServer (Captive Portal)
     |                             |
     |                             |- Connectivity Check? → 204 / Redirect
     |                             |- Localhost? → Redirect a :5000
     |                             |- ¿Tiene sesión? → Success / Redirect
     |
     |------ Puerto 5000 -------> ASP.NET Web App (Admin)
                                   |
                                   |- Blazor UI (/admin)
                                   |- API REST (/api/*)
```

## HttpServer (Puerto 8080) - Captive Portal

**Responsabilidades:**
- Detectar connectivity checks de OS (Android, iOS, Windows, Linux)
- Validar sesiones activas por MAC address
- Redirigir usuarios no autenticados al portal de login
- Permitir tráfico a usuarios autenticados

**Flujo de Decisión:**
1. ¿Es connectivity check? → Responder `204` si autenticado, sino redirect
2. ¿Es localhost? → Redirect a admin panel (puerto 5000)
3. ¿Usuario tiene sesión activa?
   - **Sí** → Permitir acceso (204)
   - **No** → Redirect a `/portal/login`

## ASP.NET Web App (Puerto 5000) - Admin Panel

**Responsabilidades:**
- Interfaz de administración (Blazor)
- API REST para gestión de usuarios, sesiones, dispositivos
- Servir páginas del portal de login

**Endpoints:**
- `/admin` - Panel de administración
- `/api/users` - Gestión de usuarios
- `/api/sessions` - Gestión de sesiones
- `/api/devices` - Gestión de dispositivos
- `/portal/login` - Página de login del portal

## Servicios en Background

### CaptivePortalHostedService
- Inicia/detiene el `HttpServer` automáticamente
- Se ejecuta como servicio de fondo de ASP.NET

## Flujo Completo

```
1. Usuario se conecta al WiFi
2. OS detecta captive portal (genera request a puerto 8080)
3. HttpServer intercepta → No tiene sesión → Redirect a portal
4. Usuario ve página de login
5. Usuario ingresa credenciales → API valida
6. Se crea sesión con MAC address
7. Connectivity check → HttpServer ve sesión activa → 204
8. OS abre Internet normalmente
```

## Configuración

**HttpServer:**
- Puerto: 8080
- Gateway IP: 192.168.100.1
- Portal URL: `/portal/login`

**ASP.NET Web:**
- Puerto: 5000
- Admin URL: `http://192.168.100.1:5000/admin`
