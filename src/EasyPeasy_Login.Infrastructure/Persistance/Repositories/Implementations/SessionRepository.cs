using EasyPeasy_Login.Domain.Interfaces;
using EasyPeasy_Login.Shared.Constants;
using System.Net.NetworkInformation;

namespace EasyPeasy_Login.Infrastructure.Data.Repositories;

public class SessionRepository : Repository<Session>, ISessionRepository
{
    public SessionRepository() : base(PersistenceConstants.SessionsDataStoragePath)
    {
        if (Items.Any())
        {
            Items.Clear();
            SaveDataAsync().Wait();
            //anadir admin session por defecto si es necesario con mac del dispositivo actual y arreglar errores de compilacion
            Items.Add(new Session(GetCurrentDeviceMacAddress(), "admin"));
            SaveDataAsync().Wait();
        }
    }

    private string GetCurrentDeviceMacAddress()
    {
        var nic = NetworkInterface.GetAllNetworkInterfaces()
            .FirstOrDefault(n => n.OperationalStatus == OperationalStatus.Up 
                              && n.NetworkInterfaceType != NetworkInterfaceType.Loopback);

        return nic?.GetPhysicalAddress().ToString().ToLower() 
               ?? "00:00:00:00:00:00";
    }

    public Task<Session?> GetByMacAddressAsync(string macAddress)
        => Task.FromResult(Items.FirstOrDefault(s => s.DeviceMacAddress == macAddress));

    public Task<IEnumerable<Session>> GetByUsernameAsync(string username)
        => Task.FromResult<IEnumerable<Session>>(Items.Where(s => s.Username == username));

    public override async Task AddAsync(Session entity)
    {
        if (Items.Any(s => s.DeviceMacAddress == entity.DeviceMacAddress && s.Username == entity.Username))
            throw new InvalidOperationException("Session already exists for this device and user.");

        Items.Add(entity);
        await SaveDataAsync();
    }

    public override async Task UpdateAsync(Session entity)
    {
        var index = Items.FindIndex(s => s.DeviceMacAddress == entity.DeviceMacAddress && s.Username == entity.Username);
        if (index < 0)
            throw new KeyNotFoundException("Session not found for update.");

        Items[index] = entity;
        await SaveDataAsync();
    }

    public async Task DeleteAsync(string macAddress, string username)
    {
        var session = Items.FirstOrDefault(s => s.DeviceMacAddress == macAddress && s.Username == username);
        if (session != null)
        {
            Items.Remove(session);
            await SaveDataAsync();
        }
    }
}