namespace EasyPeasy_Login.Server.Checking;

public class HttpPetition
{
    public string Method { get; set; } = "";
    public string Path { get; set; } = "";
    public string Host { get; set; } = "";
    public string Body { get; set; } = "";
    public string ClientIP { get; set; } = "";
    public string UserAgent { get; set; } = "";
    public string Accept { get; set; } = "";
    public string AcceptLanguage { get; set; } = "";
    public string Connection { get; set; } = "";

    public static HttpPetition Parse(string rawRequest, string clientIP)
    {
        var petition = new HttpPetition { ClientIP = clientIP };
        var lines = rawRequest.Split("\r\n");

        if (lines.Length > 0)
        {
            var firstLine = lines[0].Split(' ');
            if (firstLine.Length >= 2)
            {
                petition.Method = firstLine[0];
                petition.Path = firstLine[1];
            }
        }

        foreach (var line in lines)
        {
            if (line.StartsWith("Host:", StringComparison.OrdinalIgnoreCase))
            {
                petition.Host = line.Substring(5).Trim();
            }
            else if (line.StartsWith("User-Agent:", StringComparison.OrdinalIgnoreCase))
            {
                petition.UserAgent = line.Substring(11).Trim();
            }
            else if (line.StartsWith("Accept:", StringComparison.OrdinalIgnoreCase))
            {
                petition.Accept = line.Substring(7).Trim();
            }
            else if (line.StartsWith("Accept-Language:", StringComparison.OrdinalIgnoreCase))
            {
                petition.AcceptLanguage = line.Substring(16).Trim();
            }
            else if (line.StartsWith("Connection:", StringComparison.OrdinalIgnoreCase))
            {
                petition.Connection = line.Substring(11).Trim();
            }
        }

        int bodyIndex = rawRequest.IndexOf("\r\n\r\n");
        if (bodyIndex >= 0)
        {
            petition.Body = rawRequest.Substring(bodyIndex + 4);
        }

        return petition;
    }

    public bool IsFromLocalhost()
    {
        return ClientIP == "127.0.0.1" || ClientIP == "::1" || ClientIP == "localhost";
    }

    /// <summary>
    /// Detects the operating system from User-Agent
    /// </summary>
    public OSType DetectOS()
    {
        string ua = UserAgent.ToLower();

        // Apple devices
        if (ua.Contains("iphone") || ua.Contains("ipad") || ua.Contains("ipod"))
            return OSType.iOS;
        if (ua.Contains("mac os") || ua.Contains("macintosh"))
            return OSType.MacOS;

        // Android
        if (ua.Contains("android"))
            return OSType.Android;

        // Windows
        if (ua.Contains("windows"))
            return OSType.Windows;

        // Linux
        if (ua.Contains("linux"))
            return OSType.Linux;

        // ChromeOS
        if (ua.Contains("cros"))
            return OSType.ChromeOS;

        return OSType.Unknown;
    }

    /// <summary>
    /// Detects the browser from User-Agent
    /// </summary>
    public BrowserType DetectBrowser()
    {
        string ua = UserAgent.ToLower();

        // CaptiveNetworkSupport (Apple's captive portal detection)
        if (ua.Contains("captivenetworksupport") || ua.Contains("wispr"))
            return BrowserType.CaptivePortalDetector;

        // Android captive portal
        if (ua.Contains("dalvik") || (ua.Contains("android") && ua.Contains("okhttp")))
            return BrowserType.CaptivePortalDetector;

        // Windows NCSI
        if (ua.Contains("microsoft ncsi") || string.IsNullOrEmpty(ua))
            return BrowserType.CaptivePortalDetector;

        // Edge
        if (ua.Contains("edg/") || ua.Contains("edge"))
            return BrowserType.Edge;

        // Chrome (must check after Edge since Edge contains "Chrome")
        if (ua.Contains("chrome") && !ua.Contains("chromium"))
            return BrowserType.Chrome;

        // Firefox
        if (ua.Contains("firefox"))
            return BrowserType.Firefox;

        // Safari (must check after Chrome since Chrome contains "Safari")
        if (ua.Contains("safari") && !ua.Contains("chrome"))
            return BrowserType.Safari;

        // Opera
        if (ua.Contains("opera") || ua.Contains("opr/"))
            return BrowserType.Opera;

        return BrowserType.Unknown;
    }
}

public enum OSType
{
    Unknown,
    Windows,
    MacOS,
    iOS,
    Android,
    Linux,
    ChromeOS
}

public enum BrowserType
{
    Unknown,
    Chrome,
    Firefox,
    Safari,
    Edge,
    Opera,
    CaptivePortalDetector
}
