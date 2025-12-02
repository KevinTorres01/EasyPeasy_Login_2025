using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EasyPeasy_Login.Server.Checking;

/// <summary>
/// HttpServer - Utilities: Response builders and helper methods
/// </summary>
public partial class HttpServer
{
    #region HTTP Response Builders

    private string BuildRedirectResponse(string location)
    {
        string fullUrl = $"http://{GatewayIP}:{ServerPort}{location}";
        return $"HTTP/1.1 302 Found\r\nLocation: {fullUrl}\r\nContent-Length: 0\r\n\r\n";
    }

    private string BuildHtmlResponse(int statusCode, string statusText, string html)
    {
        byte[] bodyBytes = Encoding.UTF8.GetBytes(html);
        return $"HTTP/1.1 {statusCode} {statusText}\r\n" +
               "Content-Type: text/html; charset=utf-8\r\n" +
               $"Content-Length: {bodyBytes.Length}\r\n\r\n{html}";
    }

    private string BuildTextResponse(string text)
    {
        byte[] bodyBytes = Encoding.UTF8.GetBytes(text);
        return $"HTTP/1.1 200 OK\r\n" +
               "Content-Type: text/plain; charset=utf-8\r\n" +
               "Cache-Control: no-cache, no-store, must-revalidate\r\n" +
               $"Content-Length: {bodyBytes.Length}\r\n\r\n{text}";
    }

    private string BuildJsonResponse(int statusCode, object data)
    {
        string json = JsonSerializer.Serialize(data, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        });
        byte[] bodyBytes = Encoding.UTF8.GetBytes(json);

        string statusText = statusCode switch
        {
            200 => "OK",
            201 => "Created",
            204 => "No Content",
            400 => "Bad Request",
            401 => "Unauthorized",
            403 => "Forbidden",
            404 => "Not Found",
            405 => "Method Not Allowed",
            500 => "Internal Server Error",
            _ => "OK"
        };

        return $"HTTP/1.1 {statusCode} {statusText}\r\n" +
               "Content-Type: application/json; charset=utf-8\r\n" +
               "Cache-Control: no-cache\r\n" +
               $"Content-Length: {bodyBytes.Length}\r\n\r\n{json}";
    }

    private string Build204NoContentResponse()
    {
        return "HTTP/1.1 204 No Content\r\n" +
               "Cache-Control: no-cache, no-store, must-revalidate\r\n" +
               "Content-Length: 0\r\n\r\n";
    }

    private string Build404Response()
    {
        string html = "<!DOCTYPE html><html><head><title>404 Not Found</title></head><body><h1>404 Not Found</h1></body></html>";
        byte[] bodyBytes = Encoding.UTF8.GetBytes(html);
        return $"HTTP/1.1 404 Not Found\r\n" +
               "Content-Type: text/html; charset=utf-8\r\n" +
               $"Content-Length: {bodyBytes.Length}\r\n\r\n{html}";
    }

    private string Build405MethodNotAllowedResponse()
    {
        return BuildJsonResponse(405, new { Error = "Method not allowed" });
    }

    private string Build500ErrorResponse(string message)
    {
        return BuildJsonResponse(500, new { Error = "Internal server error", Details = message });
    }

    private string BuildCorsPreflightResponse()
    {
        return "HTTP/1.1 204 No Content\r\n" +
               "Access-Control-Allow-Origin: *\r\n" +
               "Access-Control-Allow-Methods: GET, POST, PUT, DELETE, OPTIONS\r\n" +
               "Access-Control-Allow-Headers: Content-Type, Authorization, X-Requested-With\r\n" +
               "Access-Control-Max-Age: 86400\r\n" +
               "Content-Length: 0\r\n\r\n";
    }

    #endregion

    #region System Utilities

    /// <summary>
    /// Gets the MAC address from the system ARP table
    /// </summary>
    private async Task<string?> GetMacAddressFromIpAsync(string ipAddress)
    {
        try
        {
            // Ping first to ensure the entry exists in ARP
            await ExecuteCommandAsync($"ping -c 1 -W 1 {ipAddress}", ignoreErrors: true);

            // Query ARP table
            string arpOutput = await ExecuteCommandAsync($"ip neigh show {ipAddress}");

            // Search for MAC pattern: aa:bb:cc:dd:ee:ff
            var macMatch = Regex.Match(arpOutput, @"([0-9A-Fa-f]{2}[:-]){5}([0-9A-Fa-f]{2})");

            if (macMatch.Success)
            {
                return macMatch.Value.ToLower();
            }

            // Alternative method: read DHCP leases
            string leasesPath = "/var/lib/misc/dnsmasq.leases";
            if (File.Exists(leasesPath))
            {
                var leases = await File.ReadAllLinesAsync(leasesPath);
                foreach (var lease in leases)
                {
                    var parts = lease.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length >= 3 && parts[2] == ipAddress)
                    {
                        return parts[1].ToLower();
                    }
                }
            }

            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error getting MAC: {ex.Message}");
            return null;
        }
    }

    private async Task<string> ExecuteCommandAsync(string command, bool ignoreErrors = false)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "/bin/bash",
                Arguments = $"-c \"{command}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        process.Start();
        string output = await process.StandardOutput.ReadToEndAsync();
        await process.WaitForExitAsync();

        return output;
    }

    #endregion
}
