using System;
using System.Threading.Tasks;
using EasyPeasy_Login.Application.DTOs;

namespace EasyPeasy_Login.Server.Checking;

/// <summary>
/// HttpServer - CaptivePortal: Handles OS connectivity checks for captive portal detection
/// </summary>
public partial class HttpServer
{
    /// <summary>
    /// Handles captive portal detection requests for different operating systems
    /// Returns null if not a captive portal detection request
    /// </summary>
    private async Task<string?> HandleCaptivePortalDetectionAsync(HttpPetition petition)
    {
        string path = petition.Path.ToLower();
        string host = petition.Host.ToLower();

        // ==================== APPLE iOS/macOS ====================
        if (IsAppleCaptivePortalRequest(path, host))
        {
            Console.WriteLine($"🍎 Apple Captive Portal Detection from {petition.ClientIP}");
            return await HandleCaptivePortalResponseAsync(petition, CaptivePortalType.Apple);
        }

        // ==================== ANDROID (Google) ====================
        if (IsAndroidCaptivePortalRequest(path, host))
        {
            Console.WriteLine($"🤖 Android Captive Portal Detection from {petition.ClientIP}");
            return await HandleCaptivePortalResponseAsync(petition, CaptivePortalType.Android);
        }

        // ==================== WINDOWS (NCSI) ====================
        if (IsWindowsCaptivePortalRequest(path, host))
        {
            Console.WriteLine($"🪟 Windows Captive Portal Detection from {petition.ClientIP}");
            return await HandleCaptivePortalResponseAsync(petition, CaptivePortalType.Windows);
        }

        // ==================== FIREFOX ====================
        if (IsFirefoxCaptivePortalRequest(path, host))
        {
            Console.WriteLine($"🦊 Firefox Captive Portal Detection from {petition.ClientIP}");
            return await HandleCaptivePortalResponseAsync(petition, CaptivePortalType.Firefox);
        }

        // ==================== CHROME/CHROMEOS ====================
        if (IsChromeCaptivePortalRequest(host))
        {
            Console.WriteLine($"🌐 Chrome Captive Portal Detection from {petition.ClientIP}");
            return await HandleCaptivePortalResponseAsync(petition, CaptivePortalType.Chrome);
        }

        // ==================== UBUNTU/LINUX ====================
        if (IsUbuntuCaptivePortalRequest(host))
        {
            Console.WriteLine($"🐧 Ubuntu/Linux Captive Portal Detection from {petition.ClientIP}");
            return await HandleCaptivePortalResponseAsync(petition, CaptivePortalType.Ubuntu);
        }

        // ==================== SAMSUNG ====================
        if (IsSamsungCaptivePortalRequest(host))
        {
            Console.WriteLine($"📱 Samsung Captive Portal Detection from {petition.ClientIP}");
            return await HandleCaptivePortalResponseAsync(petition, CaptivePortalType.Samsung);
        }

        // ==================== XIAOMI/MIUI ====================
        if (IsXiaomiCaptivePortalRequest(host))
        {
            Console.WriteLine($"📱 Xiaomi Captive Portal Detection from {petition.ClientIP}");
            return await HandleCaptivePortalResponseAsync(petition, CaptivePortalType.Xiaomi);
        }

        // ==================== HUAWEI ====================
        if (IsHuaweiCaptivePortalRequest(host))
        {
            Console.WriteLine($"📱 Huawei Captive Portal Detection from {petition.ClientIP}");
            return await HandleCaptivePortalResponseAsync(petition, CaptivePortalType.Huawei);
        }

        // ==================== AMAZON/KINDLE ====================
        if (IsAmazonCaptivePortalRequest(path, host))
        {
            Console.WriteLine($"📚 Amazon/Kindle Captive Portal Detection from {petition.ClientIP}");
            return await HandleCaptivePortalResponseAsync(petition, CaptivePortalType.Amazon);
        }

        // ==================== NINTENDO ====================
        if (IsNintendoCaptivePortalRequest(host))
        {
            Console.WriteLine($"🎮 Nintendo Captive Portal Detection from {petition.ClientIP}");
            return await HandleCaptivePortalResponseAsync(petition, CaptivePortalType.Nintendo);
        }

        // ==================== PLAYSTATION ====================
        if (IsPlayStationCaptivePortalRequest(host))
        {
            Console.WriteLine($"🎮 PlayStation Captive Portal Detection from {petition.ClientIP}");
            return await HandleCaptivePortalResponseAsync(petition, CaptivePortalType.PlayStation);
        }

        // ==================== GENERIC CONNECTIVITY CHECK ====================
        if (IsGenericConnectivityCheck(path))
        {
            Console.WriteLine($"🔍 Generic connectivity check from {petition.ClientIP}");
            return await HandleCaptivePortalResponseAsync(petition, CaptivePortalType.Generic);
        }

        return null;
    }

