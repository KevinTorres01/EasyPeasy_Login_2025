using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using EasyPeasy_Login.Application.Services.SessionManagement;
using EasyPeasy_Login.Application.DTOs;

public class HttpServer
{
    private readonly ISessionManagementService _sessionManagementService;
    private TcpListener? _listener;

    // URLs de redirección
    private const string PortalLoginPage = "/portal/login";
    private const string AdminPage = "/admin";

    public HttpServer(ISessionManagementService sessionManagementService)
    {
        _sessionManagementService = sessionManagementService;
    }

    public void Start()
    {
        _listener = new TcpListener(IPAddress.Any, 8080);
        _listener.Start();
        Console.WriteLine("🚀 Servidor escuchando en puerto 8080...");

        Task.Run(async () =>
        {
            while (true)
            {
                var client = await _listener.AcceptTcpClientAsync();
                _ = HandleClientAsync(client); // Fire and forget para manejar múltiples clientes
            }
        });
    }

    private async Task HandleClientAsync(TcpClient client)
    {
        try
        {
            var stream = client.GetStream();
            var buffer = new byte[4096];
            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
            string rawRequest = Encoding.UTF8.GetString(buffer, 0, bytesRead);

            string clientIP = ((IPEndPoint)client.Client.RemoteEndPoint!).Address.ToString();
            var petition = HttpPetition.Parse(rawRequest, clientIP);

            string response;
            if (petition.IsFromLocalhost())
            {
                response = HandleLocalRequest(petition);
            }
            else
            {
                response = await HandleRemoteRequestAsync(petition);
            }

            byte[] responseBytes = Encoding.UTF8.GetBytes(response);
            await stream.WriteAsync(responseBytes, 0, responseBytes.Length);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error manejando cliente: {ex.Message}");
        }
        finally
        {
            client.Close();
        }
    }

