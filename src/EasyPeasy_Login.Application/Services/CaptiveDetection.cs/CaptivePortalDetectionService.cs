using System.Diagnostics;

namespace EasyPeasy_Login.Application.Services.CaptiveDetection;

public class CaptivePortalDetectionService:ICaptivePortalDetectionService
{
    private const string PortalIp = "192.168.100.1"; 
    private const int PortalPort = 8080;

    public async Task EnforcePortalForIpAsync(string clientIpAddress)
    {
        if (string.IsNullOrWhiteSpace(clientIpAddress)) return;

        // Redirige tráfico HTTP (puerto 80) de esa IP hacia nuestro portal
        string arguments = $"-t nat -I PREROUTING 1 -s {clientIpAddress} -p tcp --dport 80 -j DNAT --to-destination {PortalIp}:{PortalPort}";

        await RunCommandAsync("iptables", arguments);
    }

    private async Task RunCommandAsync(string command, string args)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = command,
            Arguments = args,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(startInfo);
        if (process != null)
        {
            await process.WaitForExitAsync();
        }
    }
}