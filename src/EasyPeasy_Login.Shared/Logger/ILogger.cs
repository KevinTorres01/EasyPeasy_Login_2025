namespace EasyPeasy_Login.Shared;

public interface ILogger
{
    void LogInfo(string message);
    void LogWarning(string message);
    void LogError(string message);
    
    /// <summary>
    /// Event fired when a new log entry is added.
    /// Subscribe to this event to receive real-time log updates in the UI.
    /// </summary>
    event Action<LogEntry>? OnLogAdded;
    
    /// <summary>
    /// Gets all log entries in the current session.
    /// </summary>
    IReadOnlyList<LogEntry> GetLogs();
    
    /// <summary>
    /// Clears all logs from the current session.
    /// </summary>
    void ClearLogs();
}

public record LogEntry(DateTime Timestamp, LogLevel Level, string Message);

public enum LogLevel
{
    Info,
    Warning,
    Error
}