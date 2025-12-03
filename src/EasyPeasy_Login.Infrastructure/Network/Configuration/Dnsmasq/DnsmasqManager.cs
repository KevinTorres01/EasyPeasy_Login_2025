using System;
using System.IO;
using System.Threading.Tasks;
using EasyPeasy_Login.Shared;

namespace EasyPeasy_Login.Infrastructure.Network.Configuration
{
    public class DnsmasqManager : IDnsmasqManager
    {
        private readonly ICommandExecutor executor;
        private readonly INetworkConfiguration config;
        private readonly ILogger logger;

        public DnsmasqManager(ICommandExecutor executor, ILogger logger, INetworkConfiguration networkConfiguration)
        {
            this.executor = executor;
            this.logger = logger;
            this.config = networkConfiguration;
        }

        private string GetDnsmasqConfig() => $@"# Interfaces - ONLY listen on the captive portal interface, NOT localhost
interface={config.Interface}
bind-interfaces
except-interface=lo

# Listen ONLY on gateway IP (not on 127.0.0.1 to avoid affecting server's own DNS)
listen-address={config.GatewayIp}

# DNS Configuration
no-resolv
server=8.8.8.8
server=8.8.4.4
no-poll
no-hosts

# DHCP Range
dhcp-range={config.DhcpRange},12h
dhcp-option=3,{config.GatewayIp}     # Default Gateway
dhcp-option=6,{config.GatewayIp}     # DNS Server

# DNS Spoofing: All domains resolve to the gateway (only affects clients on {config.Interface})
address=/#/{config.GatewayIp}

# Disable IPv6 to avoid REFUSED errors
filter-AAAA

# Authoritative mode (respond as authoritative)
dhcp-authoritative

# Logging (for debugging)
log-queries
log-dhcp
log-facility=/var/log/dnsmasq-captive.log

# Performance
cache-size=150
";

        public async Task<ExecutionResult> ConfigureDnsmasqAsync()
        {
            logger.LogInfo("🔧 Configuring DHCP (dnsmasq) with DNS spoofing...");

            // CRITICAL: Stop conflicting services
            await executor.ExecuteCommandAsync(DnsmasqCommands.StopDnsSystemResolver(), ignoreErrors: true);
            await executor.ExecuteCommandAsync(DnsmasqCommands.StopDnsmasq(), ignoreErrors: true);

            // Prevent systemd-resolved from restarting automatically
            await executor.ExecuteCommandAsync(DnsmasqCommands.DisableCompletelyDnsSystemResolverByMasking(), ignoreErrors: true);
            await Task.Delay(2000);

            // CRITICAL: Backup and configure server's DNS BEFORE starting dnsmasq
            // This ensures the server can still resolve DNS externally
            var resolvBackupExists = File.Exists("/etc/resolv.conf.backup");
            if (!resolvBackupExists)
            {
                await executor.ExecuteCommandAsync(DnsmasqCommands.BackupResolvConf(), ignoreErrors: true);
            }
            await executor.ExecuteCommandAsync(DnsmasqCommands.ConfigureServerDns(), ignoreErrors: true);
            logger.LogInfo("✅ Server DNS configured to use external resolvers (8.8.8.8, 8.8.4.4, 1.1.1.1)");

            string dnsmasqConfig = GetDnsmasqConfig();

            await File.WriteAllTextAsync("/etc/dnsmasq.d/captive-portal.conf", dnsmasqConfig);

            // Clear global configuration that might interfere
            var backupExists = File.Exists("/etc/dnsmasq.conf.backup");
            if (!backupExists)
            {
                await executor.ExecuteCommandAsync(DnsmasqCommands.CreateBackupOfConfigFile(), ignoreErrors: true);
            }

            // Create minimal configuration
            await File.WriteAllTextAsync("/etc/dnsmasq.conf", "conf-dir=/etc/dnsmasq.d/,*.conf\n");

            return new ExecutionResult(0, "DHCP configuration created successfully.", string.Empty);
        }

        public async Task<ExecutionResult> StartDnsmasqAsync()
        {
            await executor.ExecuteCommandAsync(DnsmasqCommands.StartDnsmasq());
            await Task.Delay(3000);

            var status = await executor.ExecuteCommandAsync(DnsmasqCommands.CheckActivityStatusOfDnsmasq(), ignoreErrors: true);
            if (!status.Output.Trim().Equals("active", StringComparison.OrdinalIgnoreCase))
            {
                var journalctl = await executor.ExecuteCommandAsync(DnsmasqCommands.ShowLastLogsOfDnsmasq(30), ignoreErrors: true);
                logger.LogError($"⚠️ dnsmasq error logs:\n{journalctl}");
                throw new Exception("dnsmasq did not start correctly");
            }
            else
            {
                logger.LogInfo("✅ DHCP configured with DNS spoofing active");

                // Test DNS spoofing
                await Task.Delay(1000);
                var testDns = await executor.ExecuteCommandAsync(DnsmasqCommands.MakeDnsRequestUsingGatewayDnsServer("google.com", config.GatewayIp), ignoreErrors: true);
                logger.LogInfo($"🧪 DNS spoofing test:\n{testDns}");
                return new ExecutionResult(0, "dnsmasq is running with DNS spoofing.", string.Empty);
            }
        }

