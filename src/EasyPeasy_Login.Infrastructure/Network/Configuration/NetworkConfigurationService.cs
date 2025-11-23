using System.Diagnostics;
using System.Text.RegularExpressions;
using EasyPeasy_Login.Shared.Constants;

namespace EasyPeasy_Login.Infrastructure.Network.Configuration;

public class NetworkConfigurationService : INetworkConfigurationService
{
    private readonly string _interface = NetworkConstants.Interface;
    private readonly string _gatewayIp = NetworkConstants.GatewayIp;
    private readonly string _dhcpRange = NetworkConstants.DhcpRange;
    private readonly string _ssid = NetworkConstants.Ssid;
    private readonly string _password = NetworkConstants.Password; 
    public const int DefaultPort = NetworkConstants.DefaultPort;
    public const int MaxConnections = NetworkConstants.MaxConnections;
    public const int TimeoutMilliseconds = NetworkConstants.TimeoutMilliseconds;
    private string? _upstreamInterface;
    private bool _isVpnInterface = false;
    private string? _originalIptablesRules;
    private string? _originalIpForwarding;

    public Task<bool> SetupNetwork()
    {
        throw new NotImplementedException();
    }
    
    public void RestoreConfiguration()
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }

}