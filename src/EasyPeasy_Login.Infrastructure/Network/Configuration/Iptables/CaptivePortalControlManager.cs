using EasyPeasy_Login.Shared;

namespace EasyPeasy_Login.Infrastructure.Network.Configuration
{
    public class CaptivePortalControlManager : ICaptivePortalControlManager
    {
        private readonly ICommandExecutor executor;
        private readonly INetworkConfiguration config;
        private readonly ILogger logger;

        public CaptivePortalControlManager(ICommandExecutor commandExecutor, ILogger logger, INetworkConfiguration networkConfiguration)
        {
            executor = commandExecutor;
            this.logger = logger;
            this.config = networkConfiguration;
        }
        public async Task ConfigureCaptivePortal()
        {
            logger.LogInfo($"🔒 Configuring Captive Portal...");

            await CleanExistentRules();

            // CRITICAL: DNS redirect before default policy
            await ConfigureDnsRedirect();

            // Verify rules creation for DNS redirect
            var natCheck = await executor.ExecuteCommandAsync(IptablesCommands.ListAllRulesFromPreroutingNatTable(), ignoreErrors: true);
            logger.LogInfo($"📋 PREROUTING NAT Rules:\n{natCheck}");

                if (!natCheck.Output.Contains("dpt:53"))
                {
                    logger.LogError("❌ Failed to create DNS redirect rules");
                    throw new Exception("❌ Failed to create DNS redirect rules");
                }
                logger.LogInfo("✅ DNS redirect configured (UDP/TCP port 53)");

            await ConfigureDefaultPolicies();

            await ConfigureLoopbackTraffic();

            await ConfigurePoliciesForEstablishedConnections();

            // CRITICAL: Allow DNS getting in
            await AllowDnsGettingIntoGateway();

            await AllowDhcp();

                // Access to the web portal (port 8080)
                logger.LogInfo($"🔧 Allowing access to the web portal (port {config.DefaultPort})...");
                await executor.ExecuteCommandAsync(IptablesCommands.AllowAccessToPortal(config.Interface, config.DefaultPort));

            // ICMP (ping)
            await AllowIcmp();

            // HTTP/HTTPS redirect to the portal (AFTER allowing the portal port)
            logger.LogInfo($"🔧 Redirecting HTTP/HTTPS to the portal...");
            await RedirectToPortal();

            await CreateAndConfigureCustomChainForAuthenticatedUsers();

            // NAT/MASQUERADE (only if there is an upstream interface)
            if (!string.IsNullOrEmpty(config.UpstreamInterface))
            {
                logger.LogInfo($"🔧 Configuring NAT to {config.UpstreamInterface}...");
                await executor.ExecuteCommandAsync(IptablesCommands.ConfigureNat(config.UpstreamInterface));

                if (config.IsVpnInterface)
                {
                    await executor.ExecuteCommandAsync(IptablesCommands.OptimizesTcpSegmentSize(), ignoreErrors: true);
                }
            }

                logger.LogInfo("✅ Captive Portal fully configured");
        }

        private async Task CleanExistentRules()
        {
            logger.LogInfo("🧹 Cleaning up iptables rules...");
            await executor.ExecuteCommandAsync(IptablesCommands.CleanFirewallFilterTable(), ignoreErrors: true);
            await executor.ExecuteCommandAsync(IptablesCommands.CleanFirewallNatTable(), ignoreErrors: true);
            await executor.ExecuteCommandAsync(IptablesCommands.CleanFirewallMangleTable(), ignoreErrors: true);
            await executor.ExecuteCommandAsync(IptablesCommands.CleanFirewallPersonalizedChains(), ignoreErrors: true);
            await executor.ExecuteCommandAsync(IptablesCommands.CleanFirewallNatTablePersonalizedChains(), ignoreErrors: true);
            await executor.ExecuteCommandAsync(IptablesCommands.CleanFirewallMangleTablePersonalizedChains(), ignoreErrors: true);
        }

        private async Task ConfigureDnsRedirect()
        {
            logger.LogInfo("🔧 Configuring DNS redirect...");
            await executor.ExecuteCommandAsync(IptablesCommands.InterceptUdpDnsTrafficAndRedirectToThisDevice(config.Interface));
            await executor.ExecuteCommandAsync(IptablesCommands.InterceptTcpDnsTrafficAndRedirectToThisDevice(config.Interface));
        }

