
using EasyPeasy_Login.Domain.Interfaces;

namespace EasyPeasy_Login.Infrastructure.Data.Repositories;

public class DeviceRepository : Repository<Device>, IDeviceRepository
{
    public DeviceRepository() : base("devices.json") { }

    public Task AddAsync(Device entity)
    {
        var existingDevice = Items.FirstOrDefault(d => d.MacAddress == entity.MacAddress);
        if (existingDevice != null)
        {
            throw new InvalidOperationException("Device with this MAC address already exists.");
        }
        Items.Add(entity);
        return SaveDataAsync();
    }

    public Task DeleteAsync(string macAddress)
    {
        var device = Items.FirstOrDefault(d => d.MacAddress == macAddress);
        if (device != null)
        {
            Items.Remove(device);
            return SaveDataAsync();
        }
        return Task.CompletedTask;
    }

    public Task<IEnumerable<Device>> GetAllAsync()
    {
        return Task.FromResult<IEnumerable<Device>>(Items);
    }

    public Task<Device?> GetByIpAddressAsync(string ipAddress)
    {
        return Task.FromResult(Items.FirstOrDefault(d => d.IPAdress == ipAddress));
    }

    public Task<Device?> GetByMacAddressAsync(string macAddress)
    {
        return Task.FromResult(Items.FirstOrDefault(d => d.MacAddress == macAddress));
    }

    public Task UpdateAsync(Device entity)
    {
        var index = Items.FindIndex(d => d.MacAddress == entity.MacAddress);
        if (index >= 0)
        {
            Items[index] = entity;
            return SaveDataAsync();
        }
        throw new KeyNotFoundException("Device not found for update.");
    }
}