    #region Captive Portal Detection Methods

    private bool IsAppleCaptivePortalRequest(string path, string host)
    {
        return path == "/hotspot-detect.html" ||
               path == "/library/test/success.html" ||
               host.Contains("captive.apple.com") ||
               host.Contains("apple.com/library/test");
    }

    private bool IsAndroidCaptivePortalRequest(string path, string host)
    {
        return path == "/generate_204" ||
               path == "/gen_204" ||
               path.Contains("/generate204") ||
               host.Contains("connectivitycheck.gstatic.com") ||
               host.Contains("connectivitycheck.android.com") ||
               host.Contains("clients3.google.com") ||
               host.Contains("play.googleapis.com");
    }

    private bool IsWindowsCaptivePortalRequest(string path, string host)
    {
        return path == "/ncsi.txt" ||
               path == "/connecttest.txt" ||
               path == "/redirect" ||
               host.Contains("msftncsi.com") ||
               host.Contains("msftconnecttest.com") ||
               host.Contains("www.msftconnecttest.com") ||
               host.Contains("ipv6.msftconnecttest.com") ||
               host.Contains("dns.msftncsi.com");
    }

    private bool IsFirefoxCaptivePortalRequest(string path, string host)
    {
        return path == "/success.txt" ||
               path == "/canonical.html" ||
               host.Contains("detectportal.firefox.com");
    }

    private bool IsChromeCaptivePortalRequest(string host)
    {
        return host.Contains("clients1.google.com") ||
               host.Contains("clients.google.com") ||
               host.Contains("gstatic.com/generate_204");
    }

    private bool IsUbuntuCaptivePortalRequest(string host)
    {
        return host.Contains("connectivity-check.ubuntu.com") ||
               host.Contains("nmcheck.gnome.org") ||
               host.Contains("network-test.debian.org") ||
               host.Contains("204.pop-os.org");
    }

    private bool IsSamsungCaptivePortalRequest(string host)
    {
        return host.Contains("connectivitycheck.samsung.com") ||
               host.Contains("samsung.com/generate_204");
    }

    private bool IsXiaomiCaptivePortalRequest(string host)
    {
        return host.Contains("connect.rom.miui.com") ||
               host.Contains("connectivitycheck.miui.com") ||
               host.Contains("wifi.miui.com");
    }

    private bool IsHuaweiCaptivePortalRequest(string host)
    {
        return host.Contains("connectivitycheck.platform.hicloud.com") ||
               host.Contains("hicloud.com/generate_204") ||
               host.Contains("connectivitycheck.huawei.com");
    }

    private bool IsAmazonCaptivePortalRequest(string path, string host)
    {
        return host.Contains("spectrum.s3.amazonaws.com") ||
               path.Contains("wifistub.html") ||
               host.Contains("kindle-wifi") ||
               host.Contains("fireoscaptiveportal");
    }

    private bool IsNintendoCaptivePortalRequest(string host)
    {
        return host.Contains("conntest.nintendowifi.net") ||
               host.Contains("ctest.cdn.nintendo.net");
    }

    private bool IsPlayStationCaptivePortalRequest(string host)
    {
        return host.Contains("playstation.net") ||
               host.Contains("playstation.com/generate_204");
    }

    private bool IsGenericConnectivityCheck(string path)
    {
        return path.Contains("connectivity") ||
               path.Contains("check") ||
               path.Contains("network_test") ||
               path.Contains("internet") ||
               path.Contains("generate_204") ||
               path.Contains("gen_204");
    }

    #endregion

    #region Captive Portal Response Handler

