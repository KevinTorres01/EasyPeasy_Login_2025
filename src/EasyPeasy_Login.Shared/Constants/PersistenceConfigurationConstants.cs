using System.IO;

namespace EasyPeasy_Login.Shared.Constants;

public static class PersistenceConstants
{
    // Store JSON data under a local data/ directory next to the running app.
    private static readonly string DataDirectory = Path.Combine(AppContext.BaseDirectory, "data");

    public static readonly string UserDataStoragePath = Path.Combine(DataDirectory, "Users.json");
    public static readonly string SessionsDataStoragePath = Path.Combine(DataDirectory, "Sessions.json");
    public static readonly string DevicesDataStoragePath = Path.Combine(DataDirectory, "Devices.json");
} 