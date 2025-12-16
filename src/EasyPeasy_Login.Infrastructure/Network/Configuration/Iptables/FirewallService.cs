using System.Text.RegularExpressions;
using EasyPeasy_Login.Application.Services.NetworkControl;
using EasyPeasy_Login.Shared;

namespace EasyPeasy_Login.Infrastructure.Network.Configuration;

public class FirewallService : IFirewallService
{
    private readonly ICommandExecutor _executor;
    private readonly INetworkConfiguration _config;
    private readonly ILogger _logger;

    // Regex to extract MAC addresses from iptables output
    private static readonly Regex MacAddressRegex = new(
        @"MAC\s+([0-9A-Fa-f]{2}:[0-9A-Fa-f]{2}:[0-9A-Fa-f]{2}:[0-9A-Fa-f]{2}:[0-9A-Fa-f]{2}:[0-9A-Fa-f]{2})",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public FirewallService(ICommandExecutor executor, INetworkConfiguration config, ILogger logger)
    {
        _executor = executor;
        _config = config;
        _logger = logger;
    }

    public async Task<bool> GrantInternetAccessAsync(string macAddress)
    {
        if (string.IsNullOrWhiteSpace(macAddress))
        {
            _logger.LogWarning("Attempted to grant internet access with empty MAC address");
            return false;
        }

        // Normalize MAC address format (lowercase with colons)
        macAddress = NormalizeMacAddress(macAddress);
        var iface = _config.Interface;

        try
        {
            // Check if already has access to avoid duplicate rules
            if (await HasInternetAccessAsync(macAddress))
            {
                _logger.LogInfo($"MAC {macAddress} already has internet access, skipping");
                return true;
            }

            // 1. Add FORWARD rule to allow traffic through
            var forwardResult = await _executor.ExecuteCommandAsync(
                IptablesCommands.GrantInternetAccessToMac(macAddress),
                ignoreErrors: false);

            if (!forwardResult.Success)
            {
                _logger.LogError($"❌ Failed to add FORWARD rule for MAC: {macAddress}. Error: {forwardResult.Error}");
                return false;
            }

            // 2. Add NAT rules to redirect DNS to external server and bypass HTTP/HTTPS redirection
            // These rules are inserted at position 1, so they take precedence over redirect rules
            // DNS: Must use DNAT to external DNS (8.8.8.8) because DHCP tells clients to use gateway as DNS
            await _executor.ExecuteCommandAsync(
                IptablesCommands.RedirectDnsToExternalForMac(iface, macAddress),
                ignoreErrors: true);

            await _executor.ExecuteCommandAsync(
                IptablesCommands.BypassHttpRedirectForMac(iface, macAddress),
                ignoreErrors: true);

            await _executor.ExecuteCommandAsync(
                IptablesCommands.BypassHttpsRedirectForMac(iface, macAddress),
                ignoreErrors: true);

            _logger.LogInfo($"✅ Internet access GRANTED to MAC: {macAddress} (FORWARD + NAT bypass)");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"❌ Exception granting internet access to MAC {macAddress}: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> RevokeInternetAccessAsync(string macAddress, string? clientIp = null)
    {
        return await RevokeInternetAccessAsync(macAddress, clientIp, forceDisconnect: true);
    }

    /// Revokes internet access from a device by its MAC address.
    /// When forceDisconnect is true, it immediately terminates ALL connections
    /// If clientIp is provided, flushes connection tracking for immediate disconnection
    public async Task<bool> RevokeInternetAccessAsync(string macAddress, string? clientIp, bool forceDisconnect)
    {
        if (string.IsNullOrWhiteSpace(macAddress))
        {
            _logger.LogWarning("Attempted to revoke internet access with empty MAC address");
            return false;
        }

        macAddress = NormalizeMacAddress(macAddress);
        var iface = _config.Interface;

        try
        {
            // STEP 1: FORCE DISCONNECT - Insert DROP rule at position 1 to kill ALL traffic immediately
            // This takes precedence over ESTABLISHED,RELATED rule
            if (forceDisconnect)
            {
                _logger.LogInfo($"⚡ FORCE DISCONNECT: Immediately blocking all traffic from MAC: {macAddress}");
                _logger.LogInfo($"   Command: iptables -I FORWARD 1 -i {iface} -m mac --mac-source {macAddress} -j DROP");
                
                var dropResult = await _executor.ExecuteCommandAsync(
                    IptablesCommands.ForceDropAllTrafficFromMac(iface, macAddress),
                    ignoreErrors: false);
                
                if (!dropResult.Success)
                {
                    _logger.LogError($"❌ FORCE DROP FAILED for MAC {macAddress}");
                    _logger.LogError($"   Exit Code: {dropResult.ExitCode}");
                    _logger.LogError($"   Error Output: {dropResult.Error}");
                    _logger.LogError($"   Standard Output: {dropResult.Output}");
                    return false;
                }
                else
                {
                    _logger.LogInfo($"✅ FORCE DROP rule inserted successfully at position 1");
                }
                
                // STEP 1.5: Flush connection tracking to immediately kill established connections
                // This forces the kernel to "forget" active TCP sessions
                if (!string.IsNullOrWhiteSpace(clientIp))
                {
                    _logger.LogInfo($"🔄 Flushing connection tracking for IP: {clientIp}");
                    await _executor.ExecuteCommandAsync(
                        IptablesCommands.FlushConnectionTrackingForIp(clientIp),
                        ignoreErrors: true);
                    _logger.LogInfo($"✅ Connection tracking flushed for IP: {clientIp}");
                }
                
                // Small delay to ensure active connections are terminated
                await Task.Delay(100); // Reduced delay since conntrack flush handles this now
            }

            // STEP 2: Remove NAT rules (DNS redirect to external + HTTP/HTTPS bypass)
            await _executor.ExecuteCommandAsync(
                IptablesCommands.RemoveDnsRedirectForMac(iface, macAddress),
                ignoreErrors: true);

            await _executor.ExecuteCommandAsync(
                IptablesCommands.RemoveHttpRedirectBypassForMac(iface, macAddress),
                ignoreErrors: true);

            await _executor.ExecuteCommandAsync(
                IptablesCommands.RemoveHttpsRedirectBypassForMac(iface, macAddress),
                ignoreErrors: true);

            // STEP 3: Remove from AUTHENTICATED chain
            // Try to delete multiple times in case there are duplicate rules
            int maxAttempts = 3;
            for (int i = 0; i < maxAttempts; i++)
            {
                var result = await _executor.ExecuteCommandAsync(
                    IptablesCommands.RevokeInternetAccessFromMac(macAddress),
                    ignoreErrors: true);
                
                if (!result.Success)
                    break; // No more rules to delete
            }

            // STEP 4: Clean up - Remove the force DROP rule after a delay
            // The device is now fully blocked by the absence of AUTHENTICATED rule
            if (forceDisconnect)
            {
                // Short delay - conntrack flush already handled immediate disconnection
                await Task.Delay(200);
                
                await _executor.ExecuteCommandAsync(
                    IptablesCommands.RemoveForceDropFromMac(iface, macAddress),
                    ignoreErrors: true);
                
                _logger.LogInfo($"🔒 Internet access REVOKED from MAC: {macAddress} (FORCE DISCONNECT completed)");
            }
            else
            {
                _logger.LogInfo($"🔒 Internet access REVOKED from MAC: {macAddress}");
            }
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"❌ Exception revoking internet access from MAC {macAddress}: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> HasInternetAccessAsync(string macAddress)
    {
        if (string.IsNullOrWhiteSpace(macAddress))
            return false;

        macAddress = NormalizeMacAddress(macAddress);

        try
        {
            var result = await _executor.ExecuteCommandAsync(
                IptablesCommands.CheckIfMacIsAuthenticated(macAddress),
                ignoreErrors: true);

            // If grep finds the MAC, output will contain it
            return result.Output.ToLower().Contains(macAddress.ToLower());
        }
        catch
        {
            return false;
        }
    }

    public async Task<IEnumerable<string>> GetAuthenticatedMacAddressesAsync()
    {
        var macAddresses = new List<string>();

        try
        {
            var result = await _executor.ExecuteCommandAsync(
                IptablesCommands.ListAuthenticatedChainRules(),
                ignoreErrors: true);

            if (result.Success && !string.IsNullOrWhiteSpace(result.Output))
            {
                var matches = MacAddressRegex.Matches(result.Output);
                foreach (Match match in matches)
                {
                    if (match.Groups.Count > 1)
                    {
                        macAddresses.Add(match.Groups[1].Value.ToLower());
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"❌ Exception getting authenticated MAC addresses: {ex.Message}");
        }

        return macAddresses.Distinct();
    }

    private static string NormalizeMacAddress(string macAddress)
    {
        // Remove any existing separators and convert to lowercase
        var cleanMac = macAddress.Replace(":", "").Replace("-", "").ToLower();
        
        if (cleanMac.Length != 12)
            return macAddress.ToLower(); // Return as-is if invalid format

        // Insert colons every 2 characters
        return string.Join(":", Enumerable.Range(0, 6).Select(i => cleanMac.Substring(i * 2, 2)));
    }
}
