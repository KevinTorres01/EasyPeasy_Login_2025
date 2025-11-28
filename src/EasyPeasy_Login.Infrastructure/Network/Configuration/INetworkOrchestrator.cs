namespace EasyPeasy_Login.Infrastructure.Network.Configuration
{
    public interface INetworkOrchestrator
    {
        public Task<bool> SetUpNetwork();
        public Task RestoreConfiguration();
    }
}