        public async Task<ExecutionResult> StopDnsmasqAsync()
        {
            logger.LogInfo("🛑 Stopping dnsmasq and restoring configuration...");

            // Try to stop dnsmasq first (ignore errors if it's not running)
            await executor.ExecuteCommandAsync(DnsmasqCommands.StopDnsmasq(), ignoreErrors: true);
            await Task.Delay(1000);

            try
            {
                // Restore systemd-resolved so the system DNS works again
                await executor.ExecuteCommandAsync(DnsmasqCommands.UnmaskDnsSystemResolver(), ignoreErrors: true);
                await executor.ExecuteCommandAsync(DnsmasqCommands.StartDnsSystemResolver(), ignoreErrors: true);
                // Restore the server's resolv.conf if we backed it up
                if (File.Exists("/etc/resolv.conf.backup"))
                {
                    await executor.ExecuteCommandAsync(DnsmasqCommands.RestoreResolvConf(), ignoreErrors: true);
                    logger.LogInfo("✅ Restored /etc/resolv.conf from backup");
                }

                // If we created a backup of the global dnsmasq.conf, restore it
                if (File.Exists("/etc/dnsmasq.conf.backup"))
                {
                    await executor.ExecuteCommandAsync(DnsmasqCommands.RestoreConfigFileFromBackup(), ignoreErrors: true);
                    logger.LogInfo("Restored /etc/dnsmasq.conf from /etc/dnsmasq.conf.backup");

                    // Remove our captive config and logs
                    await executor.ExecuteCommandAsync(DnsmasqCommands.RemoveConfigOrLogFile("/etc/dnsmasq.d/captive-portal.conf"), ignoreErrors: true);
                    await executor.ExecuteCommandAsync(DnsmasqCommands.RemoveConfigOrLogFile("/var/log/dnsmasq-captive.log"), ignoreErrors: true);

                    // Restart dnsmasq so it loads the restored config
                    await executor.ExecuteCommandAsync(DnsmasqCommands.RestartDnsmasq(), ignoreErrors: true);

                    return new ExecutionResult(0, "dnsmasq stopped and configuration restored.", string.Empty);
                }

                // No backup found: just remove our captive files and leave dnsmasq stopped
                await executor.ExecuteCommandAsync(DnsmasqCommands.RemoveConfigOrLogFile("/etc/dnsmasq.d/captive-portal.conf"), ignoreErrors: true);
                await executor.ExecuteCommandAsync(DnsmasqCommands.RemoveConfigOrLogFile("/var/log/dnsmasq-captive.log"), ignoreErrors: true);

                logger.LogInfo("dnsmasq stopped. No backup found to restore.");
                return new ExecutionResult(0, "dnsmasq stopped. No backup found to restore.", string.Empty);
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to stop/restore dnsmasq: {ex.Message}");
                return new ExecutionResult(1, "Failed to stop/restore dnsmasq", ex.Message);
            }
        }

        public async Task<ExecutionResult> ValidateDnsConfiguration()
        {
            logger.LogInfo("\n🧪 Validating DNS configuration...");

            // Test 1: Verify that systemd-resolved is NOT running
            var resolvedStatus = await executor.ExecuteCommandAsync(DnsmasqCommands.CheckActivityStatusOfDnsSystemResolver(), ignoreErrors: true);
            if (resolvedStatus.Output.Trim() == "active")
            {
                logger.LogWarning("⚠️ systemd-resolved is still active, forcing stop...");
                await executor.ExecuteCommandAsync(DnsmasqCommands.StopDnsSystemResolver(), ignoreErrors: true);
                await executor.ExecuteCommandAsync(DnsmasqCommands.DisableCompletelyDnsSystemResolverByMasking(), ignoreErrors: true);
                await Task.Delay(2000);
            }

            // Test 2: dnsmasq is listening on the correct port
            var netstat = await executor.ExecuteCommandAsync(NetworkConfigurationsCommands.GetAllProcessListeningOnPort(53), ignoreErrors: true);
            logger.LogInfo($"Services on port 53:\n{netstat.Output}");

            if (!netstat.Output.Contains($"{config.GatewayIp}:53"))
            {
                logger.LogError($"❌ dnsmasq is NOT listening on {config.GatewayIp}:53");
                throw new Exception($"❌ dnsmasq is NOT listening on {config.GatewayIp}:53");
            }

            // Test 3: Resolve a domain from the gateway
            var nslookup = await executor.ExecuteCommandAsync(DnsmasqCommands.MakeDnsRequestUsingGatewayDnsServer("google.com", config.GatewayIp));
            Console.WriteLine($"Test DNS (nslookup):\n{nslookup.Output}");

            if (!nslookup.Output.Contains($"Address: {config.GatewayIp}") || nslookup.Output.Contains("REFUSED"))
            {
                logger.LogWarning("⚠️ DNS spoofing is not responding correctly");

                // Additional debug
                var dnsmasqLog = await executor.ExecuteCommandAsync(DnsmasqCommands.ShowLastLogsOfDnsmasqInCaptivePortal(20), ignoreErrors: true);
                logger.LogInfo($"Last dnsmasq logs:\n{dnsmasqLog.Output}");
            }

            // Test 4: Verify iptables NAT DNS redirect
            var iptablesNat = await executor.ExecuteCommandAsync(IptablesCommands.ShowNatTablePreroutingChainInfo(), ignoreErrors: true);
            logger.LogInfo($"NAT PREROUTING rules:\n{iptablesNat.Output}");

            if (!iptablesNat.Output.Contains("dpt:53"))
            {
                throw new Exception("❌ DNS redirect rules NOT found in iptables NAT");
            }

            // Test 5: Verify INPUT allows DNS
            var iptablesInput = await executor.ExecuteCommandAsync(IptablesCommands.ShowFirewalRulesOnInputChainRelatedToPort(53), ignoreErrors: true);
            logger.LogInfo($"INPUT rules for DNS:\n{iptablesInput.Output}");

            logger.LogInfo("✅ DNS configuration validated successfully\n");
            
            return new ExecutionResult(0, "DNS validated successfully", string.Empty);
        }
    }
}