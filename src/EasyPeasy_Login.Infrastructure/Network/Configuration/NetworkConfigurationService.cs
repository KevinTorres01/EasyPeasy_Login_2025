using System.Diagnostics;
using System.Text.RegularExpressions;
using EasyPeasy_Login.Shared.Constants;

namespace EasyPeasy_Login.Infrastructure.Network.Configuration;

public class NetworkConfigurationService : INetworkConfigurationService
{
    private readonly string _interface = NetworkConstants.Interface;
    private readonly string _gatewayIp = NetworkConstants.GatewayIp;
    private readonly string _dhcpRange = NetworkConstants.DhcpRange;
    private readonly string _ssid = NetworkConstants.Ssid;
    private readonly string _password = NetworkConstants.Password;
    public const int DefaultPort = NetworkConstants.DefaultPort;
    public const int MaxConnections = NetworkConstants.MaxConnections;
    public const int TimeoutMilliseconds = NetworkConstants.TimeoutMilliseconds;
    private string? _upstreamInterface;
    private bool _isVpnInterface = false;
    private string? _originalIptablesRules;
    private string? _originalIpForwarding;

    public async Task<bool> SetupNetwork()
    {
        try
        {
            Console.WriteLine("🔧 Iniciando configuración de Access Point...");

            // Guardar configuración actual
            await BackupCurrentConfiguration();

            // 1. Detectar interfaz con Internet (MEJORADO)
            _upstreamInterface = await DetectUpstreamInterface();

            if (string.IsNullOrEmpty(_upstreamInterface))
            {
                Console.WriteLine("⚠️ ADVERTENCIA: No se detectó interfaz con Internet.");
                Console.WriteLine("💡 Conecta tu móvil por USB o ethernet antes de continuar.");
                Console.Write("¿Deseas continuar sin Internet? (s/n): ");
                var response = Console.ReadLine()?.ToLower();
                if (response != "s")
                {
                    await RestoreConfiguration();
                    return false;
                }
            }
            else
            {
                Console.WriteLine($"✅ Interfaz con Internet detectada: {_upstreamInterface}");

                // NUEVO: Indicar si es VPN
                if (_isVpnInterface)
                {
                    Console.WriteLine("⚠️ Esta es una interfaz VPN. Se aplicará configuración especial.");
                }

                Console.Write($"¿Es correcta esta interfaz? (s/n): ");
                var confirm = Console.ReadLine()?.ToLower();

                if (confirm != "s")
                {
                    Console.Write("Ingresa el nombre de la interfaz manualmente (ej: usb0, tun0, enp0s20f0u1): ");
                    var manualInterface = Console.ReadLine()?.Trim();
                    if (!string.IsNullOrEmpty(manualInterface))
                    {
                        _upstreamInterface = manualInterface;
                        _isVpnInterface = manualInterface.StartsWith("tun") || manualInterface.StartsWith("tap");
                        Console.WriteLine($"✅ Usando interfaz manual: {_upstreamInterface}");
                    }
                }
            }

            // 2. Desbloquear WiFi (RF-kill) ANTES de cualquier otra cosa
            Console.WriteLine("🔧 Verificando bloqueo RF-kill...");
            await UnblockRfkill();

            // 3. Detener NetworkManager SOLO en la interfaz WiFi
            Console.WriteLine("🔧 Configurando NetworkManager...");
            await ExecuteCommand($"nmcli device set {_interface} managed no", ignoreErrors: true);
            await Task.Delay(1000);

            // 4. Limpiar configuración anterior de la interfaz
            await ExecuteCommand($"ip addr flush dev {_interface}", ignoreErrors: true);
            await ExecuteCommand($"ip link set {_interface} down", ignoreErrors: true);
            await Task.Delay(500);

            // 5. Configurar interfaz de red
            await ExecuteCommand($"ip link set {_interface} up");
            await ExecuteCommand($"ip addr add {_gatewayIp}/24 dev {_interface}");
            Console.WriteLine($"✅ Interfaz {_interface} configurada con IP {_gatewayIp}");

            // 6. Crear Access Point WiFi
            await CreateAccessPoint();

            // 7. Activar IP forwarding
            await ExecuteCommand("sysctl -w net.ipv4.ip_forward=1");
            Console.WriteLine("✅ IP Forwarding activado");

            // 8. Configurar NAT (solo si hay Internet)
            if (!string.IsNullOrEmpty(_upstreamInterface))
            {
                await ConfigureNAT();
            }

            // 9. Configurar DHCP con dnsmasq
            await ConfigureDnsmasq();

            Console.WriteLine("\n========================================");
            Console.WriteLine("✅ Access Point WiFi creado exitosamente");
            Console.WriteLine($"📡 SSID: {_ssid}");
            Console.WriteLine($"🔑 Password: {_password}");
            Console.WriteLine($"🌐 Gateway: {_gatewayIp}");
            Console.WriteLine($"🌐 Rango DHCP: {_dhcpRange}");
            if (!string.IsNullOrEmpty(_upstreamInterface))
            {
                Console.WriteLine($"🌍 Internet compartido desde: {_upstreamInterface}");
                if (_isVpnInterface)
                {
                    Console.WriteLine($"🔒 Tráfico enrutado a través de VPN");
                }
            }
            Console.WriteLine("========================================\n");

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error configurando red: {ex.Message}");
            Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            await RestoreConfiguration();
            return false;
        }
    }

