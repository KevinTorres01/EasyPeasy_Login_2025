namespace EasyPeasy_Login.Infrastructure.Network.Configuration;

/// <summary>
/// Interface for network configuration to enable dependency injection and testing.
/// </summary>
public interface INetworkConfiguration
{
    // Editable network configuration
    string Interface { get; set; }
    string GatewayIp { get; set; }
    string DhcpRange { get; set; }
    string Ssid { get; set; }
    string Password { get; set; }
    int DefaultPort { get; set; }

    // Runtime state
    string? UpstreamInterface { get; set; }
    bool IsVpnInterface { get; set; }
    string? OriginalIptablesRules { get; set; }
    string? OriginalIpForwarding { get; set; }
    
    // Network status
    bool IsNetworkActive { get; set; }

    void ResetRuntimeState();
    void ResetToDefaults();
}
