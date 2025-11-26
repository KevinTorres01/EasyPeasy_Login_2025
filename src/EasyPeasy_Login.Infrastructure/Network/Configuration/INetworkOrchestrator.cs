namespace EasyPeasy_Login.Infrastructure.Network.Configuration
{
    public interface INetworkOrchestrator: IDisposable
    {
        Task<bool> SetUpNetwork();
    }
}