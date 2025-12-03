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
    
    // Backup and configure resolv.conf for server's own DNS resolution
    public static string BackupResolvConf()
    {
        return "cp /etc/resolv.conf /etc/resolv.conf.backup";
    }
    public static string RestoreResolvConf()
    {
        return "cp /etc/resolv.conf.backup /etc/resolv.conf";
    }
    public static string ConfigureServerDns()
    {
        // Configure the server to use external DNS directly (bypassing dnsmasq spoofing)
        return "echo -e 'nameserver 8.8.8.8\\nnameserver 8.8.4.4\\nnameserver 1.1.1.1' > /etc/resolv.conf";
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