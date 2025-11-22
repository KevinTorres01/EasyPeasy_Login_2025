public interface ICaptivePortalDetectionService
{
    Task EnforcePortalForIpAsync(string clientIpAddress);
}