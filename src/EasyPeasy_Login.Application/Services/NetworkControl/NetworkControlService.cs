
using System.Text.Json;
using EasyPeasy_Login.Domain.Interfaces;

public class NetworkControlService : INetworkControlService
{
    private const string AllowedDevicesFile = "allowed_devices.json";
    private List<Device> _allowedDevices;
    private readonly IDeviceRepository _deviceRepository;
    private readonly ISessionRepository _sessionRepository;


    public NetworkControlService(
        IDeviceRepository deviceRepository,
        ISessionRepository sessionRepository
    )
    {
        _deviceRepository = deviceRepository;
        _sessionRepository = sessionRepository;
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
            device.IsBlocked = false;
            device.IPAddress = ipAddress;
            device.LastSeenAt = DateTime.UtcNow;
            await _deviceRepository.UpdateAsync(device);

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
            device.IsBlocked = true;
            device.LastSeenAt = DateTime.UtcNow;
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

    public Task<bool> IsDeviceAllowedAsync(string macAddress, string ipAddress)
    {
        var isAllowed = _allowedDevices.Any(d => d.MacAddress == macAddress && d.IPAddress == ipAddress);
        return Task.FromResult(isAllowed);
    }
}