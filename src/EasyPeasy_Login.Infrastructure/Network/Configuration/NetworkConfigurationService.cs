using System.Diagnostics;
using System.Text.RegularExpressions;

namespace EasyPeasy_Login.Infrastructure.Network.Configuration;

public class NetworkConfigurationService : INetworkConfigurationService
{
    private string? _interface;
    private string? _upstreamInterface;
    private bool _isVpnInterface = false;
    private string? _originalIptablesRules;
    private string? _originalIpForwarding;
    private string? _gatewayIp;
    private string? _dhcpRange;
    
    public event EventHandler<string>? LogReceived;

    private void Log(string message)
    {
        LogReceived?.Invoke(this, message);
    }

    public async Task<bool> SetupNetworkAsync(NetworkConfigurationParams configParams)
    {
        try
        {
            _interface = configParams.Interface;
            _upstreamInterface = configParams.UpstreamInterface;
            _isVpnInterface = configParams.IsVpnInterface;
            _gatewayIp = configParams.GatewayIp;
            _dhcpRange = configParams.DhcpRange;

            Log("=== Network Configuration Started ===");
            Log($"Interface: {_interface}");
            Log($"Upstream: {_upstreamInterface}");
            Log($"Gateway IP: {_gatewayIp}");
            Log($"DHCP Range: {_dhcpRange}");
            Log($"WiFi SSID: {configParams.Ssid}");
            Log($"Is VPN Interface: {_isVpnInterface}");
            Log("");

            // Save original configuration
            await SaveOriginalConfigurationAsync();

            // Configure static IP
            await ConfigureStaticIpAsync();

            // Configure WiFi (hostapd)
            await ConfigureHostapdAsync(configParams.Ssid, configParams.Password);

            // Configure DHCP (dnsmasq)
            await ConfigureDnsmasqAsync();

            // Configure firewall
            await ConfigureFirewallAsync();

            Log("");
            Log("=== Configuration completed successfully! ===");
            Log("The system is ready to accept connections.");
            
            return true;
        }
        catch (Exception ex)
        {
            Log($"ERROR: Configuration failed - {ex.Message}");
            await RestoreConfigurationAsync();
            return false;
        }
    }

    private async Task SaveOriginalConfigurationAsync()
    {
        Log("Saving original configuration...");
        
        _originalIptablesRules = await ExecuteCommand("iptables-save", true);
        _originalIpForwarding = await ExecuteCommand("cat /proc/sys/net/ipv4/ip_forward", true);
        
        Log("✓ Original configuration saved");
    }

    private async Task ConfigureStaticIpAsync()
    {
        Log($"\nConfiguring static IP on {_interface}...");
        
        await ExecuteCommand($"ip addr flush dev {_interface}");
        await ExecuteCommand($"ip addr add {_gatewayIp}/24 dev {_interface}");
        await ExecuteCommand($"ip link set {_interface} up");
        
        var ipInfo = await ExecuteCommand($"ip addr show {_interface}");
        Log($"✓ Static IP configured: {_gatewayIp}");
        Log($"Interface info:\n{ipInfo}");
    }

    private async Task ConfigureHostapdAsync(string ssid, string password)
    {
        Log("\nConfiguring WiFi Access Point (hostapd)...");
        
        var hostapdConfig = $@"interface={_interface}
driver=nl80211
ssid={ssid}
hw_mode=g
channel=7
wmm_enabled=0
macaddr_acl=0
auth_algs=1
ignore_broadcast_ssid=0
wpa=2
wpa_passphrase={password}
wpa_key_mgmt=WPA-PSK
wpa_pairwise=TKIP
rsn_pairwise=CCMP";

        await File.WriteAllTextAsync("/tmp/hostapd.conf", hostapdConfig);
        await ExecuteCommand("cp /tmp/hostapd.conf /etc/hostapd/hostapd.conf");
        
        Log("✓ hostapd configuration created");
        
        await ExecuteCommand("systemctl stop hostapd", true);
        await ExecuteCommand("systemctl start hostapd");
        await ExecuteCommand("systemctl enable hostapd", true);
        
        var status = await ExecuteCommand("systemctl is-active hostapd", true);
        Log($"✓ hostapd service status: {status.Trim()}");
    }

    private async Task ConfigureDnsmasqAsync()
    {
        Log("\nConfiguring DHCP server (dnsmasq)...");
        
        var rangeParts = _dhcpRange!.Split(',');
        var dnsmasqConfig = $@"interface={_interface}
dhcp-range={rangeParts[0]},{rangeParts[1]},255.255.255.0,24h
dhcp-option=3,{_gatewayIp}
dhcp-option=6,8.8.8.8,8.8.4.4
server=8.8.8.8
log-queries
log-dhcp
listen-address={_gatewayIp}";

        await File.WriteAllTextAsync("/tmp/dnsmasq.conf", dnsmasqConfig);
        await ExecuteCommand("cp /tmp/dnsmasq.conf /etc/dnsmasq.conf");
        
        Log("✓ dnsmasq configuration created");
        
        await ExecuteCommand("systemctl stop dnsmasq", true);
        await ExecuteCommand("systemctl start dnsmasq");
        await ExecuteCommand("systemctl enable dnsmasq", true);
        
        var status = await ExecuteCommand("systemctl is-active dnsmasq", true);
        Log($"✓ dnsmasq service status: {status.Trim()}");
    }