    private enum CaptivePortalType
    {
        Apple,
        Android,
        Windows,
        Firefox,
        Chrome,
        Ubuntu,
        Samsung,
        Xiaomi,
        Huawei,
        Amazon,
        Nintendo,
        PlayStation,
        Generic
    }

    private async Task<string> HandleCaptivePortalResponseAsync(HttpPetition petition, CaptivePortalType portalType)
    {
        bool isAuthenticated = await IsClientAuthenticatedAsync(petition.ClientIP);

        if (isAuthenticated)
        {
            return BuildAuthenticatedResponse(portalType, petition.Path.ToLower());
        }
        else
        {
            return BuildCaptivePortalRedirect(petition);
        }
    }

    private string BuildAuthenticatedResponse(CaptivePortalType portalType, string path)
    {
        return portalType switch
        {
            // Apple expects specific HTML response
            CaptivePortalType.Apple =>
                BuildHtmlResponse(200, "OK", "<HTML><HEAD><TITLE>Success</TITLE></HEAD><BODY>Success</BODY></HTML>"),

            // Windows NCSI expects specific text
            CaptivePortalType.Windows when path == "/ncsi.txt" =>
                BuildTextResponse("Microsoft NCSI"),
            CaptivePortalType.Windows when path == "/connecttest.txt" =>
                BuildTextResponse("Microsoft Connect Test"),
            CaptivePortalType.Windows =>
                BuildHtmlResponse(200, "OK", "Microsoft NCSI"),

            // Firefox expects "success\n"
            CaptivePortalType.Firefox when path == "/success.txt" =>
                BuildTextResponse("success\n"),
            CaptivePortalType.Firefox =>
                BuildHtmlResponse(200, "OK", "<html><body>success</body></html>"),

            // Ubuntu/Linux expects 204 or specific content
            CaptivePortalType.Ubuntu =>
                BuildHtmlResponse(200, "OK", "NetworkManager is online"),

            // Amazon Kindle expects specific response
            CaptivePortalType.Amazon =>
                BuildHtmlResponse(200, "OK", "<!DOCTYPE html><html><head><title>Success</title></head><body>Success</body></html>"),

            // Nintendo expects 200 OK
            CaptivePortalType.Nintendo =>
                BuildHtmlResponse(200, "OK", ""),

            // PlayStation expects 200 OK
            CaptivePortalType.PlayStation =>
                BuildHtmlResponse(200, "OK", ""),

            // Android, Chrome, Samsung, Xiaomi, Huawei, Generic - all expect 204 No Content
            _ => Build204NoContentResponse()
        };
    }

    /// <summary>
    /// Builds a redirect response for captive portal
    /// </summary>
    private string BuildCaptivePortalRedirect(HttpPetition petition)
    {
        string portalUrl = $"http://{GatewayIP}:{ServerPort}{PortalLoginPage}";

        string html = $@"<!DOCTYPE html>
<html>
<head>
    <title>Redirecting...</title>
    <meta http-equiv='refresh' content='0;url={portalUrl}'>
</head>
<body>
    <p>Redirecting to login portal...</p>
    <p><a href='{portalUrl}'>Click here if not redirected</a></p>
</body>
</html>";

        byte[] bodyBytes = System.Text.Encoding.UTF8.GetBytes(html);
        return $"HTTP/1.1 302 Found\r\n" +
               $"Location: {portalUrl}\r\n" +
               "Cache-Control: no-cache, no-store, must-revalidate\r\n" +
               "Pragma: no-cache\r\n" +
               "Content-Type: text/html; charset=utf-8\r\n" +
               $"Content-Length: {bodyBytes.Length}\r\n\r\n{html}";
    }

    /// <summary>
    /// Checks if the client is authenticated by MAC address
    /// </summary>
    private async Task<bool> IsClientAuthenticatedAsync(string clientIP)
    {
        string? macAddress = await GetMacAddressFromIpAsync(clientIP);
        if (string.IsNullOrEmpty(macAddress))
            return false;

        var sessionDto = new SessionDto
        {
            MacAddress = macAddress,
            Username = string.Empty
        };

        return await _sessionManagementService.IsActiveSession(sessionDto);
    }

    #endregion
}
