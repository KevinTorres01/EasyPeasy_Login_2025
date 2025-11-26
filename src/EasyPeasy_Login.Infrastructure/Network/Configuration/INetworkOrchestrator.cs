namespace EasyPeasy_Login.Infrastructure.Network.Configuration
{
    public interface INetworkOrchestrator: IDisposable
    {
        public Task<bool> SetUpNetwork();
        public Task RestoreConfiguration();
    }
}