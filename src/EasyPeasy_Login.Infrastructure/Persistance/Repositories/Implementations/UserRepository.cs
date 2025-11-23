
using EasyPeasy_Login.Domain.Exceptions;
using EasyPeasy_Login.Shared.Constants;

namespace EasyPeasy_Login.Infrastructure.Data.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository() : base(PersistenceConstants.UserDataStoragePath)
    {
        if (!Items.Any())
        {
            User defaultUser = new User("admin", "$2a$11$OlHPfN0V9EcZwDZ2NhvzaOT0E6F8/EfWo2wHzJhSFEVEwd7fqBkCa"); // Username = admin, Password = admin05
            Items.Add(defaultUser);
            SaveDataAsync().Wait();
        }
    }

    public Task<User?> GetByUsernameAsync(string username)
    {
        return Task.FromResult(Items.FirstOrDefault(u =>
            u.Username.Equals(username)));
    }

    public Task<IEnumerable<User>> GetAllAsync()
    {
        return Task.FromResult<IEnumerable<User>>(Items);
    }

    public async Task AddAsync(User user)
    {
        if (Items.Any(u => u.Username.Equals(user.Username)))
        {
            throw new InvalidCredentialsException($"User '{user.Username}' already exists");
        }
        Items.Add(user);
        await SaveDataAsync();
    }

    public async Task UpdateAsync(User user)
    {
        var index = Items.FindIndex(u => u.Username == user.Username);
        if (index >= 0)
        {
            Items[index] = user;
            await SaveDataAsync();
        }
        else
        {
            throw new UserNotFoundException(user.Username);
        }
    }

    public async Task DeleteByUsernameAsync(string username)
    {
        var user = Items.FirstOrDefault(u => u.Username.Equals(username));
        if (user != null)
        {
            Items.Remove(user);
            await SaveDataAsync();
        }
        else
        {
            throw new UserNotFoundException(username);
        }
    }
}