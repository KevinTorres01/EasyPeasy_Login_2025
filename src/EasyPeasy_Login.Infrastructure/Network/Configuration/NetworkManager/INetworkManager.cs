using EasyPeasy_Login.Infrastructure.Network.Configuration;

namespace EasyPeasy_Login.Infrastructure.Network.Configuration
{
    public interface INetworkManager
    {
        Task<ExecutionResult> ConfigureNetworkInterface();
        Task<string?> DetectUpstreamInterface();
        Task<ExecutionResult> UnblockRfkill();
        Task<ExecutionResult> EnableIpPacketForwarding();
        Task<ExecutionResult> RestoreNetworkInterfaceConfiguration();
    }
}