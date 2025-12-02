using System;
using System.Threading.Tasks;

namespace EasyPeasy_Login.Server.Checking;

/// <summary>
/// HttpServer - Browser: Handles common browser automatic requests
/// </summary>
public partial class HttpServer
{
    /// <summary>
    /// Handles common browser requests (favicon, robots.txt, etc.)
    /// Returns null if the request should be handled by other handlers
    /// </summary>
    private string? HandleBrowserCommonRequests(HttpPetition petition)
    {
        string path = petition.Path.ToLower();

        // ===== Favicon =====
        if (path == "/favicon.ico" || path.EndsWith("/favicon.ico"))
        {
            return Build204NoContentResponse();
        }

        // ===== Apple Touch Icons =====
        if (path.Contains("apple-touch-icon"))
        {
            return Build204NoContentResponse();
        }

        // ===== Robots.txt =====
        if (path == "/robots.txt")
        {
            return BuildTextResponse("User-agent: *\nDisallow: /api/\nDisallow: /admin/\n");
        }

        // ===== PWA Manifest =====
        if (path == "/manifest.json" || path == "/site.webmanifest")
        {
            return Build204NoContentResponse();
        }

        // ===== Service Workers =====
        if (path == "/sw.js" || path == "/service-worker.js")
        {
            return Build204NoContentResponse();
        }

        // ===== Source Maps =====
        if (path.EndsWith(".map"))
        {
            return Build404Response();
        }

        // ===== Well-known paths =====
        if (path.StartsWith("/.well-known/"))
        {
            return Build404Response();
        }

        // ===== CORS Preflight =====
        if (petition.Method == "OPTIONS")
        {
            return BuildCorsPreflightResponse();
        }

        // ===== Common browser files that don't exist =====
        if (path == "/browserconfig.xml" ||      // IE/Edge tile config
            path == "/crossdomain.xml" ||         // Flash cross-domain policy
            path == "/clientaccesspolicy.xml" ||  // Silverlight policy
            path == "/humans.txt" ||              // Website credits
            path == "/ads.txt" ||                 // Advertising config
            path == "/app-ads.txt" ||             // Mobile app advertising
            path == "/security.txt" ||            // Security contact info
            path == "/.git/HEAD" ||               // Git repository probe
            path == "/wp-login.php" ||            // WordPress login (security probe)
            path == "/wp-admin" ||                // WordPress admin (security probe)
            path == "/xmlrpc.php" ||              // WordPress XML-RPC (security probe)
            path == "/phpmyadmin" ||              // phpMyAdmin probe
            path == "/.env" ||                    // Environment file probe
            path == "/config.php" ||              // Config file probe
            path == "/.htaccess")                 // Apache config probe
        {
            return Build404Response();
        }

        return null; // Not a common browser request
    }
}
