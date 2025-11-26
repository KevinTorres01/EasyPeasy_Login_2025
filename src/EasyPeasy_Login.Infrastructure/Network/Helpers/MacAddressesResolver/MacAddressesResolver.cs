using System.Diagnostics;
using System.Text.RegularExpressions;

namespace EasyPeasy_Login.Infrastructure.Network;

public class MacAddressResolver : IMacAddressResolver
{
    public async Task<string?> GetMacAddressFromIpAsync(string ipAddress)
    {
        try
        {
            var arpOutput = await ExecuteCommandAsync($"ip neigh show {ipAddress}");
            
            var macMatch = Regex.Match(arpOutput, @"([0-9A-Fa-f]{2}[:-]){5}([0-9A-Fa-f]{2})");
            if (macMatch.Success)
            {
                return macMatch.Value.ToLower();
            }

            var leasesPath = "/var/lib/misc/dnsmasq.leases";
            if (File.Exists(leasesPath))
            {
                var leases = await File.ReadAllLinesAsync(leasesPath);
                foreach (var lease in leases)
                {
                    // Formate: timestamp mac ip hostname clientid
                    var parts = lease.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length >= 3 && parts[2] == ipAddress)
                    {
                        return parts[1].ToLower(); // MAC address
                    }
                }
            }

            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error obteniendo MAC para IP {ipAddress}: {ex.Message}");
            return null;
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
        var output = await process.StandardOutput.ReadToEndAsync();
        await process.WaitForExitAsync();
        
        return output;
    }
}