    private async Task UnblockRfkill()
    {
        try
        {
            // Verificar estado de RF-kill
            var rfkillStatus = await ExecuteCommand("rfkill list", ignoreErrors: true);
            Console.WriteLine($"Estado RF-kill:\n{rfkillStatus}");

            // Desbloquear WiFi
            await ExecuteCommand("rfkill unblock wifi", ignoreErrors: true);
            await ExecuteCommand("rfkill unblock all", ignoreErrors: true);

            await Task.Delay(1000);

            // Verificar que se desbloqueó
            rfkillStatus = await ExecuteCommand("rfkill list", ignoreErrors: true);

            if (rfkillStatus.Contains("Soft blocked: yes") || rfkillStatus.Contains("Hard blocked: yes"))
            {
                Console.WriteLine("⚠️ ADVERTENCIA: WiFi aún bloqueado por RF-kill");
                Console.WriteLine("💡 Si tienes un botón físico de WiFi, actívalo manualmente");
                Console.Write("¿Continuar de todas formas? (s/n): ");
                var response = Console.ReadLine()?.ToLower();
                if (response != "s")
                {
                    await RestoreConfiguration();
                    throw new Exception("WiFi bloqueado por RF-kill");
                }
            }
            else
            {
                Console.WriteLine("✅ RF-kill desbloqueado correctamente");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️ Error verificando RF-kill: {ex.Message}");
            // Continuar de todas formas
        }
    }

    private async Task<string?> DetectUpstreamInterface()
    {
        Console.WriteLine("🔍 Buscando interfaz con acceso a Internet...");

        try
        {
            // Listar todas las interfaces disponibles con información
            var interfacesOutput = await ExecuteCommand("ip -o link show", ignoreErrors: true);
            Console.WriteLine("\n📋 Interfaces de red disponibles:");

            var availableInterfaces = new List<string>();
            foreach (var line in interfacesOutput.Split('\n'))
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                var match = Regex.Match(line, @"^\d+:\s+([^:@]+)");
                if (match.Success)
                {
                    var ifaceName = match.Groups[1].Value.Trim();
                    // Filtrar interfaces no deseadas
                    if (ifaceName != "lo" && ifaceName != _interface &&
                        !ifaceName.StartsWith("veth") &&
                        !ifaceName.StartsWith("docker") &&
                        !ifaceName.StartsWith("br-"))
                    {
                        availableInterfaces.Add(ifaceName);

                        // Obtener estado de la interfaz
                        var addrInfo = await ExecuteCommand($"ip -o addr show {ifaceName}", ignoreErrors: true);
                        var hasIp = addrInfo.Contains("inet ");
                        var state = line.Contains("state UP") ? "UP" : "DOWN";
                        var isVpn = ifaceName.StartsWith("tun") || ifaceName.StartsWith("tap");

                        Console.WriteLine($"  - {ifaceName} [{state}] {(hasIp ? "✅ Con IP" : "❌ Sin IP")}{(isVpn ? " 🔒 VPN" : "")}");
                    }
                }
            }

            // Obtener la ruta por defecto
            var routeOutput = await ExecuteCommand("ip route show default", ignoreErrors: true);

            if (!string.IsNullOrWhiteSpace(routeOutput))
            {
                // Extraer interfaz de la ruta por defecto
                var match = Regex.Match(routeOutput, @"dev\s+(\S+)");
                if (match.Success)
                {
                    var upstreamIface = match.Groups[1].Value;

                    // CAMBIO: Permitir VPN si no hay alternativa física
                    if (!upstreamIface.StartsWith("tun") &&
                        !upstreamIface.StartsWith("tap") &&
                        upstreamIface != _interface)
                    {
                        return upstreamIface;
                    }
                    else if (upstreamIface.StartsWith("tun") || upstreamIface.StartsWith("tap"))
                    {
                        Console.WriteLine($"⚠️ Se detectó VPN ({upstreamIface}), buscando interfaz física...");

                        // Buscar interfaces físicas con IP (USB, Ethernet)
                        foreach (var iface in availableInterfaces)
                        {
                            if (iface.StartsWith("usb") ||
                                iface.StartsWith("enp") ||
                                iface.StartsWith("eth") ||
                                iface.StartsWith("enx"))
                            {
                                var addrInfo = await ExecuteCommand($"ip -o addr show {iface}", ignoreErrors: true);
                                if (addrInfo.Contains("inet ") && addrInfo.Contains("state UP"))
                                {
                                    Console.WriteLine($"💡 Interfaz física encontrada: {iface}");
                                    return iface;
                                }
                            }
                        }

                        // NUEVO: Si no hay física, usar VPN
                        Console.WriteLine($"💡 No se encontró interfaz física, usando VPN: {upstreamIface}");
                        Console.Write("¿Deseas usar la VPN para compartir Internet? (s/n): ");
                        var useVpn = Console.ReadLine()?.ToLower();
                        if (useVpn == "s")
                        {
                            _isVpnInterface = true;
                            return upstreamIface;
                        }
                        await RestoreConfiguration();
                    }
                }
            }

            // Si no hay ruta por defecto, buscar interfaces activas con IP
            foreach (var iface in availableInterfaces)
            {
                var addrInfo = await ExecuteCommand($"ip -o addr show {iface}", ignoreErrors: true);
                if (addrInfo.Contains("inet ") && !addrInfo.Contains("127.0.0.1") && addrInfo.Contains("state UP"))
                {
                    Console.WriteLine($"⚠️ No hay ruta por defecto, usando interfaz activa: {iface}");
                    return iface;
                }
            }

            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️ Error detectando interfaz upstream: {ex.Message}");
            return null;
        }
    }

