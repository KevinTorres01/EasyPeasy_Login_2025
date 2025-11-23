public interface INetworkConfigurationService : IDisposable
{
    event EventHandler<string>? LogReceived;
    Task<bool> SetupNetworkAsync(NetworkConfigurationParams configParams);
    Task RestoreConfigurationAsync();
    Task<string> GetCurrentNetworkInfoAsync();
}

public class NetworkConfigurationParams
{
    public string Interface { get; set; } = string.Empty;
    public string UpstreamInterface { get; set; } = string.Empty;
    public string GatewayIp { get; set; } = string.Empty;
    public string DhcpRange { get; set; } = string.Empty;
    public string Ssid { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool IsVpnInterface { get; set; }
}