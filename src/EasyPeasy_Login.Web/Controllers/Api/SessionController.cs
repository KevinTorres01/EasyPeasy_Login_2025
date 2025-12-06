using Microsoft.AspNetCore.Mvc;
using EasyPeasy_Login.Application.Services.SessionManagement;
using EasyPeasy_Login.Application.DTOs;
using EasyPeasy_Login.Domain.Interfaces;

namespace EasyPeasy_Login.Web.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public class SessionController : ControllerBase
{
    private readonly ISessionManagementService _sessionManagementService;
    private readonly ISessionRepository _sessionRepository;

    public SessionController(
        ISessionManagementService sessionManagementService,
        ISessionRepository sessionRepository)
    {
        _sessionManagementService = sessionManagementService;
        _sessionRepository = sessionRepository;
    }

    /// <summary>
    /// GET /api/session - Get all active sessions
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAllSessions()
    {
        try
        {
            var sessions = await _sessionRepository.GetAllAsync();
            var sessionDtos = sessions.Select(s => new
            {
                macAddress = s.DeviceMacAddress,
                username = s.Username,
                ipAddress = s.IpAddress
            });
            
            return Ok(sessionDtos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    /// <summary>
    /// GET /api/session/{macAddress} - Get session by MAC address
    /// </summary>
    [HttpGet("{macAddress}")]
    public async Task<IActionResult> GetSession(string macAddress)
    {
        try
        {
            var session = await _sessionRepository.GetByMacAddressAsync(macAddress);
            if (session == null)
                return NotFound(new { error = "Session not found" });

            return Ok(new
            {
                macAddress = session.DeviceMacAddress,
                username = session.Username,
                ipAddress = session.IpAddress
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    /// <summary>
    /// DELETE /api/session/{macAddress} - Terminate session
    /// </summary>
    [HttpDelete("{macAddress}")]
    public async Task<IActionResult> DeleteSession(string macAddress)
    {
        try
        {
            var session = await _sessionRepository.GetByMacAddressAsync(macAddress);
            if (session == null)
                return NotFound(new { error = "Session not found" });

            await _sessionManagementService.InvalidateSession(new SessionDto
            {
                MacAddress = macAddress,
                Username = session.Username
            });

            return Ok(new { success = true, message = "Session terminated successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }
}
