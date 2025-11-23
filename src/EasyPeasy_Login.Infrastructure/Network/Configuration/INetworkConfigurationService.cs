public interface INetworkConfigurationService :IDisposable
{
    public Task<bool> SetupNetwork();
}