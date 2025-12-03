
using System.Text.Json;
using EasyPeasy_Login.Domain.Interfaces;
using EasyPeasy_Login.Application.Services.NetworkControl;

public class NetworkControlService : INetworkControlService
{
    private const string AllowedDevicesFile = "allowed_devices.json";
    private List<Device> _allowedDevices;
    private readonly IDeviceRepository _deviceRepository;
    private readonly ISessionRepository _sessionRepository;
    private readonly IFirewallService _firewallService;


    public NetworkControlService(
        IDeviceRepository deviceRepository,
        ISessionRepository sessionRepository,
        IFirewallService firewallService
    )
    {
        _deviceRepository = deviceRepository;
        _sessionRepository = sessionRepository;
        _firewallService = firewallService;
        _allowedDevices = LoadAllowedDevices();
    }

    private List<Device> LoadAllowedDevices()
    {
        if (File.Exists(AllowedDevicesFile))
        {
            var json = File.ReadAllText(AllowedDevicesFile);
            return JsonSerializer.Deserialize<List<Device>>(json) ?? new List<Device>();
        }
        return new List<Device>();
    }

    public async Task AllowDeviceAsync(string macAddress, string ipAddress, string username)
    {
        var device = await _deviceRepository.GetByMacAddressAsync(macAddress);
        if (device != null)
        {
            device.IPAddress = ipAddress;
            await _deviceRepository.UpdateAsync(device);

            // Grant internet access via iptables
            await _firewallService.GrantInternetAccessAsync(macAddress);

            if (!_allowedDevices.Any(d => d.MacAddress == macAddress))
            {
                _allowedDevices.Add(device);
                var json = JsonSerializer.Serialize(_allowedDevices, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
                await File.WriteAllTextAsync(AllowedDevicesFile, json);
            }
        }
        else
        {
            throw new KeyNotFoundException("Device not found.");
        }
    }

    public async Task BlockDeviceAsync(string macAddress)
    {
        var device = await _deviceRepository.GetByMacAddressAsync(macAddress);
        if (device != null)
        {
            // Revoke internet access via iptables
            await _firewallService.RevokeInternetAccessAsync(macAddress);

            await _deviceRepository.UpdateAsync(device);

            _allowedDevices.RemoveAll(d => d.MacAddress == macAddress);
            var json = JsonSerializer.Serialize(_allowedDevices, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            await File.WriteAllTextAsync(AllowedDevicesFile, json);
        }
        else
        {
            throw new KeyNotFoundException("Device not found.");
        }
    }

    public async Task<bool> IsDeviceAllowedAsync(string macAddress, string ipAddress)
    {
        // Check both local cache AND iptables
        var isInLocalCache = _allowedDevices.Any(d => d.MacAddress == macAddress && d.IPAddress == ipAddress);
        var hasFirewallAccess = await _firewallService.HasInternetAccessAsync(macAddress);
        
        return isInLocalCache || hasFirewallAccess;
    }
}