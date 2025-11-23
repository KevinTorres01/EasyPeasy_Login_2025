using System.Text.Json;

namespace EasyPeasy_Login.Infrastructure.Data;

public abstract class Repository<T> where T : class
{
    protected readonly string FilePath;
    protected List<T> Items = new();
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    protected Repository(string filePath)
    {
        FilePath = filePath;
        LoadData();
    }

    protected void LoadData()
    {
        if (File.Exists(FilePath))
        {
            var json = File.ReadAllText(FilePath);
            try
            {
                Items = JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
            }
            catch(Exception e)
            {
                System.Console.WriteLine(e.Message);
                Items = new List<T>();
            }
        }
    }

    protected async Task SaveDataAsync()
    {
        await _semaphore.WaitAsync();
        try
        {
            var json = JsonSerializer.Serialize(Items, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            await File.WriteAllTextAsync(FilePath, json);
        }
        finally
        {
            _semaphore.Release();
        }
    }
}
