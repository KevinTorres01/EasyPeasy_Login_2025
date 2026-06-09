using System.Text;
using System.Text.Json;

namespace EasyPeasy_Login.Server.Checking;

/// <summary>
/// Helper class for building consistent HTTP responses.
/// All responses include "Connection: close" because the server closes the socket
/// after each reply; without it some HTTP/1.1 clients keep the connection open
/// waiting for more data and delay rendering.
/// </summary>
public static class ApiResponseBuilder
{
    private const string ConnectionClose = "Connection: close\r\n";

    public static string Http204() =>
        "HTTP/1.1 204 No Content\r\n" + ConnectionClose + "Content-Length: 0\r\n\r\n";

    public static string HttpOptions() =>
        "HTTP/1.1 204 No Content\r\n" +
        "Access-Control-Allow-Origin: *\r\n" +
        "Access-Control-Allow-Methods: GET, POST, PUT, DELETE, OPTIONS\r\n" +
        "Access-Control-Allow-Headers: Content-Type\r\n" +
        ConnectionClose +
        "Content-Length: 0\r\n\r\n";

    public static string RedirectToPortal(string gatewayIP, int port)
    {
        string url = $"http://{gatewayIP}:{port}/portal/login";
        return $"HTTP/1.1 302 Found\r\nLocation: {url}\r\n{ConnectionClose}Content-Length: 0\r\n\r\n";
    }

    public static string Redirect(string url)
    {
        return $"HTTP/1.1 302 Found\r\nLocation: {url}\r\n{ConnectionClose}Content-Length: 0\r\n\r\n";
    }

    public static string HttpHtml(string html)
    {
        int len = Encoding.UTF8.GetByteCount(html);
        return $"HTTP/1.1 200 OK\r\nContent-Type: text/html; charset=utf-8\r\n{ConnectionClose}Content-Length: {len}\r\n\r\n{html}";
    }

    public static string HttpJson(object data)
    {
        string json = JsonSerializer.Serialize(data);
        int len = Encoding.UTF8.GetByteCount(json);
        return $"HTTP/1.1 200 OK\r\nContent-Type: application/json; charset=utf-8\r\nAccess-Control-Allow-Origin: *\r\nAccess-Control-Allow-Methods: GET, POST, PUT, DELETE, OPTIONS\r\nAccess-Control-Allow-Headers: Content-Type\r\n{ConnectionClose}Content-Length: {len}\r\n\r\n{json}";
    }

    public static string HttpError(int statusCode, string message)
    {
        var errorObj = new { error = message };
        string json = JsonSerializer.Serialize(errorObj);
        int len = Encoding.UTF8.GetByteCount(json);
        string statusText = statusCode switch
        {
            400 => "Bad Request",
            403 => "Forbidden",
            404 => "Not Found",
            413 => "Payload Too Large",
            415 => "Unsupported Media Type",
            500 => "Internal Server Error",
            _ => "Error"
        };
        return $"HTTP/1.1 {statusCode} {statusText}\r\nContent-Type: application/json; charset=utf-8\r\nAccess-Control-Allow-Origin: *\r\nAccess-Control-Allow-Methods: GET, POST, PUT, DELETE, OPTIONS\r\nAccess-Control-Allow-Headers: Content-Type\r\n{ConnectionClose}Content-Length: {len}\r\n\r\n{json}";
    }
}
