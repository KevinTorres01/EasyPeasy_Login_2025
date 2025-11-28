using System.Threading;
using System.Threading.Tasks;
using EasyPeasy_Login.Infrastructure.Network.Configuration;

namespace EasyPeasy_Login.Infrastructure.Network.Configuration
{
    public interface IHostapdManager
    {
        Task<ExecutionResult> ConfigureHostapdAsync();
        Task<ExecutionResult> StartHostapdAsync();
        Task<ExecutionResult> StopHostapdAsync();
    }
}