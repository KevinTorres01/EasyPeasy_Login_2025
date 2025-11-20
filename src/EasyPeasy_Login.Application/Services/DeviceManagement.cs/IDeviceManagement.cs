using EasyPeasy_Login.Application.DTOs;
namespace EasyPeasy_Login.Application.Services.DeviceManagement;
interface IDeviceManagement
{
    Task<bool> IsDeviceConnectedAsync(string deviceId);
    Task ConnectDeviceAsync(string deviceId);
    Task DisconnectDeviceAsync(string deviceId);
    
}