using System.Diagnostics;
using System.Text.RegularExpressions;
using EasyPeasy_Login.Infrastructure.Network.Configuration;
using EasyPeasy_Login.Shared;

namespace EasyPeasy_Login.Infrastructure.Network;

public class MacAddressResolver : IMacAddressResolver
{
    private readonly ILogger logger;
    private readonly ICommandExecutor executor;
    public MacAddressResolver(ILogger logger, ICommandExecutor commandExecutor)
    {
        this.logger = logger;
        executor = commandExecutor;
    }
    public async Task<string?> GetMacAddressFromIpAsync(string ipAddress)
    {
        try
        {
            var arpOutput = await executor.ExecuteCommandAsync($"ip neigh show {ipAddress}");
            
            var macMatch = Regex.Match(arpOutput.Output, @"([0-9A-Fa-f]{2}[:-]){5}([0-9A-Fa-f]{2})");
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
            logger.LogError($"Error obteniendo MAC para IP {ipAddress}: {ex.Message}");
            return null;
        }
    }
}