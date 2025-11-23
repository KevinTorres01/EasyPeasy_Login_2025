using Microsoft.AspNetCore.Mvc;
using EasyPeasy_Login.Infrastructure.Network.Configuration;

namespace EasyPeasy_Login.Web.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public class NetworkController : ControllerBase
{
    private readonly INetworkConfigurationService _networkService;
    private readonly ILogger<NetworkController> _logger;

    public NetworkController(
        INetworkConfigurationService networkService,
        ILogger<NetworkController> logger)
    {
        _networkService = networkService;
        _logger = logger;
    }

    [HttpPost("configure")]
    public async Task<IActionResult> ConfigureNetwork([FromBody] NetworkConfigurationParams configParams)
    {
        try
        {
            _logger.LogInformation("Starting network configuration");
            var result = await _networkService.SetupNetworkAsync(configParams);
            
            if (result)
            {
                return Ok(new { success = true, message = "Network configured successfully" });
            }
            else
            {
                return BadRequest(new { success = false, message = "Network configuration failed" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error configuring network");
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    [HttpPost("restore")]
    public async Task<IActionResult> RestoreConfiguration()
    {
        try
        {
            _logger.LogInformation("Restoring network configuration");
            await _networkService.RestoreConfigurationAsync();
            return Ok(new { success = true, message = "Configuration restored successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error restoring network configuration");
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    [HttpGet("info")]
    public async Task<IActionResult> GetNetworkInfo()
    {
        try
        {
            var info = await _networkService.GetCurrentNetworkInfoAsync();
            return Ok(new { success = true, data = info });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting network information");
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }
}
