
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using EasyPeasy_Login.Shared;

namespace EasyPeasy_Login.Infrastructure.Network.Configuration
{
    public class NetworkOrchestrator : INetworkOrchestrator
    {
        private readonly IDnsmasqManager dnsmasqManager;
        private readonly IHostapdManager hostapdManager;
        private readonly INetworkManager networkManager;
        private readonly ICaptivePortalControlManager captivePortalManager;
        private readonly INetworkConfiguration config;
        private readonly ILogger logger;

        public NetworkOrchestrator( IDnsmasqManager dnsmasq, IHostapdManager hostapd, INetworkManager network, ILogger logger, ICaptivePortalControlManager captivePortal, INetworkConfiguration networkConfiguration)
        {
            dnsmasqManager = dnsmasq;
            hostapdManager = hostapd;
            networkManager = network;
            this.logger = logger;
            captivePortalManager = captivePortal;
            config = networkConfiguration;
        }

        public async Task<bool> SetUpNetwork()
        {
            try
            {
                logger.LogInfo("🔧 Starting Access Point configuration with Captive Portal...");

                config.UpstreamInterface = await networkManager.DetectUpstreamInterface();

                if (string.IsNullOrEmpty(config.UpstreamInterface))
                {
                    logger.LogWarning("⚠️ No upstream interface with Internet detected.");
                    logger.LogInfo("💡 Connect your phone via USB or ethernet before continuing.");
                    await RestoreConfiguration();
                    return false;
                }
                else
                {
                    logger.LogInfo($"✅ Internet interface detected: {config.UpstreamInterface}");
                    if (config.IsVpnInterface)
                    {
                        logger.LogWarning("⚠️ This is a VPN interface. Special configuration will be applied.");
                    }
                }

                logger.LogInfo("🔧 Checking RF-kill block...");
                await networkManager.UnblockRfkill();

                logger.LogInfo("🔧 Configuring NetworkManager...");

                await networkManager.ConfigureNetworkInterface();

                logger.LogInfo($"✅ Interface {config.Interface} configured with IP {config.GatewayIp}");

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
                if (!string.IsNullOrEmpty(config.UpstreamInterface))
                {
                    await captivePortalManager.ConfigureCaptivePortal();
                }

                await dnsmasqManager.ValidateDnsConfiguration();

                config.IsNetworkActive = true;
                ShowFinalResume();
                return true;
            }
            catch (Exception ex)
            {
                logger.LogInfo($"❌ Error configuring network: {ex.Message}");
                logger.LogInfo($"Stack Trace: {ex.StackTrace}");
                config.IsNetworkActive = false;
                await RestoreConfiguration();
                return false;
            }
        }

        private void ShowFinalResume()
        {
            logger.LogInfo("\n========================================");
            logger.LogInfo("✅ Captive Portal configured successfully");
            logger.LogInfo($"📡 SSID: {config.Ssid}");
            logger.LogInfo($"🔑 Password: {config.Password}");
            logger.LogInfo($"🌐 Gateway: {config.GatewayIp}");
            logger.LogInfo($"🌐 DHCP Range: {config.DhcpRange}");
            logger.LogInfo($"🔒 Portal: http://{config.GatewayIp}:{config.DefaultPort}/portal");
            if (!string.IsNullOrEmpty(config.UpstreamInterface))
            {
                logger.LogInfo($"🌍 Internet shared from: {config.UpstreamInterface}");
                if (config.IsVpnInterface)
                {
                    logger.LogInfo($"🔒 Traffic routed through VPN");
                }
            }
            logger.LogInfo("========================================\n");
        }

        public async Task RestoreConfiguration()
        {
            logger.LogInfo("\n🔄 Restoring network configuration...");

            await networkManager.RestoreNetworkInterfaceConfiguration();
            await hostapdManager.StopHostapdAsync();
            await dnsmasqManager.StopDnsmasqAsync();
            await captivePortalManager.RestoreCaptivePortalConfiguration();
            await networkManager.RestartNetworkInterface();
            
            config.ResetRuntimeState();

            logger.LogInfo("✅ Configuration restored\n");
        }
    }
}