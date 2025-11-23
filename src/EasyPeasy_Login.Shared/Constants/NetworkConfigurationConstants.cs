namespace EasyPeasy_Login.Shared.Constants;

public static class NetworkConstants
{
    // Network interface
    public static readonly string Interface = "wlp2s0";

    // IP configuration

    // Gateway
    public static readonly string GatewayIp = "192.168.100.1";

    // DHCP Range
    public static readonly string DhcpRange = "192.168.100.50,192.168.100.150";

    // DNS

    // Ports
    public const int DefaultPort = 8080;

    // WiFi Credentials
    public static readonly string Ssid = "EasyPeasy_Guest";
    public static readonly string Password = "12345678"; 
    public const int MaxConnections = 100;
    public const int TimeoutMilliseconds = 30000;
}