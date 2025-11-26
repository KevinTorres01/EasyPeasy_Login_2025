using EasyPeasy_Login.Domain.Interfaces;
namespace EasyPeasy_Login.Application.Services.DeviceManagement;

public class DeviceManagement : IDeviceManagement
{
    IDeviceRepository _deviceRepository;
    ISessionRepository _sessionRepository;

    public DeviceManagement(IDeviceRepository deviceRepository, ISessionRepository sessionRepository)
    {
        _deviceRepository = deviceRepository;
        _sessionRepository = sessionRepository;
    }
    public async Task<bool> IsDeviceAuthenticatedAsync(string macAddress)
    {
        var device = await _deviceRepository.GetByMacAddressAsync(macAddress);
        if (device == null)
        {
            return false;
        }
        else
        {
            return await _sessionRepository.GetByMacAddressAsync(device.MacAddress) != null;
        }
    }

    public async Task AuthenticateDeviceAsync(string macAddress, string ipAddress)
    {
        var device = await _deviceRepository.GetByMacAddressAsync(macAddress);
        if (device == null)
        {
            var newDevice = new Device(
                macAddress,
                ipAddress
            );
            await _deviceRepository.AddAsync(newDevice);
        }
    }

    public async Task DisconnectDeviceAsync(string macAddress)
    {
        var device = await _deviceRepository.GetByMacAddressAsync(macAddress);
        if (device != null)
        {
            var session = await _sessionRepository.GetByMacAddressAsync(device.MacAddress);
            if (session != null)
            {
                await _sessionRepository.DeleteAsync(session.DeviceMacAddress, session.Username);
            }
        }
    }

    public async Task<IEnumerable<Device>> GetAllDevicesAsync()
    {
        return await _deviceRepository.GetAllAsync();
    }
}