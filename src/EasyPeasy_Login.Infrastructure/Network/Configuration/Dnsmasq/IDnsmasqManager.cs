using System.Threading;
using System.Threading.Tasks;
using EasyPeasy_Login.Infrastructure.Network.Configuration;

namespace EasyPeasy_Login.Infrastructure.Network.Configuration
{
    public interface IDnsmasqManager
    {
        Task<ExecutionResult> ConfigureDnsmasqAsync();
        Task<ExecutionResult> StartDnsmasqAsync();
        Task<ExecutionResult> StopDnsmasqAsync();
        Task<ExecutionResult> ValidateDnsConfiguration();
    }
}