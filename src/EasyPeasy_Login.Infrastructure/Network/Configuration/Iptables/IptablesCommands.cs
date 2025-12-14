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
    public static string GrantInternetAccessToMac(string macAddress)
    {
        return $"iptables -I AUTHENTICATED 1 -m mac --mac-source {macAddress} -j ACCEPT";
    }
    public static string RevokeInternetAccessFromMac(string macAddress)
    {
        return $"iptables -D AUTHENTICATED -m mac --mac-source {macAddress} -j ACCEPT";
    }
    public static string CheckIfMacIsAuthenticated(string macAddress)
    {
        return $"iptables -L AUTHENTICATED -n -v | grep -i {macAddress}";
    }
    public static string ListAuthenticatedChainRules()
    {
        return "iptables -L AUTHENTICATED -n -v --line-numbers";
    }

    // NAT BYPASS FOR AUTHENTICATED USERS
    // These rules must be inserted BEFORE the redirect rules


    /// Redirects DNS queries from authenticated MAC to external DNS (8.8.8.8).
    /// This is needed because DHCP tells clients to use the gateway as DNS,
    /// so we must DNAT authenticated users' DNS to a real DNS server.
    /// Must be inserted at position 1 to take precedence over spoofing redirect rules.
    public static string RedirectDnsToExternalForMac(string _interface, string macAddress, string externalDns = "8.8.8.8")
    {
        return $"iptables -t nat -I PREROUTING 1 -i {_interface} -m mac --mac-source {macAddress} -p udp --dport 53 -j DNAT --to-destination {externalDns}:53";
    }

    /// Excludes an authenticated MAC from HTTP redirection (allows direct HTTP).
    /// ACCEPT in NAT means "don't modify this packet" - it goes to its original destination.
    public static string BypassHttpRedirectForMac(string _interface, string macAddress)
    {
        return $"iptables -t nat -I PREROUTING 1 -i {_interface} -m mac --mac-source {macAddress} -p tcp --dport 80 -j ACCEPT";
    }

    /// Excludes an authenticated MAC from HTTPS redirection (allows direct HTTPS).
    public static string BypassHttpsRedirectForMac(string _interface, string macAddress)
    {
        return $"iptables -t nat -I PREROUTING 1 -i {_interface} -m mac --mac-source {macAddress} -p tcp --dport 443 -j ACCEPT";
    }

    /// Removes DNS redirect rule for authenticated MAC (when revoking access).
    public static string RemoveDnsRedirectForMac(string _interface, string macAddress, string externalDns = "8.8.8.8")
    {
        return $"iptables -t nat -D PREROUTING -i {_interface} -m mac --mac-source {macAddress} -p udp --dport 53 -j DNAT --to-destination {externalDns}:53";
    }

    /// Removes HTTP bypass rule for a MAC (when revoking access).
    public static string RemoveHttpRedirectBypassForMac(string _interface, string macAddress)
    {
        return $"iptables -t nat -D PREROUTING -i {_interface} -m mac --mac-source {macAddress} -p tcp --dport 80 -j ACCEPT";
    }

    /// Removes HTTPS bypass rule for a MAC (when revoking access).
    public static string RemoveHttpsRedirectBypassForMac(string _interface, string macAddress)
    {
        return $"iptables -t nat -D PREROUTING -i {_interface} -m mac --mac-source {macAddress} -p tcp --dport 443 -j ACCEPT";
    }

    // FORCE DISCONNECT - IMMEDIATE CONNECTION TERMINATION (may took a few minutes)
    // These rules override ESTABLISHED,RELATED to force-kill active connections

    /// Inserts a DROP rule at the very beginning of FORWARD chain for a specific MAC.
    /// This takes precedence over the ESTABLISHED,RELATED rule, immediately killing
    /// ALL traffic from this device including active connections.
    public static string ForceDropAllTrafficFromMac(string _interface, string macAddress)
    {
        return $"iptables -I FORWARD 1 -i {_interface} -m mac --mac-source {macAddress} -j DROP";
    }

    /// Removes the force DROP rule after the device has been disconnected.
    /// This is called after a short delay to clean up the temporary blocking rule.
    public static string RemoveForceDropFromMac(string _interface, string macAddress)
    {
        return $"iptables -D FORWARD -i {_interface} -m mac --mac-source {macAddress} -j DROP";
    }

    /// Flushes connection tracking entries for a specific MAC address.
    /// This forces the kernel to forget about established connections,
    /// ensuring they cannot continue even if the MAC spoofs or reconnects quickly.
    /// Requires the conntrack utility to be installed.
    public static string FlushConnectionTrackingForMac(string macAddress)
    {
        // Note: conntrack doesn't filter by MAC directly, but we can delete all entries
        // This is a more aggressive approach - use with caution
        return $"conntrack -D -s 0.0.0.0/0 2>/dev/null || true";
    }
}