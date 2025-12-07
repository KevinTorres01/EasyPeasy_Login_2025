using EasyPeasy_Login.Application.DTOs;
namespace EasyPeasy_Login.Application.Services.DeviceManagement;

public interface IDeviceManagement
{
    Task<bool> IsDeviceAuthenticatedAsync(string macAddress);
    Task AuthenticateDeviceAsync(string macAddress, string ipAddress);
    Task DisconnectDeviceAsync(string macAddress);
    Task<IEnumerable<Device>> GetAllDevicesAsync();
    Task<IEnumerable<DeviceSessionDto>> GetConnectedDevicesAsync();

}