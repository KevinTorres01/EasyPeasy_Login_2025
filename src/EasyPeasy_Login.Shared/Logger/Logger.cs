using EasyPeasy_Login.Shared;

public class Logger : ILogger
{
    private readonly List<LogEntry> _logs = new();
    private readonly object _lock = new();
    
    public event Action<LogEntry>? OnLogAdded;

    public void LogInfo(string message)
    {
        AddLog(LogLevel.Info, message);
        Console.WriteLine($"INFO: {message}");
    }

    public void LogWarning(string message)
    {
        AddLog(LogLevel.Warning, message);
        Console.WriteLine($"WARNING: {message}");
    }

    public void LogError(string message)
    {
        AddLog(LogLevel.Error, message);
        Console.WriteLine($"ERROR: {message}");
    }
    
    public IReadOnlyList<LogEntry> GetLogs()
    {
        lock (_lock)
        {
            return _logs.ToList().AsReadOnly();
        }
    }
    
    public void ClearLogs()
    {
        lock (_lock)
        {
            _logs.Clear();
        }
    }
    
    private void AddLog(LogLevel level, string message)
    {
        var entry = new LogEntry(DateTime.Now, level, message);
        lock (_lock)
        {
            _logs.Add(entry);
            // Keep only last 500 logs to prevent memory issues
            if (_logs.Count > 500)
            {
                _logs.RemoveAt(0);
            }
        }
        OnLogAdded?.Invoke(entry);
    }
}