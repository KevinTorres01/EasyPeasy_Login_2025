namespace EasyPeasy_Login.Domain.Interfaces;

public interface IDeviceRepository:IRepository<Device>
{
    Task<Device?> GetByMacAddressAsync(string macAddress);
    Task<Device?> GetByIpAddressAsync(string ipAddress);
    Task DeleteAsync(string macAddress);
}