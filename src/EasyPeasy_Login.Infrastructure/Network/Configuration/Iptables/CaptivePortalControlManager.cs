
namespace EasyPeasy_Login.Infrastructure.Network.Configuration
{
    public class CaptivePortalControlManager : ICaptivePortalControlManager
    {
        private readonly ICommandExecutor executor;
        private readonly ILogger logger;

        public CaptivePortalControlManager(ICommandExecutor commandExecutor, ILogger logger)
        {
            executor = commandExecutor;
            this.logger = logger;
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
                logger.LogInfo($"🔧 Allowing access to the web portal (port {NetworkConfigurationDefaults.DefaultPort})...");
                await executor.ExecuteCommandAsync(IptablesCommands.AllowAccessToPortal(NetworkConfigurationDefaults._interface, NetworkConfigurationDefaults.DefaultPort));

            // ICMP (ping)
            await AllowIcmp();

            // HTTP/HTTPS redirect to the portal (AFTER allowing the portal port)
            logger.LogInfo($"🔧 Redirecting HTTP/HTTPS to the portal...");
            await RedirectToPortal();

            await CreateAndConfigureCustomChainForAuthenticatedUsers();

            // NAT/MASQUERADE (only if there is an upstream interface)
            if (!string.IsNullOrEmpty(NetworkConfigurationDefaults._upstreamInterface))
            {
                logger.LogInfo($"🔧 Configuring NAT to {NetworkConfigurationDefaults._upstreamInterface}...");
                await executor.ExecuteCommandAsync(IptablesCommands.ConfigureNat(NetworkConfigurationDefaults._upstreamInterface));

                if (NetworkConfigurationDefaults._isVpnInterface)
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
            await executor.ExecuteCommandAsync(IptablesCommands.InterceptUdpDnsTrafficAndRedirectToThisDevice(NetworkConfigurationDefaults._interface));
            await executor.ExecuteCommandAsync(IptablesCommands.InterceptTcpDnsTrafficAndRedirectToThisDevice(NetworkConfigurationDefaults._interface));
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
            await executor.ExecuteCommandAsync(IptablesCommands.AllowDnsGettingInToThisDeviceUdp(NetworkConfigurationDefaults._interface));
            await executor.ExecuteCommandAsync(IptablesCommands.AllowDnsGettingInToThisDeviceTcp(NetworkConfigurationDefaults._interface));

        }

        private async Task AllowDhcp()
        {
            logger.LogInfo("🔧 Allowing DHCP...");
            await executor.ExecuteCommandAsync(IptablesCommands.AllowDhcpGettingIn(NetworkConfigurationDefaults._interface));
            await executor.ExecuteCommandAsync(IptablesCommands.AllowDhcpGettingOut(NetworkConfigurationDefaults._interface));
        }

        private async Task AllowIcmp()
        {
            await executor.ExecuteCommandAsync(IptablesCommands.AllowIcmpGettingInToThisDevice(NetworkConfigurationDefaults._interface));
            await executor.ExecuteCommandAsync(IptablesCommands.AllowIcmpPassingThroughThisDevice());
        }

        private async Task RedirectToPortal()
        {
            logger.LogInfo($"🔧 Redirecting HTTP/HTTPS to the portal...");
            await executor.ExecuteCommandAsync(IptablesCommands.RedirectHttpTrafficToPortal(NetworkConfigurationDefaults._interface, NetworkConfigurationDefaults._gatewayIp, NetworkConfigurationDefaults.DefaultPort));
            await executor.ExecuteCommandAsync(IptablesCommands.RedirectHttpsTrafficToPortal(NetworkConfigurationDefaults._interface, NetworkConfigurationDefaults._gatewayIp, NetworkConfigurationDefaults.DefaultPort));
        }

        private async Task CreateAndConfigureCustomChainForAuthenticatedUsers()
        {
            // Custom chain for authenticated users
            logger.LogInfo("🔧 Creating chain for authenticated users...");
            await executor.ExecuteCommandAsync(IptablesCommands.CreatePersonalizedChainForAuthenticatedUser(), ignoreErrors: true);
            await executor.ExecuteCommandAsync(IptablesCommands.CleanPersonalizedChainForAuthenticatedUser(), ignoreErrors: true);

            // By default, if a user reaches here they are NOT authenticated → DROP
            await executor.ExecuteCommandAsync(IptablesCommands.RedirectTrafficPassingThroughToCustomChainForAuthenticatedUsers(NetworkConfigurationDefaults._interface));
            await executor.ExecuteCommandAsync(IptablesCommands.ForbidPacketsPassingThroughThisDevice(NetworkConfigurationDefaults._interface));
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