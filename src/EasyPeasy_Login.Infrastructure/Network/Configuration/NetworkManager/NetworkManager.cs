using System.Linq.Expressions;
using System.Text.RegularExpressions;
using EasyPeasy_Login.Shared;

namespace EasyPeasy_Login.Infrastructure.Network.Configuration
{
    public class NetworkManager : INetworkManager
    {
        private readonly ILogger logger;
        private readonly ICommandExecutor executor;
        private readonly INetworkConfiguration config;
        
        public NetworkManager(ILogger logger, ICommandExecutor executor, INetworkConfiguration networkConfiguration)
        {
            this.logger = logger;
            this.executor = executor;
            this.config = networkConfiguration;
        }
        public async Task<ExecutionResult> ConfigureNetworkInterface()
        {
            logger.LogInfo("🔧 Configuring network interface...");

            await executor.ExecuteCommandAsync(NetworkConfigurationsCommands.DisableLinuxNetworkManager(config.Interface));
            await Task.Delay(1000);

            await executor.ExecuteCommandAsync(NetworkConfigurationsCommands.CleanIpAddressesAssociatedToAnInterface(config.Interface));
            await executor.ExecuteCommandAsync(NetworkConfigurationsCommands.DeactivateNetworkInterface(config.Interface));
            await Task.Delay(500);

            await executor.ExecuteCommandAsync(NetworkConfigurationsCommands.ActivateNetworkInterface(config.Interface));
            await executor.ExecuteCommandAsync(NetworkConfigurationsCommands.AddIpAddressToRoutesTable(config.GatewayIp, config.Interface));

            logger.LogInfo("✅ Network interface configuration created successfully");
            return new ExecutionResult(0, "Network interface configured successfully.", string.Empty);
        }

