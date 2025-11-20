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
    public async Task<bool> IsDeviceConnectedAsync(string deviceId)
    {
        var device = await _deviceRepository.GetByMacAddressAsync(deviceId);
        if (device == null)
        {
            return false;
        }
        else
        {
            return await _sessionRepository.GetByMacAddressAsync(device.MacAddress) != null;
        }
    }

    public async Task ConnectDeviceAsync(string deviceId)
    {
        var device = await _deviceRepository.GetByMacAddressAsync(deviceId);
        if (device == null)
        {
            //Pending implementation
        }

    }

    public async Task DisconnectDeviceAsync(string deviceId)
    {
        var device = await _deviceRepository.GetByMacAddressAsync(deviceId);
        if (device != null)
        {
            var session = await _sessionRepository.GetByMacAddressAsync(device.MacAddress);
            if (session != null)
            {
                await _sessionRepository.DeleteAsync(session.DeviceMacAddress, session.Username);
                await _deviceRepository.DeleteAsync(device.MacAddress);
            }
        }
    }
}