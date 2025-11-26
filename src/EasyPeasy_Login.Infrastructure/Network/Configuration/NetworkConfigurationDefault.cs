using EasyPeasy_Login.Shared.Constants;

public static class NetworkConfigurationDefaults
{
    public static readonly string _interface = NetworkConstants.Interface;
    public static readonly string _gatewayIp = NetworkConstants.GatewayIp;
    public static readonly string _dhcpRange = NetworkConstants.DhcpRange;
    public static readonly string _ssid = NetworkConstants.Ssid;
    public static readonly string _password = NetworkConstants.Password;
    public const int DefaultPort = NetworkConstants.DefaultPort;
    public static string? _upstreamInterface;
    public static bool _isVpnInterface = false;
    public static string? _originalIptablesRules;
    public static string? _originalIpForwarding;
}