    private async Task ConfigureNAT()
    {
        Console.WriteLine($"🔧 Configurando NAT para compartir Internet desde {_upstreamInterface}...");

        await ExecuteCommand("iptables -F", ignoreErrors: true);
        await ExecuteCommand("iptables -t nat -F", ignoreErrors: true);
        await ExecuteCommand("iptables -X", ignoreErrors: true);

        // Reglas básicas de FORWARD
        await ExecuteCommand($"iptables -A FORWARD -i {_interface} -o {_upstreamInterface} -j ACCEPT");
        await ExecuteCommand($"iptables -A FORWARD -i {_upstreamInterface} -o {_interface} -m state --state RELATED,ESTABLISHED -j ACCEPT");

        // NAT/MASQUERADE
        await ExecuteCommand($"iptables -t nat -A POSTROUTING -o {_upstreamInterface} -j MASQUERADE");

        // NUEVO: Configuración especial para VPN
        if (_isVpnInterface)
        {
            Console.WriteLine("🔒 Aplicando configuración especial para VPN...");

            // Permitir todo el tráfico desde la interfaz WiFi
            await ExecuteCommand($"iptables -A INPUT -i {_interface} -j ACCEPT", ignoreErrors: true);
            await ExecuteCommand($"iptables -A OUTPUT -o {_interface} -j ACCEPT", ignoreErrors: true);

            // Desactivar MSS clamping (común en VPNs)
            await ExecuteCommand($"iptables -t mangle -A FORWARD -p tcp --tcp-flags SYN,RST SYN -j TCPMSS --clamp-mss-to-pmtu", ignoreErrors: true);

            // Permitir respuestas ICMP
            await ExecuteCommand($"iptables -A FORWARD -p icmp -j ACCEPT", ignoreErrors: true);

            Console.WriteLine("✅ Configuración VPN aplicada");
        }

        Console.WriteLine("✅ NAT configurado");
    }

