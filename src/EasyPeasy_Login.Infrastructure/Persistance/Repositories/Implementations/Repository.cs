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
        EnsureFileExists();
        LoadData();
    }
     private void EnsureFileExists()
    {
        var directory = Path.GetDirectoryName(FilePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        if (!File.Exists(FilePath))
        {
            File.WriteAllText(FilePath, "[]");
        }
        else
        {
            // Check if file is empty or contains only whitespace
            var content = File.ReadAllText(FilePath);
            if (string.IsNullOrWhiteSpace(content))
            {
                File.WriteAllText(FilePath, "[]");
            }
        }
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
