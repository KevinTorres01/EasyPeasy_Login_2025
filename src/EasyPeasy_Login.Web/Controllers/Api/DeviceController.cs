using Microsoft.AspNetCore.Mvc;
using EasyPeasy_Login.Domain.Interfaces;

namespace EasyPeasy_Login.Web.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public class DeviceController : ControllerBase
{
    private readonly IDeviceRepository _deviceRepository;
    private readonly ISessionRepository _sessionRepository;

    public DeviceController(
        IDeviceRepository deviceRepository,
        ISessionRepository sessionRepository)
    {
        _deviceRepository = deviceRepository;
        _sessionRepository = sessionRepository;
    }

    /// <summary>
    /// GET /api/device - Get all devices with session info
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAllDevices()
    {
        try
        {
            var devices = await _deviceRepository.GetAllAsync();
            var sessions = await _sessionRepository.GetAllAsync();
            
            var deviceList = devices.Select(d =>
            {
                var session = sessions.FirstOrDefault(s => s.DeviceMacAddress == d.MacAddress);
                return new
                {
                    macAddress = d.MacAddress,
                    ipAddress = d.IPAddress,
                    username = session?.Username ?? "Unknown"
                };
            });

            return Ok(deviceList);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    /// <summary>
    /// GET /api/device/{macAddress} - Get device by MAC address
    /// </summary>
    [HttpGet("{macAddress}")]
    public async Task<IActionResult> GetDevice(string macAddress)
    {
        try
        {
            var device = await _deviceRepository.GetByMacAddressAsync(macAddress);
            if (device == null)
                return NotFound(new { error = "Device not found" });

            var sessions = await _sessionRepository.GetByUsernameAsync(device.MacAddress);
            var session = sessions.FirstOrDefault();

            return Ok(new
            {
                macAddress = device.MacAddress,
                ipAddress = device.IPAddress,
                username = session?.Username ?? "Unknown"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    /// <summary>
    /// DELETE /api/device/{macAddress} - Remove device
    /// </summary>
    [HttpDelete("{macAddress}")]
    public async Task<IActionResult> DeleteDevice(string macAddress)
    {
        try
        {
            var device = await _deviceRepository.GetByMacAddressAsync(macAddress);
            if (device == null)
                return NotFound(new { error = "Device not found" });

            await _deviceRepository.DeleteAsync(macAddress);
            return Ok(new { success = true, message = "Device removed successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }
}
