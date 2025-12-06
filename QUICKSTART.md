# 🚀 EasyPeasy Login - Guía de Inicio Rápido

## Requisitos Previos

- **.NET 10.0 SDK** (o superior)
- **Linux/macOS** o **Windows**
- **Acceso a puertos 8080 y 5000**
- **Permisos de administrador** (para configurar red)

## Instalación Rápida

### Linux/macOS

```bash
./run.sh
```

### Windows

```cmd
run.bat
```

## Descripción de Puertos

| Puerto | Servicio | Descripción |
|--------|----------|-------------|
| **8080** | HttpServer | Captive Portal (Socket raw) |
| **5000** | ASP.NET Web | Admin Panel & API |

## Acceso a la Aplicación

### Captive Portal (Puerto 8080)
- **URL**: `http://192.168.100.1:8080`
- **Login**: `http://192.168.100.1:8080/portal/login`
- **Success**: `http://192.168.100.1:8080/portal/success`
- **Usuario default**: `admin` / `admin05`

### Admin Panel (Puerto 5000)
- **URL**: `http://192.168.100.1:5000/admin`
- **API REST**: `http://192.168.100.1:5000/api/*`
- **Usuario default**: `admin` / `admin05`

## Arquitectura

```
┌─────────────────────────────────────────┐
│         Usuario WiFi (Cliente)          │
└──────────────────┬──────────────────────┘
                   │
        ┌──────────┴──────────┐
        │                     │
   Puerto 8080          Puerto 5000
        │                     │
┌───────▼──────────┐  ┌──────▼────────────┐
│   HttpServer     │  │  ASP.NET Web App  │
│ (Captive Portal) │  │  (Admin & API)    │
│                  │  │                   │
│ • Connectivity   │  │ • Blazor UI       │
│   Checks         │  │ • Controllers     │
│ • Session Valid. │  │ • Services        │
│ • Login Form     │  │ • Database        │
│ • Success Page   │  │                   │
└──────────────────┘  └───────────────────┘
```

## Flujo de Autenticación

```
1. Usuario se conecta al WiFi
                ↓
2. OS detecta captive portal (request a :8080)
                ↓
3. HttpServer intercepta → ¿Sesión activa?
                ↓
        [NO] → Redirect a /portal/login
        [SÍ] → 204 No Content (permitir tráfico)
                ↓
4. Usuario ve formulario de login HTML
                ↓
5. POST /portal/login (usuario + contraseña)
                ↓
6. HttpServer valida y crea sesión
                ↓
7. 302 Redirect a /portal/success
                ↓
8. Connectivity check (nueva request)
                ↓
9. HttpServer → Sesión activa → 204
                ↓
10. Internet abierto ✅
```

## Gestión de Usuarios

### Crear Usuario
```bash
curl -X POST http://192.168.100.1:5000/api/users \
  -H "Content-Type: application/json" \
  -d '{"username":"nuevo_user","password":"password123"}'
```

### Ver Usuarios
```bash
curl http://192.168.100.1:5000/api/users
```

### Ver Sesiones
```bash
curl http://192.168.100.1:5000/api/sessions
```

## Solución de Problemas

### El HttpServer no inicia
- Verifica que el puerto 8080 no esté en uso: `sudo lsof -i :8080`
- Requiere permisos de administrador para conectarse a la red

### El ASP.NET no inicia
- Verifica que el puerto 5000 no esté en uso: `sudo lsof -i :5000`
- Intenta con `dotnet run --urls="http://0.0.0.0:5000"`

### No se detecta la MAC del dispositivo
- En Linux: Asegúrate de tener `ip` command disponible
- Verifica con: `ip neigh show`

## Variables de Entorno

```bash
# Cambiar puertos (opcional)
export HTTP_SERVER_PORT=8080
export ASPNET_PORT=5000
export GATEWAY_IP=192.168.100.1

# Habilitar modo desarrollo
export ASPNETCORE_ENVIRONMENT=Development
```

## Desarrollo

### Build sin correr
```bash
dotnet build
```

### Test
```bash
dotnet test
```

### Publicar
```bash
dotnet publish -c Release -o ./publish
```

## Configuración de Red

### Linux - Configurar como gateway
```bash
# 1. Habilitar IP forwarding
sudo sysctl -w net.ipv4.ip_forward=1

# 2. Configurar DHCP (dnsmasq)
sudo apt-get install dnsmasq
sudo systemctl start dnsmasq

# 3. Configurar firewall (iptables)
sudo iptables -t nat -A POSTROUTING -o eth0 -j MASQUERADE
```

### Windows - Configurar como gateway
```cmd
# Habilitar IP forwarding
reg add HKLM\SYSTEM\CurrentControlSet\Services\Tcpip\Parameters /v IPEnableRouter /t REG_DWORD /d 1 /f

# Reiniciar para aplicar cambios
ipconfig /all
```

## Logs

Los logs se guardan en:
- **Linux/macOS**: `~/.easypeasy/logs/`
- **Windows**: `%APPDATA%\EasyPeasy\logs\`

## Contacto & Soporte

Para reportar bugs o solicitar features: [GitHub Issues](https://github.com/KevinTorres01/EasyPeasy_Login_2025/issues)

---

**Versión**: 1.0.0  
**Última actualización**: Diciembre 2025
