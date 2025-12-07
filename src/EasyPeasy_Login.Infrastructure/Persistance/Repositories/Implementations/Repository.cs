using System.Text.Json;
using EasyPeasy_Login.Domain.Interfaces;

namespace EasyPeasy_Login.Infrastructure.Data.Repositories;

/// <summary>
/// Base repository with thread-safe JSON file persistence.
/// </summary>
public abstract class Repository<T> : IRepository<T> where T : class
{
    protected readonly string FilePath;
    protected readonly List<T> Items = new();
    private readonly SemaphoreSlim _fileSemaphore = new(1, 1);

    protected Repository(string filePath)
    {
        FilePath = filePath;
        EnsureFileExists();
        LoadData();
    }

    private void EnsureFileExists()
    {
        var directory = Path.GetDirectoryName(FilePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        if (!File.Exists(FilePath) || string.IsNullOrWhiteSpace(File.ReadAllText(FilePath)))
            File.WriteAllText(FilePath, "[]");
    }

    private void LoadData()
    {
        try
        {
            var json = File.ReadAllText(FilePath);
            var items = JsonSerializer.Deserialize<List<T>>(json);
            if (items != null)
                Items.AddRange(items);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error loading {typeof(T).Name}: {e.Message}");
        }
    }

    #region IRepository<T> Implementation

    public Task<IEnumerable<T>> GetAllAsync()
        => Task.FromResult<IEnumerable<T>>(Items.ToList());

    public abstract Task AddAsync(T entity);

    public abstract Task UpdateAsync(T entity);

    #endregion

    #region Persistence

    protected async Task SaveDataAsync()
    {
        await _fileSemaphore.WaitAsync();
        try
        {
            var json = JsonSerializer.Serialize(Items, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(FilePath, json);
        }
        finally
        {
            _fileSemaphore.Release();
        }
    }

    #endregion
}
