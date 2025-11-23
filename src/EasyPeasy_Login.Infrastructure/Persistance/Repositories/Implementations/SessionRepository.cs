using EasyPeasy_Login.Domain.Interfaces;
using EasyPeasy_Login.Shared.Constants;

namespace EasyPeasy_Login.Infrastructure.Data.Repositories;

public class SessionRepository : Repository<Session>, ISessionRepository
{

    public SessionRepository() : base(PersistenceConstants.SessionsDataStoragePath) { }


    public Task<Session?> GetByMacAddressAsync(string macAddress)
    {
        return Task.FromResult(Items.FirstOrDefault(s => s.DeviceMacAddress == macAddress));
    }

    public Task<IEnumerable<Session>> GetByUsernameAsync(string username)
    {
        return Task.FromResult<IEnumerable<Session>>(Items.Where(s => s.Username == username));
    }

    public Task DeleteAsync(string macAddress, string username)
    {
        var session = Items.FirstOrDefault(s => s.DeviceMacAddress == macAddress && s.Username == username);
        if (session != null)
        {
            Items.Remove(session);
            return SaveDataAsync();
        }
        return Task.CompletedTask;
    }

    public Task<IEnumerable<Session>> GetAllAsync()
    {
       return Task.FromResult<IEnumerable<Session>>(Items);
    }

    public Task AddAsync(Session entity)
    {
        var existingSession = Items.FirstOrDefault(s => s.DeviceMacAddress == entity.DeviceMacAddress && s.Username == entity.Username);
        if (existingSession != null)
        {
            throw new InvalidOperationException("Session already exists for this device and user.");
        }
        Items.Add(entity);
        return SaveDataAsync();
    }

    public Task UpdateAsync(Session entity)
    {
        var index = Items.FindIndex(s => s.DeviceMacAddress == entity.DeviceMacAddress && s.Username == entity.Username);
        if (index >= 0)
        {
            Items[index] = entity;
            return SaveDataAsync();
        }
        throw new KeyNotFoundException("Session not found for update.");
    }
}