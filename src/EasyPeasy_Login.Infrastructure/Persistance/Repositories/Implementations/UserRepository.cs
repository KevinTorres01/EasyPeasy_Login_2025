using EasyPeasy_Login.Domain.Exceptions;
using EasyPeasy_Login.Domain.Interfaces;
using EasyPeasy_Login.Shared.Constants;

namespace EasyPeasy_Login.Infrastructure.Data.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository() : base(PersistenceConstants.UserDataStoragePath)
    {
        if (!Items.Any())
        {
            // Create default admin: Username = admin, Password = admin05
            Items.Add(new User("admin", "Administrator", "$2a$11$OlHPfN0V9EcZwDZ2NhvzaOT0E6F8/EfWo2wHzJhSFEVEwd7fqBkCa"));
            SaveDataAsync().Wait();
        }
    }

    public Task<User?> GetByUsernameAsync(string username)
        => Task.FromResult(Items.FirstOrDefault(u => u.Username.Equals(username)));

    public override async Task AddAsync(User user)
    {
        if (Items.Any(u => u.Username.Equals(user.Username)))
            throw new InvalidCredentialsException($"User '{user.Username}' already exists");

        Items.Add(user);
        await SaveDataAsync();
    }

    public override async Task UpdateAsync(User user)
    {
        var index = Items.FindIndex(u => u.Username == user.Username);
        if (index < 0)
            throw new UserNotFoundException(user.Username);

        Items[index] = user;
        await SaveDataAsync();
    }

    public async Task DeleteByUsernameAsync(string username)
    {
        var user = Items.FirstOrDefault(u => u.Username.Equals(username))
            ?? throw new UserNotFoundException(username);

        Items.Remove(user);
        await SaveDataAsync();
    }
}