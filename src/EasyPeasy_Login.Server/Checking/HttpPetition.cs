namespace EasyPeasy_Login.Server.Checking;

/// <summary>
/// Represents a parsed HTTP request received by the captive portal server.
/// This class extracts essential information from raw HTTP requests to handle
/// captive portal detection and user authentication flows.
/// </summary>
public class HttpPetition
{
    // ===== Core HTTP properties (essential for any HTTP handling) =====
    
    /// <summary>HTTP method (GET, POST, etc.)</summary>
    public string Method { get; set; } = "";
    
    /// <summary>Request path (e.g., "/login", "/generate_204")</summary>
    public string Path { get; set; } = "";
    
    /// <summary>Target host from the Host header</summary>
    public string Host { get; set; } = "";
    
    /// <summary>Request body content (used for POST data like login credentials)</summary>
    public string Body { get; set; } = "";
    
    /// <summary>Client's IP address (used for device identification and access control)</summary>
    public string ClientIP { get; set; } = "";
    
    /// <summary>
    /// User-Agent header - CRITICAL for captive portal detection.
    /// Operating systems send specific User-Agents when checking for captive portals:
    /// - iOS/macOS: "CaptiveNetworkSupport"
    /// - Android: "Dalvik" or "okhttp"  
    /// - Windows: "Microsoft NCSI"
    /// We must detect these to respond appropriately and trigger the login popup.
    /// </summary>
    public string UserAgent { get; set; } = "";

    /// <summary>
    /// Parses a raw HTTP request string into an HttpPetition object.
    /// Extracts method, path, host, user-agent and body from the raw request.
    /// </summary>
    /// <param name="rawRequest">The complete raw HTTP request as received from the socket</param>
    /// <param name="clientIP">The IP address of the client making the request</param>
    public static HttpPetition Parse(string rawRequest, string clientIP)
    {
        var petition = new HttpPetition { ClientIP = clientIP };
        var lines = rawRequest.Split("\r\n");

        // Parse the request line (e.g., "GET /path HTTP/1.1")
        if (lines.Length > 0)
        {
            var firstLine = lines[0].Split(' ');
            if (firstLine.Length >= 2)
            {
                petition.Method = firstLine[0];
                petition.Path = firstLine[1];
            }
        }

        // Parse headers - we only extract what we actually need
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
        }

        // Extract body (everything after the empty line that separates headers from body)
        int bodyIndex = rawRequest.IndexOf("\r\n\r\n");
        if (bodyIndex >= 0)
        {
            petition.Body = rawRequest.Substring(bodyIndex + 4);
        }

        return petition;
    }

    /// <summary>
    /// Checks if the request comes from localhost.
    /// Useful for allowing admin access or debugging without authentication.
    /// </summary>
    public bool IsFromLocalhost()
    {
        return ClientIP == "127.0.0.1" || ClientIP == "::1" || ClientIP == "localhost";
    }

    /// <summary>
    /// Detects if this request is from a captive portal detector (automatic OS check)
    /// vs a real browser (user navigation).
    /// 
    /// WHY THIS MATTERS:
    /// When a device connects to WiFi, the OS automatically checks for internet access.
    /// If we detect it's a captive portal check, we must respond with a redirect or
    /// specific response to trigger the login popup. If we respond normally, the popup
    /// won't appear and the user won't know they need to login.
    /// 
    /// Detection endpoints by OS:
    /// - iOS/macOS: captive.apple.com/hotspot-detect.html (User-Agent: CaptiveNetworkSupport)
    /// - Android: connectivitycheck.gstatic.com/generate_204 (User-Agent: Dalvik)
    /// - Windows: www.msftconnecttest.com/connecttest.txt (User-Agent: Microsoft NCSI)
    /// </summary>
    public bool IsCaptivePortalDetection()
    {
        string ua = UserAgent.ToLower();

        // iOS/macOS captive portal detection
        if (ua.Contains("captivenetworksupport") || ua.Contains("wispr"))
            return true;

        // Android captive portal detection (Dalvik VM or OkHttp client)
        if (ua.Contains("dalvik") || (ua.Contains("android") && ua.Contains("okhttp")))
            return true;

        // Windows NCSI (Network Connectivity Status Indicator)
        // Empty User-Agent can also indicate automated connectivity checks
        if (ua.Contains("microsoft ncsi") || string.IsNullOrEmpty(UserAgent))
            return true;

        return false;
    }
}
