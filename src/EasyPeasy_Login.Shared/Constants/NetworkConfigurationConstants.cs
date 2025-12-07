namespace EasyPeasy_Login.Shared.Constants;

public static class NetworkConstants
{
    // Network interface name
    public static readonly string Interface = "wlp2s0";

    // IP configuration details

    // Gateway IP address
    public static readonly string GatewayIp = "192.168.100.1";

    // DHCP Range for dynamic IP allocation
    public static readonly string DhcpRange = "192.168.100.50,192.168.100.150";

    // DNS settings

    // Default port for network communication
    public const int DefaultPort = 8080;

    // WiFi Credentials for guest network
    public static readonly string Ssid = "EasyPeasy_Guest";
    public static readonly string Password = "12345678"; 
}