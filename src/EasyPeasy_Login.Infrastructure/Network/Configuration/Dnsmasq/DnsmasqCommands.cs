public static class DnsmasqCommands
{
    public static string StartDnsSystemResolver()
    {
        return "systemctl start systemd-resolved";
    }
    public static string StopDnsSystemResolver()
    {
        return "systemctl stop systemd-resolved";
    }
    public static string DisableCompletelyDnsSystemResolverByMasking()
    {
        return "systemctl mask systemd-resolved";
    }
    public static string UnmaskDnsSystemResolver()
    {
        return "systemctl unmask systemd-resolved";
    }
    public static string CheckActivityStatusOfDnsSystemResolver()
    {
        return "systemctl is-active systemd-resolved";
    }
    public static string StartDnsmasq()
    {
        return "systemctl start dnsmasq";
    }
    public static string CheckActivityStatusOfDnsmasq()
    {
        return "systemctl is-active dnsmasq";
    }
    public static string StopDnsmasq()
    {
        return "systemctl stop dnsmasq";
    }
    public static string RestartDnsmasq()
    {
        return "systemctl restart dnsmasq";
    }
    public static string ShowLastLogsOfDnsmasq(int linesCount)
    {
        return $"journalctl -u dnsmasq -n {linesCount} --no-pager";
    }
    public static string CreateBackupOfConfigFile()
    {
        return "cp /etc/dnsmasq.conf /etc/dnsmasq.conf.backup";
    }
    public static string RestoreConfigFileFromBackup()
    {
        return "mv /etc/dnsmasq.conf.backup /etc/dnsmasq.conf";
    }
    public static string MakeDnsRequestUsingGatewayDnsServer(string domain, string gatewayIp)
    {
        return $"nslookup {domain} {gatewayIp}";
    }  
    public static string RemoveConfigOrLogFile(string path)
    {
        return $"rm -f {path}";
    }
    public static string ShowLastLogsOfDnsmasqInCaptivePortal(int linesCount)
    {
        return $"tail -{linesCount} /var/log/dnsmasq-captive.log";
    }
}