        private async Task ConfigureDefaultPolicies()
        {
            logger.LogInfo("🔧 Configuring default policies...");
            await executor.ExecuteCommandAsync(IptablesCommands.AllowPacketsGettingInToThisDevice());
            await executor.ExecuteCommandAsync(IptablesCommands.ForbidPacketsPassingThroughThisDevice());
            await executor.ExecuteCommandAsync(IptablesCommands.AllowPacketsGettingOutFromThisDevice());
        }

        private async Task ConfigureLoopbackTraffic()
        {
            await executor.ExecuteCommandAsync(IptablesCommands.AllowLoopbackPacketsGettingIn());
            await executor.ExecuteCommandAsync(IptablesCommands.AllowLoopbackPacketsGettingOut());
        }

        private async Task ConfigurePoliciesForEstablishedConnections()
        {
            await executor.ExecuteCommandAsync(IptablesCommands.AllowTrafficFromEstablishedOrRelatedConnectionsGettingInToThisDevice());
            await executor.ExecuteCommandAsync(IptablesCommands.AllowTrafficFromEstablishedOrRelatedConnectionsGettingOutFromThisDevice());
            await executor.ExecuteCommandAsync(IptablesCommands.AllowTrafficFromEstablishedOrRelatedConnectionsPassingThroughThisDevice());
        }

        private async Task AllowDnsGettingIntoGateway()
        {
            logger.LogInfo("🔧 Allowing DNS queries to the gateway...");
            await executor.ExecuteCommandAsync(IptablesCommands.AllowDnsGettingInToThisDeviceUdp(config.Interface));
            await executor.ExecuteCommandAsync(IptablesCommands.AllowDnsGettingInToThisDeviceTcp(config.Interface));

        }

        private async Task AllowDhcp()
        {
            logger.LogInfo("🔧 Allowing DHCP...");
            await executor.ExecuteCommandAsync(IptablesCommands.AllowDhcpGettingIn(config.Interface));
            await executor.ExecuteCommandAsync(IptablesCommands.AllowDhcpGettingOut(config.Interface));
        }

        private async Task AllowIcmp()
        {
            await executor.ExecuteCommandAsync(IptablesCommands.AllowIcmpGettingInToThisDevice(config.Interface));
            await executor.ExecuteCommandAsync(IptablesCommands.AllowIcmpPassingThroughThisDevice());
        }

        private async Task RedirectToPortal()
        {
            logger.LogInfo($"🔧 Redirecting HTTP/HTTPS to the portal...");
            await executor.ExecuteCommandAsync(IptablesCommands.RedirectHttpTrafficToPortal(config.Interface, config.GatewayIp, config.DefaultPort));
            await executor.ExecuteCommandAsync(IptablesCommands.RedirectHttpsTrafficToPortal(config.Interface, config.GatewayIp, config.DefaultPort));
        }

        private async Task CreateAndConfigureCustomChainForAuthenticatedUsers()
        {
            // Custom chain for authenticated users
            logger.LogInfo("🔧 Creating chain for authenticated users...");
            await executor.ExecuteCommandAsync(IptablesCommands.CreatePersonalizedChainForAuthenticatedUser(), ignoreErrors: true);
            await executor.ExecuteCommandAsync(IptablesCommands.CleanPersonalizedChainForAuthenticatedUser(), ignoreErrors: true);

            // By default, if a user reaches here they are NOT authenticated → DROP
            await executor.ExecuteCommandAsync(IptablesCommands.RedirectTrafficPassingThroughToCustomChainForAuthenticatedUsers(config.Interface));
            await executor.ExecuteCommandAsync(IptablesCommands.ForbidPacketsPassingThroughThisDevice(config.Interface));
        }

        public async Task RestoreCaptivePortalConfiguration()
        {
            await CleanExistentRules();

            // Restore default policies
            await executor.ExecuteCommandAsync(IptablesCommands.AllowPacketsGettingInToThisDevice());
            await executor.ExecuteCommandAsync(IptablesCommands.AllowPacketsPassingThroughThisDevice());
            await executor.ExecuteCommandAsync(IptablesCommands.AllowPacketsGettingOutFromThisDevice());
        }
    }
}