    private async Task CreateAccessPoint()
    {
        Console.WriteLine("🔧 Creando Access Point con hostapd...");

        // Detener hostapd anterior si existe
        await ExecuteCommand("killall hostapd", ignoreErrors: true);
        await Task.Delay(1000);

        // Crear archivo de configuración hostapd
        var hostapdConfig = $@"interface={_interface}
driver=nl80211
ssid={_ssid}
hw_mode=g
channel=6
ieee80211n=1
wmm_enabled=1
macaddr_acl=0
auth_algs=1
ignore_broadcast_ssid=0
wpa=2
wpa_passphrase={_password}
wpa_key_mgmt=WPA-PSK
wpa_pairwise=CCMP
rsn_pairwise=CCMP
";
        await File.WriteAllTextAsync("/etc/hostapd/hostapd.conf", hostapdConfig);

        // Iniciar hostapd en segundo plano
        var result = await ExecuteCommandWithOutput("hostapd /etc/hostapd/hostapd.conf -B");
        Console.WriteLine($"Hostapd output: {result}");

        // Esperar a que hostapd inicialice
        await Task.Delay(3000);

        // Verificar que hostapd está corriendo
        var hostapdStatus = await ExecuteCommand("pgrep hostapd", ignoreErrors: true);
        if (string.IsNullOrWhiteSpace(hostapdStatus))
        {
            throw new Exception("Hostapd no se inició correctamente. Ejecuta 'sudo hostapd /etc/hostapd/hostapd.conf' manualmente para ver errores.");
        }

        Console.WriteLine("✅ Hostapd iniciado correctamente");
    }

    private async Task ConfigureDnsmasq()
    {
        Console.WriteLine("🔧 Configurando DHCP (dnsmasq)...");

        // Detener dnsmasq
        await ExecuteCommand("systemctl stop dnsmasq", ignoreErrors: true);

        var dnsmasqConfig = $@"interface={_interface}
bind-interfaces
dhcp-range={_dhcpRange},12h
dhcp-option=3,{_gatewayIp}
dhcp-option=6,8.8.8.8,8.8.4.4
server=8.8.8.8
server=8.8.4.4
log-queries
log-dhcp
";
        await File.WriteAllTextAsync("/etc/dnsmasq.d/captive-portal.conf", dnsmasqConfig);

        // Reiniciar dnsmasq
        await ExecuteCommand("systemctl start dnsmasq");
        await Task.Delay(2000);

        // Verificar estado
        var status = await ExecuteCommand("systemctl is-active dnsmasq", ignoreErrors: true);
        if (!status.Trim().Equals("active", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine("⚠️ Advertencia: dnsmasq podría no estar activo");
        }
        else
        {
            Console.WriteLine("✅ DHCP configurado y activo");
        }
    }

    private async Task BackupCurrentConfiguration()
    {
        _originalIptablesRules = await ExecuteCommand("iptables-save", ignoreErrors: true);
        _originalIpForwarding = await ExecuteCommand("sysctl net.ipv4.ip_forward", ignoreErrors: true);
    }

    public async Task RestoreConfiguration()
    {
        Console.WriteLine("\n🔄 Restaurando configuración de red...");

        // Detener hostapd
        await ExecuteCommand("killall hostapd", ignoreErrors: true);

        // Detener dnsmasq
        await ExecuteCommand("systemctl stop dnsmasq", ignoreErrors: true);

        // Limpiar iptables
        await ExecuteCommand("iptables -F", ignoreErrors: true);
        await ExecuteCommand("iptables -t nat -F", ignoreErrors: true);
        await ExecuteCommand("iptables -t mangle -F", ignoreErrors: true);
        await ExecuteCommand("iptables -X", ignoreErrors: true);

        // Desactivar IP forwarding
        await ExecuteCommand("sysctl -w net.ipv4.ip_forward=0", ignoreErrors: true);

        // Limpiar interfaz
        await ExecuteCommand($"ip addr flush dev {_interface}", ignoreErrors: true);
        await ExecuteCommand($"ip link set {_interface} down", ignoreErrors: true);

        // Reactivar NetworkManager en la interfaz WiFi
        await ExecuteCommand($"nmcli device set {_interface} managed yes", ignoreErrors: true);

        // Eliminar archivos de configuración
        await ExecuteCommand("rm -f /etc/dnsmasq.d/captive-portal.conf", ignoreErrors: true);
        await ExecuteCommand("rm -f /etc/hostapd/hostapd.conf", ignoreErrors: true);

        // Reiniciar servicios
        await ExecuteCommand("systemctl restart dnsmasq", ignoreErrors: true);

        Console.WriteLine("✅ Configuración restaurada\n");
    }

    private async Task<string> ExecuteCommand(string command, bool ignoreErrors = false)
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

        if (process.ExitCode != 0 && !ignoreErrors)
            throw new Exception($"Command failed: {command}\nError: {error}");

        return output;
    }

    private async Task<string> ExecuteCommandWithOutput(string command)
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

        return $"Output: {output}\nError: {error}\nExit Code: {process.ExitCode}";
    }

    public void Dispose()
    {
        RestoreConfiguration().Wait();
    }

}