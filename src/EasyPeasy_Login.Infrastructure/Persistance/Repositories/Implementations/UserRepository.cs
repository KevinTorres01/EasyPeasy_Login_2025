
using EasyPeasy_Login.Domain.Exceptions;

namespace EasyPeasy_Login.Infrastructure.Data.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(string path) : base(path)
    {
        if (!Items.Any())
        {
            var defaultUser = new User
            (
                "admin",
                //should be hashed by a proper hashing function that is not implemented yet
                "admin123");
            ;
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
        user.CreatedAt = DateTime.UtcNow;
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