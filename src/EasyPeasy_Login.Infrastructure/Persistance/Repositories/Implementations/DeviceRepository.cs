using EasyPeasy_Login.Domain.Interfaces;
using EasyPeasy_Login.Shared.Constants;

namespace EasyPeasy_Login.Infrastructure.Data.Repositories;

public class DeviceRepository : Repository<Device>, IDeviceRepository
{
    public DeviceRepository() : base(PersistenceConstants.DevicesDataStoragePath) { }

    public Task<Device?> GetByMacAddressAsync(string macAddress)
        => Task.FromResult(Items.FirstOrDefault(d => d.MacAddress == macAddress));

    public Task<Device?> GetByIpAddressAsync(string ipAddress)
        => Task.FromResult(Items.FirstOrDefault(d => d.IPAddress == ipAddress));

    public override async Task AddAsync(Device entity)
    {
        if (Items.Any(d => d.MacAddress == entity.MacAddress))
            throw new InvalidOperationException("Device with this MAC address already exists.");

        Items.Add(entity);
        await SaveDataAsync();
    }

    public override async Task UpdateAsync(Device entity)
    {
        var index = Items.FindIndex(d => d.MacAddress == entity.MacAddress);
        if (index < 0)
            throw new KeyNotFoundException("Device not found for update.");

        Items[index] = entity;
        await SaveDataAsync();
    }

    public async Task DeleteAsync(string macAddress)
    {
        var device = Items.FirstOrDefault(d => d.MacAddress == macAddress);
        if (device != null)
        {
            Items.Remove(device);
            await SaveDataAsync();
        }
    }
}