public interface INetworkConfigurationService :IDisposable
{
    public Task<bool> SetupNetwork();
    public void RestoreConfiguration();
}