    private async Task ConfigureFirewallAsync()
    {
        Log("\nConfiguring firewall and NAT...");
        
        // Enable IP forwarding
        await ExecuteCommand("echo 1 > /proc/sys/net/ipv4/ip_forward");
        await ExecuteCommand("sysctl -w net.ipv4.ip_forward=1");
        
        Log("✓ IP forwarding enabled");
        
        // Flush existing rules
        await ExecuteCommand("iptables -F");
        await ExecuteCommand("iptables -t nat -F");
        
        // Configure NAT
        if (_isVpnInterface)
        {
            await ExecuteCommand($"iptables -t nat -A POSTROUTING -o {_upstreamInterface} -j MASQUERADE");
            await ExecuteCommand($"iptables -A FORWARD -i {_interface} -o {_upstreamInterface} -j ACCEPT");
            await ExecuteCommand($"iptables -A FORWARD -i {_upstreamInterface} -o {_interface} -m state --state RELATED,ESTABLISHED -j ACCEPT");
            Log($"✓ NAT configured for VPN interface: {_upstreamInterface}");
        }
        else
        {
            await ExecuteCommand($"iptables -t nat -A POSTROUTING -o {_upstreamInterface} -j MASQUERADE");
            await ExecuteCommand($"iptables -A FORWARD -i {_interface} -o {_upstreamInterface} -m state --state RELATED,ESTABLISHED -j ACCEPT");
            await ExecuteCommand($"iptables -A FORWARD -i {_upstreamInterface} -o {_interface} -j ACCEPT");
            Log($"✓ NAT configured for standard interface: {_upstreamInterface}");
        }
        
        // Save iptables rules
        await ExecuteCommand("iptables-save > /etc/iptables/rules.v4", true);
        
        var rules = await ExecuteCommand("iptables -L -v -n");
        Log($"✓ Firewall rules applied:\n{rules}");
    }

    public async Task<string> GetCurrentNetworkInfoAsync()
    {
        var info = new System.Text.StringBuilder();
        
        try
        {
            info.AppendLine("=== Current Network Configuration ===\n");
            
            // Network interfaces
            var interfaces = await ExecuteCommand("ip link show", true);
            info.AppendLine("Network Interfaces:");
            info.AppendLine(interfaces);
            info.AppendLine();
            
            // IP addresses
            var ipAddresses = await ExecuteCommand("ip addr show", true);
            info.AppendLine("IP Addresses:");
            info.AppendLine(ipAddresses);
            info.AppendLine();
            
            // Routing table
            var routes = await ExecuteCommand("ip route show", true);
            info.AppendLine("Routing Table:");
            info.AppendLine(routes);
            info.AppendLine();
            
            // iptables rules
            var iptables = await ExecuteCommand("iptables -L -v -n", true);
            info.AppendLine("Firewall Rules:");
            info.AppendLine(iptables);
            info.AppendLine();
            
            // Service status
            var hostapd = await ExecuteCommand("systemctl is-active hostapd", true);
            var dnsmasq = await ExecuteCommand("systemctl is-active dnsmasq", true);
            info.AppendLine("Service Status:");
            info.AppendLine($"hostapd: {hostapd.Trim()}");
            info.AppendLine($"dnsmasq: {dnsmasq.Trim()}");
        }
        catch (Exception ex)
        {
            info.AppendLine($"Error getting network info: {ex.Message}");
        }
        
        return info.ToString();
    }

    public async Task RestoreConfigurationAsync()
    {
        Log("\nRestoring original configuration...");
        
        try
        {
            if (!string.IsNullOrEmpty(_originalIptablesRules))
            {
                await File.WriteAllTextAsync("/tmp/iptables-restore.txt", _originalIptablesRules);
                await ExecuteCommand("iptables-restore < /tmp/iptables-restore.txt", true);
                Log("✓ iptables rules restored");
            }
            
            if (!string.IsNullOrEmpty(_originalIpForwarding))
            {
                await ExecuteCommand($"echo {_originalIpForwarding.Trim()} > /proc/sys/net/ipv4/ip_forward", true);
                Log("✓ IP forwarding restored");
            }
            
            await ExecuteCommand("systemctl stop hostapd", true);
            await ExecuteCommand("systemctl stop dnsmasq", true);
            
            if (!string.IsNullOrEmpty(_interface))
            {
                await ExecuteCommand($"ip addr flush dev {_interface}", true);
                await ExecuteCommand($"ip link set {_interface} down", true);
            }
            
            Log("✓ Configuration restored successfully");
        }
        catch (Exception ex)
        {
            Log($"Warning: Error during restoration - {ex.Message}");
        }
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
        RestoreConfigurationAsync().Wait();
    }
}