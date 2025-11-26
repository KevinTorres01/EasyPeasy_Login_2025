namespace EasyPeasy_Login.Server.Checking;

public class HttpPetition
{
    public string Method { get; set; } = "";
    public string Path { get; set; } = "";
    public string Host { get; set; } = "";
    public string Body { get; set; } = "";
    public string ClientIP { get; set; } = "";

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
}
