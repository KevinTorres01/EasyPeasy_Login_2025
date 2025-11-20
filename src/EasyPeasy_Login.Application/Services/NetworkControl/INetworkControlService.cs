public interface INetworkControlService
{
    Task<bool> IsDeviceAllowedAsync(string macAddress, string ipAddress);
    Task AllowDeviceAsync(string macAddress, string ipAddress, string username);
    Task BlockDeviceAsync(string macAddress);

}