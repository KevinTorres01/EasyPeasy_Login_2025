namespace EasyPeasy_Login.Infrastructure.Network.Configuration
{
    public interface ICaptivePortalControlManager
    {
        public Task ConfigureCaptivePortal();
        public Task RestoreCaptivePortalConfiguration();
    }
}