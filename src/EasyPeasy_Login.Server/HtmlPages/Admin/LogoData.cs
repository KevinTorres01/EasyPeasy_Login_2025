namespace EasyPeasy_Login.Server.HtmlPages.Admin;

/// <summary>
/// Contains the embedded logo as a base64 data URI
/// </summary>
public static class LogoData
{
    public static string GetLogoDataUri()
    {
        // Read and encode logo on demand to avoid huge string constant
        var logoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "images", "Logo.png");
        
        if (File.Exists(logoPath))
        {
            var logoBytes = File.ReadAllBytes(logoPath);
            var base64 = Convert.ToBase64String(logoBytes);
            return $"data:image/png;base64,{base64}";
        }
        
        // Fallback SVG logo if file not found
        return "data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 200 80'%3E%3Ctext x='50%25' y='50%25' dominant-baseline='middle' text-anchor='middle' font-size='40' font-weight='bold' font-family='Arial, sans-serif' fill='%235aabea'%3EEasyPeasy%3C/text%3E%3C/svg%3E";
    }
}
