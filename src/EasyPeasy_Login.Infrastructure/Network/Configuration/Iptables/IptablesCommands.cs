public static class IptablesCommands
{
    public static string CleanFirewallFilterTable()
    {
        return "iptables -F";
    }
    public static string CleanFirewallNatTable()
    {
        return "iptables -t nat -F";
    }
    public static string CleanFirewallMangleTable()
    {
        return "iptables -t mangle -F";
    }
    public static string CleanFirewallPersonalizedChains()
    {
        return "iptables -X";
    }
    public static string CleanFirewallNatTablePersonalizedChains()
    {
        return "iptables -t nat -X";
    }
    public static string CleanFirewallMangleTablePersonalizedChains()
    {
        return "iptables -t mangle -X";
    }
    public static string AllowPacketsGettingInToThisDevice()
    {
        return "iptables -P INPUT ACCEPT";
    }
    public static string ForbidPacketsPassingThroughThisDevice()
    {
        return "iptables -P FORWARD DROP";
    }
    public static string AllowPacketsPassingThroughThisDevice()
    {
        return "iptables -P FORWARD ACCEPT";
    }
    public static string ForbidPacketsPassingThroughThisDevice(string _interface)
    {
        return $"iptables -A FORWARD -i {_interface} -j DROP";
    }
    public static string AllowPacketsGettingOutFromThisDevice()
    {
        return "iptables -P OUTPUT ACCEPT";
    }
    public static string AllowLoopbackPacketsGettingIn()
    {
        return "iptables -A INPUT -i lo -j ACCEPT";
    }
    public static string AllowLoopbackPacketsGettingOut()
    {
        return "iptables -A OUTPUT -o lo -j ACCEPT";
    }
    public static string AllowDnsGettingInToThisDeviceUdp(string _interface)
    {
        return $"iptables -A INPUT -p udp --dport 53 -j ACCEPT";
    }
    public static string AllowDnsGettingInToThisDeviceTcp(string _interface)
    {
        return $"iptables -A INPUT -p tcp --dport 53 -j ACCEPT";
    }
    public static string AllowDhcpGettingIn(string _interface)
    {
        return $"iptables -A INPUT -i {_interface} -p udp --dport 67:68 -j ACCEPT";
    }
    public static string AllowDhcpGettingOut(string _interface)
    {
        return $"iptables -A OUTPUT -o {_interface} -p udp --dport 67:68 -j ACCEPT";
    }
    public static string AllowAccessToPortal(string _interface, int port)
    {
        return $"iptables -A INPUT -i {_interface} -p tcp --dport {port} -j ACCEPT";
    }
    public static string AllowIcmpGettingInToThisDevice(string _interface)
    {
        return $"iptables -A INPUT -i {_interface} -p icmp -j ACCEPT";
    }
    public static string AllowIcmpPassingThroughThisDevice()
    {
        return "iptables -A FORWARD -p icmp -j ACCEPT";
    }
    public static string RedirectHttpTrafficToPortal(string _interface, string portalIp, int port)
    {
        return $"iptables -t nat -A PREROUTING -i {_interface} -p tcp --dport 80 -j DNAT --to-destination {portalIp}:{port}";
    }
    public static string RedirectHttpsTrafficToPortal(string _interface, string portalIp, int port)
    {
        return $"iptables -t nat -A PREROUTING -i {_interface} -p tcp --dport 443 -j DNAT --to-destination {portalIp}:{port}";
    }
    public static string CreatePersonalizedChainForAuthenticatedUser()
    {
        return "iptables -N AUTHENTICATED";
    }
    public static string CleanPersonalizedChainForAuthenticatedUser()
    {
        return "iptables -F AUTHENTICATED";
    }
    public static string InterceptUdpDnsTrafficAndRedirectToThisDevice(string _interface)
    {
        return $"iptables -t nat -A PREROUTING -i {_interface} -p udp --dport 53 -j REDIRECT --to-port 53";
    }
    public static string InterceptTcpDnsTrafficAndRedirectToThisDevice(string _interface)
    {
        return $"iptables -t nat -A PREROUTING -i {_interface} -p tcp --dport 53 -j REDIRECT --to-port 53";
    }
    public static string ListAllRulesFromPreroutingNatTable()
    {
        return "iptables -t nat -L PREROUTING -n --line-numbers";
    }
    public static string AllowTrafficFromEstablishedOrRelatedConnectionsGettingInToThisDevice()
    {
        return "iptables -A INPUT -m state --state ESTABLISHED,RELATED -j ACCEPT";
    }
    public static string AllowTrafficFromEstablishedOrRelatedConnectionsPassingThroughThisDevice()
    {
        return "iptables -A FORWARD -m state --state ESTABLISHED,RELATED -j ACCEPT";
    }
    public static string AllowTrafficFromEstablishedOrRelatedConnectionsGettingOutFromThisDevice()
    {
        return "iptables -A OUTPUT -m state --state ESTABLISHED,RELATED -j ACCEPT";
    }

    public static string RedirectTrafficPassingThroughToCustomChainForAuthenticatedUsers(string _interface)
    {
        return $"iptables -A FORWARD -i {_interface} -j AUTHENTICATED";
    }

    public static string ConfigureNat(string upstreamInterface)
    {
        return $"iptables -t nat -A POSTROUTING -o {upstreamInterface} -j MASQUERADE";
    }

    public static string OptimizesTcpSegmentSize()
    {
        return "iptables -t mangle -A FORWARD -p tcp --tcp-flags SYN,RST SYN -j TCPMSS --clamp-mss-to-pmtu";
    }

    public static string ShowNatTablePreroutingChainInfo()
    {
        return "iptables -t nat -L PREROUTING -n -v --line-numbers";
    }

    public static string ShowFirewalRulesOnInputChainRelatedToPort(int port)
    {
        return $"iptables -L INPUT -n -v | grep -E 'dpt:{port}|spt:{port}'";
    }
}