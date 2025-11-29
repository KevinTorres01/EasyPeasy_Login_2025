using System.Text.Json;

namespace EasyPeasy_Login.Infrastructure.Data;

public abstract class Repository<T> where T : class
{
    protected readonly string FilePath;
    protected List<T> Items = new();
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private readonly object _lock = new();

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

    /// <summary>
    /// Thread-safe read operation
    /// </summary>
    protected List<T> GetItemsSnapshot()
    {
        lock (_lock)
        {
            return Items.ToList();
        }
    }

    /// <summary>
    /// Thread-safe add operation
    /// </summary>
    protected void AddItemSafe(T item)
    {
        lock (_lock)
        {
            Items.Add(item);
        }
    }

    /// <summary>
    /// Thread-safe remove operation
    /// </summary>
    protected bool RemoveItemSafe(T item)
    {
        lock (_lock)
        {
            return Items.Remove(item);
        }
    }

    /// <summary>
    /// Thread-safe update operation
    /// </summary>
    protected void UpdateItemSafe(int index, T item)
    {
        lock (_lock)
        {
            if (index >= 0 && index < Items.Count)
            {
                Items[index] = item;
            }
        }
    }

    /// <summary>
    /// Thread-safe find index operation
    /// </summary>
    protected int FindIndexSafe(Predicate<T> predicate)
    {
        lock (_lock)
        {
            return Items.FindIndex(predicate);
        }
    }

    protected async Task SaveDataAsync()
    {
        await _semaphore.WaitAsync();
        try
        {
            List<T> itemsToSave;
            lock (_lock)
            {
                itemsToSave = Items.ToList();
            }

            var json = JsonSerializer.Serialize(itemsToSave, new JsonSerializerOptions
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
