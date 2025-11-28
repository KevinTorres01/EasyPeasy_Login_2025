using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EasyPeasy_Login.Shared;

namespace EasyPeasy_Login.Infrastructure.Network.Configuration
{
    public class HostapdManager : IHostapdManager
    {
        private readonly ICommandExecutor executor;
        private readonly INetworkConfiguration config;
        private readonly ILogger logger;

        private string GetHostapdConfig() => $@"interface={config.Interface}
driver=nl80211
ssid={config.Ssid}
hw_mode=g
channel=6
ieee80211n=1
wmm_enabled=1
macaddr_acl=0
auth_algs=1
ignore_broadcast_ssid=0
wpa=2
wpa_passphrase={config.Password}
wpa_key_mgmt=WPA-PSK
wpa_pairwise=CCMP
rsn_pairwise=CCMP
";
        public HostapdManager(ICommandExecutor executor, ILogger logger, INetworkConfiguration networkConfiguration)
        {
            this.executor = executor;
            this.logger = logger;
            this.config = networkConfiguration;
        }
        public async Task<ExecutionResult> ConfigureHostapdAsync()
        {
            logger.LogInfo("🔧 Creating access point with hostapd...");

            // Stop any running hostapd instance first (ignore errors if none running)
            await executor.ExecuteCommandAsync(HostapdCommands.StopHostapd(), ignoreErrors: true);
            await Task.Delay(1000);

            // Backup existing configuration if present. We'll store backups in the repo's data/backup folder.
            try
            {
                var backupDir = Path.Combine(Directory.GetCurrentDirectory(), "data", "backup");
                Directory.CreateDirectory(backupDir);

                var systemConfigPath = "/etc/hostapd/hostapd.conf";
                if (File.Exists(systemConfigPath))
                {
                    var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
                    var backupPath = Path.Combine(backupDir, $"hostapd.conf.{timestamp}");
                    File.Copy(systemConfigPath, backupPath, overwrite: true);
                    // keep a copy named 'hostapd.conf.latest' for easy lookup
                    File.Copy(systemConfigPath, Path.Combine(backupDir, "hostapd.conf.latest"), overwrite: true);
                    logger.LogInfo($"Saved existing hostapd.conf to {backupPath}");
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to backup existing hostapd.conf: {ex.Message}");
                // proceed even if backup failed
            }

            string hostapdConfig = GetHostapdConfig();

            await File.WriteAllTextAsync("/etc/hostapd/hostapd.conf", hostapdConfig);

            logger.LogInfo("✅ Hostapd configuration created successfully");
            return new ExecutionResult(0, "Hostapd configuration created successfully.", string.Empty);
        }

        public async Task<ExecutionResult> StartHostapdAsync()
        {
            var result = await executor.ExecuteCommandWithOutput(HostapdCommands.StartHostapd("/etc/hostapd/hostapd.conf"));
            logger.LogInfo($"Hostapd output: {result.Output}");

            await Task.Delay(3000);

            var hostapdStatus = await executor.ExecuteCommandAsync(HostapdCommands.GetHostapdPid(), ignoreErrors: true);
            if (string.IsNullOrWhiteSpace(hostapdStatus.Output))
            {
                logger.LogError("❌ Hostapd did not start correctly. Run 'sudo hostapd /etc/hostapd/hostapd.conf' manually to see errors.");
                throw new Exception("Hostapd did not start correctly. Run 'sudo hostapd /etc/hostapd/hostapd.conf' manually to see errors.");
            }

            logger.LogInfo("✅ Hostapd started successfully");
            return new ExecutionResult(0, "Hostapd is running.", string.Empty);
        }

        public async Task<ExecutionResult> StopHostapdAsync()
        {
            logger.LogInfo("🛑 Stopping hostapd...");

            // Try to stop hostapd (ignore errors if it's not running)
            await executor.ExecuteCommandAsync(HostapdCommands.StopHostapd(), ignoreErrors: true);
            await Task.Delay(1000);

            // Attempt to restore the most recent backup from data/backup
            try
            {
                var backupDir = Path.Combine(Directory.GetCurrentDirectory(), "data", "backup");
                if (Directory.Exists(backupDir))
                {
                    // Prefer timestamped backups (hostapd.conf.YYYYMMDDHHMMSS), otherwise use hostapd.conf.latest
                    var backups = Directory.GetFiles(backupDir, "hostapd.conf.*")
                        .Where(p => !p.EndsWith("hostapd.conf.latest"))
                        .OrderByDescending(p => p)
                        .ToList();

                    string? restoreFile = null;
                    if (backups.Any())
                    {
                        restoreFile = backups.First();
                    }
                    else
                    {
                        var latest = Path.Combine(backupDir, "hostapd.conf.latest");
                        if (File.Exists(latest)) restoreFile = latest;
                    }

                    if (!string.IsNullOrWhiteSpace(restoreFile) && File.Exists(restoreFile))
                    {
                        File.Copy(restoreFile, "/etc/hostapd/hostapd.conf", overwrite: true);
                        logger.LogInfo($"✅ Restored hostapd.conf from {restoreFile}");
                        return new ExecutionResult(0, "Hostapd stopped and configuration restored.", string.Empty);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to restore hostapd configuration from backup: {ex.Message}");
                return new ExecutionResult(1, "Hostapd stopped but failed to restore configuration from backup.", ex.Message);
            }

            logger.LogInfo("Hostapd stopped. No backup found to restore.");
            return new ExecutionResult(0, "Hostapd stopped. No backup found to restore.", string.Empty);
        }
    }
}