    /// <summary>
    /// Maneja peticiones locales (desde el mismo servidor) - Sirve página de admin
    /// </summary>
    private string HandleLocalRequest(HttpPetition petition)
    {
        Console.WriteLine($"🏠 Petición LOCAL: {petition.Method} {petition.Path}");

        // Si ya está en /admin, servir la página
        if (petition.Path.StartsWith("/admin"))
        {
            // TODO: Reemplazar con tu página HTML de admin real
            string adminHtml = @"
<!DOCTYPE html>
<html>
<head>
    <title>Panel de Administración</title>
    <style>
        body { font-family: Arial, sans-serif; margin: 40px; background: #f5f5f5; }
        .container { max-width: 800px; margin: 0 auto; background: white; padding: 30px; border-radius: 10px; box-shadow: 0 2px 10px rgba(0,0,0,0.1); }
        h1 { color: #333; }
        .info { background: #e7f3ff; padding: 15px; border-radius: 5px; margin: 20px 0; }
    </style>
</head>
<body>
    <div class='container'>
        <h1>🔧 Panel de Administración</h1>
        <div class='info'>
            <p><strong>Acceso concedido desde localhost</strong></p>
            <p>Aquí puedes gestionar el Captive Portal.</p>
        </div>
        <ul>
            <li><a href='/admin/users'>Gestión de Usuarios</a></li>
            <li><a href='/admin/sessions'>Sesiones Activas</a></li>
            <li><a href='/admin/devices'>Dispositivos Conectados</a></li>
            <li><a href='/admin/network'>Configuración de Red</a></li>
        </ul>
    </div>
</body>
</html>";
            return BuildHtmlResponse(200, "OK", adminHtml);
        }

        // Redirigir a /admin si no está ahí
        return BuildRedirectResponse(AdminPage);
    }

    /// <summary>
    /// Maneja peticiones remotas - Verifica autenticación por MAC
    /// </summary>
    private async Task<string> HandleRemoteRequestAsync(HttpPetition petition)
    {
        Console.WriteLine($"🌐 Petición REMOTA desde {petition.ClientIP}: {petition.Method} {petition.Path}");

        // 1. Obtener MAC address desde la IP usando tabla ARP
        string? macAddress = await GetMacAddressFromIpAsync(petition.ClientIP);

        if (string.IsNullOrEmpty(macAddress))
        {
            Console.WriteLine($"⚠️ No se pudo obtener MAC para IP: {petition.ClientIP}");
            // Si no podemos obtener MAC, redirigir al portal de login
            return BuildRedirectResponse(PortalLoginPage);
        }

        Console.WriteLine($"🔍 MAC detectada: {macAddress} para IP: {petition.ClientIP}");

        // 2. Verificar si existe sesión activa para esta MAC
        var sessionDto = new SessionDto
        {
            MacAddress = macAddress,
            Username = string.Empty // No necesario para verificación
        };

        bool isAuthenticated = await _sessionManagementService.IsActiveSession(sessionDto);

        if (isAuthenticated)
        {
            // 3a. AUTENTICADO: Permitir acceso a internet
            Console.WriteLine($"✅ Dispositivo {macAddress} AUTENTICADO - Permitiendo tráfico");
            
            // Si está pidiendo el portal pero ya está autenticado, mostrar página de éxito
            if (petition.Path.StartsWith("/portal"))
            {
                return BuildSuccessPage(macAddress);
            }

            // Para otras rutas, permitir el acceso (en un captive portal real, aquí harías proxy o pass-through)
            return BuildAllowedResponse();
        }
        else
        {
            // 3b. NO AUTENTICADO: Redirigir al portal de login
            Console.WriteLine($"🔒 Dispositivo {macAddress} NO AUTENTICADO - Redirigiendo a login");

            // Si ya está en la página de login, servirla
            if (petition.Path.StartsWith("/portal"))
            {
                return BuildLoginPage(macAddress, petition.ClientIP);
            }

            // Redirigir al portal de login
            return BuildRedirectResponse(PortalLoginPage);
        }
    }

    /// <summary>
    /// Obtiene la MAC address desde la tabla ARP del sistema
    /// </summary>
    private async Task<string?> GetMacAddressFromIpAsync(string ipAddress)
    {
        try
        {
            // Hacer ping primero para asegurar que la entrada existe en ARP
            await ExecuteCommandAsync($"ping -c 1 -W 1 {ipAddress}", ignoreErrors: true);

            // Consultar tabla ARP
            string arpOutput = await ExecuteCommandAsync($"ip neigh show {ipAddress}");

            // Buscar patrón MAC: aa:bb:cc:dd:ee:ff
            var macMatch = Regex.Match(arpOutput, @"([0-9A-Fa-f]{2}[:-]){5}([0-9A-Fa-f]{2})");
            
            if (macMatch.Success)
            {
                return macMatch.Value.ToLower();
            }

            // Método alternativo: leer desde DHCP leases
            string leasesPath = "/var/lib/misc/dnsmasq.leases";
            if (File.Exists(leasesPath))
            {
                var leases = await File.ReadAllLinesAsync(leasesPath);
                foreach (var lease in leases)
                {
                    var parts = lease.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length >= 3 && parts[2] == ipAddress)
                    {
                        return parts[1].ToLower();
                    }
                }
            }

            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error obteniendo MAC: {ex.Message}");
            return null;
        }
    }

    private async Task<string> ExecuteCommandAsync(string command, bool ignoreErrors = false)
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

    #region HTTP Response Builders

    private string BuildRedirectResponse(string location)
    {
        string fullUrl = $"http://192.168.100.1:8080{location}";
        return $"HTTP/1.1 302 Found\r\nLocation: {fullUrl}\r\nContent-Length: 0\r\n\r\n";
    }

    private string BuildHtmlResponse(int statusCode, string statusText, string html)
    {
        byte[] bodyBytes = Encoding.UTF8.GetBytes(html);
        return $"HTTP/1.1 {statusCode} {statusText}\r\n" +
               "Content-Type: text/html; charset=utf-8\r\n" +
               $"Content-Length: {bodyBytes.Length}\r\n\r\n{html}";
    }

    private string BuildAllowedResponse()
    {
        string html = @"
<!DOCTYPE html>
<html>
<head><title>Acceso Permitido</title></head>
<body>
    <h1>✅ Acceso a Internet Permitido</h1>
    <p>Tu dispositivo está autenticado. Puedes navegar libremente.</p>
    <script>window.location.href = 'http://www.google.com';</script>
</body>
</html>";
        return BuildHtmlResponse(200, "OK", html);
    }

    private string BuildSuccessPage(string macAddress)
    {
        string html = $@"
<!DOCTYPE html>
<html>
<head>
    <title>Conectado</title>
    <style>
        body {{ font-family: Arial, sans-serif; margin: 40px; background: #f0fff0; text-align: center; }}
        .success {{ background: #d4edda; padding: 30px; border-radius: 10px; max-width: 500px; margin: 50px auto; }}
        h1 {{ color: #28a745; }}
    </style>
</head>
<body>
    <div class='success'>
        <h1>✅ ¡Conectado Exitosamente!</h1>
        <p>Tu dispositivo <code>{macAddress}</code> está autenticado.</p>
        <p>Ya puedes navegar en internet.</p>
        <a href='http://www.google.com'>Ir a Google</a>
    </div>
</body>
</html>";
        return BuildHtmlResponse(200, "OK", html);
    }

    private string BuildLoginPage(string macAddress, string clientIp)
    {
        // TODO: Reemplazar con tu página HTML de login real
        string html = $@"
<!DOCTYPE html>
<html>
<head>
    <title>Portal de Login</title>
    <style>
        body {{ font-family: Arial, sans-serif; margin: 0; background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); min-height: 100vh; display: flex; align-items: center; justify-content: center; }}
        .login-box {{ background: white; padding: 40px; border-radius: 10px; box-shadow: 0 10px 30px rgba(0,0,0,0.3); width: 100%; max-width: 400px; }}
        h1 {{ text-align: center; color: #333; margin-bottom: 30px; }}
        .form-group {{ margin-bottom: 20px; }}
        label {{ display: block; margin-bottom: 5px; font-weight: bold; color: #555; }}
        input[type='text'], input[type='password'] {{ width: 100%; padding: 12px; border: 1px solid #ddd; border-radius: 5px; font-size: 16px; box-sizing: border-box; }}
        button {{ width: 100%; padding: 12px; background: #667eea; color: white; border: none; border-radius: 5px; font-size: 16px; cursor: pointer; }}
        button:hover {{ background: #5a6fd6; }}
        .device-info {{ background: #f8f9fa; padding: 10px; border-radius: 5px; margin-top: 20px; font-size: 12px; color: #666; }}
    </style>
</head>
<body>
    <div class='login-box'>
        <h1>🔒 Captive Portal</h1>
        <form action='/portal/authenticate' method='POST'>
            <input type='hidden' name='mac' value='{macAddress}' />
            <input type='hidden' name='ip' value='{clientIp}' />
            <div class='form-group'>
                <label>Usuario:</label>
                <input type='text' name='username' required placeholder='Ingresa tu usuario' />
            </div>
            <div class='form-group'>
                <label>Contraseña:</label>
                <input type='password' name='password' required placeholder='Ingresa tu contraseña' />
            </div>
            <button type='submit'>Iniciar Sesión</button>
        </form>
        <div class='device-info'>
            <p>🖧 MAC: {macAddress}</p>
            <p>📍 IP: {clientIp}</p>
        </div>
    </div>
</body>
</html>";
        return BuildHtmlResponse(200, "OK", html);
    }

    #endregion
}
