using EasyPeasy_Login.Application.DTOs;
using EasyPeasy_Login.Domain.Interfaces;
using EasyPeasy_Login.Application.Services.NetworkControl;

namespace EasyPeasy_Login.Application.Services.DeviceManagement;

public class DeviceManagement : IDeviceManagement
{
    IDeviceRepository _deviceRepository;
    ISessionRepository _sessionRepository;
    INetworkControlService _networkControlService;

    public DeviceManagement(IDeviceRepository deviceRepository, ISessionRepository sessionRepository, INetworkControlService networkControlService)
    {
        _deviceRepository = deviceRepository;
        _sessionRepository = sessionRepository;
        _networkControlService = networkControlService;
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
        else
        {
            // Device already exists, update its IP if it changed
            if (device.IPAddress != ipAddress)
            {
                device.IPAddress = ipAddress;
                await _deviceRepository.UpdateAsync(device);
            }
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
                // Remove session from repository
                await _sessionRepository.DeleteAsync(session.DeviceMacAddress, session.Username);
            }
            
            // Block device in iptables (remove internet access)
            await _networkControlService.BlockDeviceAsync(macAddress);
        }
    }

    public async Task<IEnumerable<Device>> GetAllDevicesAsync()
    {
        return await _deviceRepository.GetAllAsync();
    }

    public async Task<IEnumerable<DeviceSessionDto>> GetConnectedDevicesAsync()
    {
        var devices = await _deviceRepository.GetAllAsync();
        var sessions = await _sessionRepository.GetAllAsync();
        
        var connectedDevices = new List<DeviceSessionDto>();
        
        foreach (var device in devices)
        {
            var session = sessions.FirstOrDefault(s => s.DeviceMacAddress == device.MacAddress);
            if (session != null)
            {
                connectedDevices.Add(new DeviceSessionDto
                {
                    IpAddress = session.IpAddress,
                    MacAddress = device.MacAddress,
                    Username = session.Username
                });
            }
        }
        
        return connectedDevices;
    }
}