using Microsoft.AspNetCore.Mvc;
using EasyPeasy_Login.Infrastructure.Network.Configuration;

namespace EasyPeasy_Login.Web.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public class NetworkController : ControllerBase
{
    private readonly INetworkOrchestrator _networkOrchestrator;
    private readonly ILogger<NetworkController> _logger;

    public NetworkController(INetworkOrchestrator networkOrchestrator, ILogger<NetworkController> logger)
    {
        _networkOrchestrator = networkOrchestrator;
        _logger = logger;
    }

    [HttpPost("restore")]
    public async Task<IActionResult> RestoreConfiguration()
    {
        try
        {
            _logger.LogInformation("Restoring network configuration...");
            await _networkOrchestrator.RestoreConfiguration();
            _logger.LogInformation("Network configuration restored successfully");
            return Ok(new { success = true, message = "Network configuration restored successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error restoring network configuration");
            return StatusCode(500, new { success = false, message = "Failed to restore network configuration", error = ex.Message });
        }
    }
}
