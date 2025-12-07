using EasyPeasy_Login.Domain.Interfaces;

public interface IUserRepository:IRepository<User>
{
    Task<User?> GetByUsernameAsync(string username);
    Task DeleteByUsernameAsync(string username);
    
}