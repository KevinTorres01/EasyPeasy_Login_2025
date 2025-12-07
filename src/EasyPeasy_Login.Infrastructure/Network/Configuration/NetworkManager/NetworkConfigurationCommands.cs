// ip link up/down, ip addr add/flush, sysctl, nmcli commands

namespace EasyPeasy_Login.Infrastructure.Network.Configuration
{
    public static class NetworkConfigurationsCommands
    {
        // ip link
        public static string DeactivateNetworkInterface(string _interface)
        {
            return $"ip link set {_interface} down";
        }

        public static string ActivateNetworkInterface(string _interface)
        {
            return $"ip link set {_interface} up";
        }

        public static string ShowAllNetworkInterfacesInfo()
        {
            return "ip -o link show";
        }

        // ip addr
        public static string CleanIpAddressesAssociatedToAnInterface(string _interface)
        {
            return $"ip addr flush dev {_interface}";
        }
        public static string AddIpAddressToRoutesTable(string route, string _interface)
        {
            return $"ip addr add {route}/24 dev {_interface}";
        }
        public static string ShowInfoAboutIpAddressConfiguredOnInterface(string _interface)
        {
            return $"ip -o addr show {_interface}";
        }
        public static string GetDefaultRoute()
        {
            return "ip route show default";
        }

        // sysctl
        public static string EnableIpPacketForwarding()
        {
            return "sysctl -w net.ipv4.ip_forward=1";
        }
        public static string DisableIpPacketForwarding()
        {
            return "sysctl -w net.ipv4.ip_forward=0";
        }

        // nmcli
        public static string DisableLinuxNetworkManager(string _interface)
        {
            return $"nmcli device set {_interface} managed no";
        }

        public static string EnableLinuxNetworkManager(string _interface)
        {
            return $"nmcli device set {_interface} managed yes";
        }

        // netstat
        public static string GetAllProcessListeningOnPort(int port)
        {
            return $"netstat -tulpn | grep :{port}";
        }

        // rfkill
        public static string ShowAllWirelessDevicesBlockState()
        {
            return "rfkill list";
        }

        public static string UnblockAllWifiDevicesBySoftware()
        {
            return "rfkill unblock wifi";
        }

        public static string UnblockAllWirelessDevicesBySoftware()
        {
            return "rfkill unblock all";
        }
    }
}