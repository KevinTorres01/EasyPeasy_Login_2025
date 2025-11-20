namespace EasyPeasy_Login.Domain.Interfaces;

public interface ISessionRepository:IRepository<Session>
{
    Task<Session?> GetByMacAddressAsync(string macAddress);
    Task<IEnumerable<Session>> GetByUsernameAsync(string username);
    Task DeleteAsync(string macAddress,string username);
}