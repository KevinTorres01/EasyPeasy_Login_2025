using System.IO;

namespace EasyPeasy_Login.Shared.Constants;

public static class PersistenceConstants
{
    // Store JSON data under the project root's data/ directory for consistent access.
    // We navigate up from the bin directory to find the project root.
    private static readonly string DataDirectory = GetDataDirectory();

    private static string GetDataDirectory()
    {
        // First, try to find the data directory relative to the current working directory (project root)
        var currentDir = Directory.GetCurrentDirectory();
        var dataPath = Path.Combine(currentDir, "data");
        
        if (Directory.Exists(dataPath))
        {
            return dataPath;
        }
        
        // Fallback: Navigate up from BaseDirectory to find project root
        var baseDir = AppContext.BaseDirectory;
        var dir = new DirectoryInfo(baseDir);
        
        // Walk up the directory tree looking for a data folder or solution file
        while (dir != null)
        {
            var potentialDataDir = Path.Combine(dir.FullName, "data");
            if (Directory.Exists(potentialDataDir))
            {
                return potentialDataDir;
            }
            
            // Check if we found the solution file (we're at project root)
            if (File.Exists(Path.Combine(dir.FullName, "EasyPeasy_Login.sln")))
            {
                var projectDataDir = Path.Combine(dir.FullName, "data");
                Directory.CreateDirectory(projectDataDir);
                return projectDataDir;
            }
            
            dir = dir.Parent;
        }
        
        // Final fallback: create data directory next to BaseDirectory
        Directory.CreateDirectory(dataPath);
        return dataPath;
    }

    public static readonly string UserDataStoragePath = Path.Combine(DataDirectory, "Users.json");
    public static readonly string SessionsDataStoragePath = Path.Combine(DataDirectory, "Sessions.json");
    public static readonly string DevicesDataStoragePath = Path.Combine(DataDirectory, "Devices.json");
    public static readonly string AllowedDevicesDataStoragePath = Path.Combine(DataDirectory, "allowed_devices.json");
    public static readonly string BackupDirectory = Path.Combine(DataDirectory, "backup");
} 