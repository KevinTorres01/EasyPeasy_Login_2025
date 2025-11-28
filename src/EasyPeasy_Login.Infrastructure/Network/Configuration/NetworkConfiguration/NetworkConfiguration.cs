using EasyPeasy_Login.Shared.Constants;

namespace EasyPeasy_Login.Infrastructure.Network.Configuration;

/// <summary>
/// Encapsulates network configuration state for a single captive portal session.
/// This class is NOT static, allowing each orchestrator instance to have its own isolated state.
/// Register as Scoped or Singleton in DI depending on your needs.
/// </summary>
public class NetworkConfiguration : INetworkConfiguration
{
    // Editable configuration (initialized from constants)
    public string Interface { get; set; } = NetworkConstants.Interface;
    public string GatewayIp { get; set; } = NetworkConstants.GatewayIp;
    public string DhcpRange { get; set; } = NetworkConstants.DhcpRange;
    public string Ssid { get; set; } = NetworkConstants.Ssid;
    public string Password { get; set; } = NetworkConstants.Password;
    public int DefaultPort { get; set; } = NetworkConstants.DefaultPort;

    // Mutable runtime state (isolated per instance)
    public string? UpstreamInterface { get; set; }
    public bool IsVpnInterface { get; set; }
    public string? OriginalIptablesRules { get; set; }
    public string? OriginalIpForwarding { get; set; }
    
    // Network status
    public bool IsNetworkActive { get; set; }

    /// <summary>
    /// Resets the mutable runtime state to defaults.
    /// Call this when restoring configuration.
    /// </summary>
    public void ResetRuntimeState()
    {
        UpstreamInterface = null;
        IsVpnInterface = false;
        OriginalIptablesRules = null;
        OriginalIpForwarding = null;
        IsNetworkActive = false;
    }
    
    /// <summary>
    /// Resets all configuration to default values from NetworkConstants.
    /// </summary>
    public void ResetToDefaults()
    {
        Interface = NetworkConstants.Interface;
        GatewayIp = NetworkConstants.GatewayIp;
        DhcpRange = NetworkConstants.DhcpRange;
        Ssid = NetworkConstants.Ssid;
        Password = NetworkConstants.Password;
        DefaultPort = NetworkConstants.DefaultPort;
        ResetRuntimeState();
    }
}
