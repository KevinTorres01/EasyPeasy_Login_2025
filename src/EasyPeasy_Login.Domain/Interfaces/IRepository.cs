namespace EasyPeasy_Login.Domain.Interfaces
{

    public interface IRepository<T>
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
    }

}