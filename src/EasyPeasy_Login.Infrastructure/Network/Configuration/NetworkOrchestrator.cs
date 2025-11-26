
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace EasyPeasy_Login.Infrastructure.Network.Configuration
{
    public class NetworkOrchestrator : INetworkOrchestrator
    {
        private readonly IDnsmasqManager dnsmasqManager;
        private readonly IHostapdManager hostapdManager;
        private readonly INetworkManager networkManager;
        private readonly ICaptivePortalControlManager captivePortalManager;
        private readonly ILogger logger;

        public NetworkOrchestrator(IDnsmasqManager dnsmasq, IHostapdManager hostapd, INetworkManager network, ILogger logger, ICaptivePortalControlManager captivePortal)
        {
            dnsmasqManager = dnsmasq;
            hostapdManager = hostapd;
            networkManager = network;
            this.logger = logger;
            captivePortalManager = captivePortal;
        }

        public async Task<bool> SetUpNetwork()
        {
            try
            {
                logger.LogInfo("🔧 Starting Access Point configuration with Captive Portal...");

                NetworkConfigurationDefaults._upstreamInterface = await networkManager.DetectUpstreamInterface();

                if (string.IsNullOrEmpty(NetworkConfigurationDefaults._upstreamInterface))
                {
                    logger.LogWarning("⚠️ No upstream interface with Internet detected.");
                    logger.LogInfo("💡 Connect your phone via USB or ethernet before continuing.");
                    Console.Write("Do you want to continue without Internet? (y/n): ");
                    await RestoreConfiguration();
                    return false;
                }
                else
                {
                    logger.LogInfo($"✅ Internet interface detected: {NetworkConfigurationDefaults._upstreamInterface}");
                    if (NetworkConfigurationDefaults._isVpnInterface)
                    {
                        logger.LogWarning("⚠️ This is a VPN interface. Special configuration will be applied.");
                    }
                }

                logger.LogInfo("🔧 Checking RF-kill block...");
                await networkManager.UnblockRfkill();

                logger.LogInfo("🔧 Configuring NetworkManager...");

                await networkManager.ConfigureNetworkInterface();

                logger.LogInfo($"✅ Interface {NetworkConfigurationDefaults._interface} configured with IP {NetworkConfigurationDefaults._gatewayIp}");

                await hostapdManager.ConfigureHostapdAsync();
                await hostapdManager.StartHostapdAsync();

                logger.LogInfo("✅ Hostapd started");

                await networkManager.EnableIpPacketForwarding();
                logger.LogInfo("✅ IP Forwarding enabled");

                await dnsmasqManager.ConfigureDnsmasqAsync();
                await dnsmasqManager.StartDnsmasqAsync();
                logger.LogInfo("✅ Dnsmasq started");
                await Task.Delay(2000);

                // Configure Captive Portal with iptables
                if (!string.IsNullOrEmpty(NetworkConfigurationDefaults._upstreamInterface))
                {
                    await captivePortalManager.ConfigureCaptivePortal();
                }

                await dnsmasqManager.ValidateDnsConfiguration();

                ShowFinalResume();
                return true;
            }
            catch (Exception ex)
            {
                logger.LogInfo($"❌ Error configuring network: {ex.Message}");
                logger.LogInfo($"Stack Trace: {ex.StackTrace}");
                await RestoreConfiguration();
                return false;
            }
        }

        public async void Dispose()
        {
            await RestoreConfiguration();
        }

        private void ShowFinalResume()
        {
            logger.LogInfo("\n========================================");
            logger.LogInfo("✅ Captive Portal configured successfully");
            logger.LogInfo($"📡 SSID: {NetworkConfigurationDefaults._ssid}");
            logger.LogInfo($"🔑 Password: {NetworkConfigurationDefaults._password}");
            logger.LogInfo($"🌐 Gateway: {NetworkConfigurationDefaults._gatewayIp}");
            logger.LogInfo($"🌐 DHCP Range: {NetworkConfigurationDefaults._dhcpRange}");
            logger.LogInfo($"🔒 Portal: http://{NetworkConfigurationDefaults._gatewayIp}:{NetworkConfigurationDefaults.DefaultPort}/portal");
            if (!string.IsNullOrEmpty(NetworkConfigurationDefaults._upstreamInterface))
            {
                logger.LogInfo($"🌍 Internet shared from: {NetworkConfigurationDefaults._upstreamInterface}");
                if (NetworkConfigurationDefaults._isVpnInterface)
                {
                    logger.LogInfo($"🔒 Traffic routed through VPN");
                }
            }
            logger.LogInfo("========================================\n");
        }

        public async Task RestoreConfiguration()
        {
            logger.LogInfo("\n🔄 Restoring network configuration...");

            await hostapdManager.StopHostapdAsync();
            await dnsmasqManager.StopDnsmasqAsync();
            await captivePortalManager.RestoreCaptivePortalConfiguration();
            await networkManager.RestoreNetworkInterfaceConfiguration();

            Console.WriteLine("✅ Configuration restored\n");
        }
    }
}