        public async Task<string?> DetectUpstreamInterface()
        {
            logger.LogInfo("🔍 Searching for an interface with Internet access...");

            try
            {
                var interfacesOutput = await executor.ExecuteCommandAsync(NetworkConfigurationsCommands.ShowAllNetworkInterfacesInfo(), ignoreErrors: true);

                logger.LogInfo("\n📋 Available network interfaces:");

                var availableInterfaces = new List<string>();
                foreach (var line in interfacesOutput.Output.Split('\n'))
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    var match = Regex.Match(line, @"^\d+:\s+([^:@]+)");
                    if (match.Success)
                    {
                        var ifaceName = match.Groups[1].Value.Trim();
                        if (ifaceName != "lo" && ifaceName != config.Interface &&
                            !ifaceName.StartsWith("veth") &&
                            !ifaceName.StartsWith("docker") &&
                            !ifaceName.StartsWith("br-"))
                        {
                            availableInterfaces.Add(ifaceName);

                            var addrInfo = await executor.ExecuteCommandAsync(NetworkConfigurationsCommands.ShowInfoAboutIpAddressConfiguredOnInterface(ifaceName), ignoreErrors: true);
                            var hasIp = addrInfo.Output.Contains("inet ");
                            var state = line.Contains("state UP") ? "UP" : "DOWN";
                            var isVpn = ifaceName.StartsWith("tun") || ifaceName.StartsWith("tap");

                            logger.LogInfo($"  - {ifaceName} [{state}] {(hasIp ? "✅ Has IP" : "❌ No IP")}{(isVpn ? " 🔒 VPN" : "")}");
                        }
                    }
                }

                var routeOutput = await executor.ExecuteCommandAsync(NetworkConfigurationsCommands.GetDefaultRoute(), ignoreErrors: true);

                if (!string.IsNullOrWhiteSpace(routeOutput.Output))
                {
                    var match = Regex.Match(routeOutput.Output, @"dev\s+(\S+)");
                    if (match.Success)
                    {
                        var upstreamIface = match.Groups[1].Value;

                        if (!upstreamIface.StartsWith("tun") &&
                            !upstreamIface.StartsWith("tap") &&
                            upstreamIface != config.Interface)
                        {
                            return upstreamIface;
                        }
                        else if (upstreamIface.StartsWith("tun") || upstreamIface.StartsWith("tap"))
                        {
                            logger.LogWarning($"⚠️ VPN detected ({upstreamIface}), searching for a physical interface...");

                            foreach (var iface in availableInterfaces)
                            {
                                if (iface.StartsWith("usb") ||
                                    iface.StartsWith("enp") ||
                                    iface.StartsWith("eth") ||
                                    iface.StartsWith("enx"))
                                {
                                    var addrInfo = await executor.ExecuteCommandAsync(NetworkConfigurationsCommands.ShowInfoAboutIpAddressConfiguredOnInterface(iface), ignoreErrors: true);
                                    if (addrInfo.Output.Contains("inet ") && addrInfo.Output.Contains("state UP"))
                                    {
                                        logger.LogInfo($"💡 Physical interface found: {iface}");
                                        return iface;
                                    }
                                }
                            }

                            logger.LogWarning($"💡 No physical interface found, using VPN: {upstreamIface}");
                            logger.LogInfo("VPN will be used to share the Internet");

                            config.IsVpnInterface = true;
                            return upstreamIface;
                        }
                    }
                }

                foreach (var iface in availableInterfaces)
                {
                    var addrInfo = await executor.ExecuteCommandAsync(NetworkConfigurationsCommands.ShowAllNetworkInterfacesInfo(), ignoreErrors: true);
                    if (addrInfo.Output.Contains("inet ") && !addrInfo.Output.Contains("127.0.0.1") && addrInfo.Output.Contains("state UP"))
                    {
                        logger.LogWarning($"⚠️ No default route, using active interface: {iface}");
                        return iface;
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                logger.LogError($"⚠️ Error detecting upstream interface: {ex.Message}");
                return null;
            }
        }

        public async Task<ExecutionResult> EnableIpPacketForwarding()
        {
            return await executor.ExecuteCommandAsync(NetworkConfigurationsCommands.EnableIpPacketForwarding());
        }

        public async Task<ExecutionResult> RestartNetworkInterface()
        {
            await executor.ExecuteCommandAsync(NetworkConfigurationsCommands.DeactivateNetworkInterface(config.Interface));
            await executor.ExecuteCommandAsync(NetworkConfigurationsCommands.ActivateNetworkInterface(config.Interface));        
            return new ExecutionResult(0, "Network interface restarted successfully.", string.Empty);
        }

        public async Task<ExecutionResult> RestoreNetworkInterfaceConfiguration()
        {
            logger.LogInfo("\n🔄 Restaurando configuración de red...");
            await executor.ExecuteCommandAsync(NetworkConfigurationsCommands.DisableIpPacketForwarding(), ignoreErrors:true);

            await executor.ExecuteCommandAsync(NetworkConfigurationsCommands.CleanIpAddressesAssociatedToAnInterface(config.Interface), ignoreErrors:true);
            await executor.ExecuteCommandAsync(NetworkConfigurationsCommands.DeactivateNetworkInterface(config.Interface), ignoreErrors: true);

            await executor.ExecuteCommandAsync(NetworkConfigurationsCommands.EnableLinuxNetworkManager(config.Interface), ignoreErrors:true);

            return new ExecutionResult(0, "Network interface restored successfully.", string.Empty);
        }

        public async Task<ExecutionResult> UnblockRfkill()
        {
            try
            {
                var rfkillStatus = await executor.ExecuteCommandAsync(NetworkConfigurationsCommands.ShowAllWirelessDevicesBlockState(), ignoreErrors: true);
                logger.LogInfo($"RF-kill status:\n{rfkillStatus.Output}");

                await executor.ExecuteCommandAsync(NetworkConfigurationsCommands.UnblockAllWifiDevicesBySoftware(), ignoreErrors: true);
                await executor.ExecuteCommandAsync(NetworkConfigurationsCommands.UnblockAllWirelessDevicesBySoftware(), ignoreErrors: true);
                await Task.Delay(1000);

                rfkillStatus = await executor.ExecuteCommandAsync(NetworkConfigurationsCommands.ShowAllWirelessDevicesBlockState(), ignoreErrors: true);

                if (rfkillStatus.Output.Contains("Soft blocked: yes") || rfkillStatus.Output.Contains("Hard blocked: yes"))
                {
                    logger.LogError("⚠️ WiFi still blocked by RF-kill");
                    logger.LogInfo("💡 If you have a physical WiFi switch, please activate it manually");
                    throw new Exception("WiFi blocked by RF-kill");
                }
                else
                {
                    logger.LogInfo("✅ RF-kill unblocked successfully");
                    return new ExecutionResult(0, "RF-kill unblocked successfully.", string.Empty);
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"⚠️ Error checking RF-kill: {ex.Message}");
                throw new Exception("WiFi blocked by RF-kill");
            }
        }
    }
}