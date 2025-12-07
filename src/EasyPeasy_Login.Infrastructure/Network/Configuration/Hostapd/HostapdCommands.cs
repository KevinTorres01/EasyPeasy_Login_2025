public static class HostapdCommands
{
    public static string StartHostapd(string configPath)
    {
        return $"hostapd {configPath} -B";
    }
    public static string StopHostapd()
    {
        return "killall hostapd";
    }
    public static string GetHostapdPid()
    {
        return "pgrep hostapd";
    }
}