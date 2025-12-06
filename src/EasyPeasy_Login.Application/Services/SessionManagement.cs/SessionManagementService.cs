using System.Threading.Tasks;
using EasyPeasy_Login.Application.DTOs;
using EasyPeasy_Login.Domain.Interfaces;
using EasyPeasy_Login.Application.Services.DeviceManagement;
using EasyPeasy_Login.Shared;

namespace EasyPeasy_Login.Application.Services.SessionManagement;

public class SessionManagementService : ISessionManagementService
{
    private readonly ISessionRepository _sessionRepository;
    private readonly IDeviceRepository _deviceRepository;
    private readonly IDeviceManagement _deviceManagement;
    private readonly INetworkControlService _networkControlService;
    private readonly ILogger _logger;

    public SessionManagementService(
        ISessionRepository sessionRepository, 
        IDeviceRepository deviceRepository, 
        IDeviceManagement deviceManagement,
        INetworkControlService networkControlService,
        ILogger logger)
    {
        _sessionRepository = sessionRepository;
        _deviceRepository = deviceRepository;
        _deviceManagement = deviceManagement;
        _networkControlService = networkControlService;
        _logger = logger;
    }

    public async Task<CreateSessionResponseDto> CreateSession(CreateSessionRequestDto createSessionRequest)
    {
        _logger.LogInfo($"Creating session for MAC: {createSessionRequest.MacAddress}, User: {createSessionRequest.Name}");

        var session = await _sessionRepository.GetByMacAddressAsync(createSessionRequest.MacAddress);
        if (session != null)
        {
            _logger.LogWarning($"Session already exists for MAC: {createSessionRequest.MacAddress}");
            return new CreateSessionResponseDto
            {
                Success = false,
                Message = "Session already exists for this device."
            };
        }
        else
        {
            var newSession = new Session(createSessionRequest.MacAddress, createSessionRequest.Name, createSessionRequest.IPAddress);
            if (await _deviceRepository.GetByMacAddressAsync(createSessionRequest.MacAddress) == null)
            {
                await _deviceManagement.AuthenticateDeviceAsync(newSession.DeviceMacAddress, createSessionRequest.IPAddress);
            }
            await _sessionRepository.AddAsync(newSession);
            
            // Grant internet access via iptables
            await _networkControlService.AllowDeviceAsync(
                createSessionRequest.MacAddress, 
                createSessionRequest.IPAddress, 
                createSessionRequest.Name);
            
            _logger.LogInfo($"Session created successfully for MAC: {createSessionRequest.MacAddress}");
            return new CreateSessionResponseDto
            {
                Success = true,
                Message = "Session created successfully."
            };
        }
    }

    public async Task InvalidateSession(SessionDto session)
    {
        _logger.LogInfo($"Invalidating session for MAC: {session.MacAddress}, User: {session.Username}");
        
        var existingSession = await _sessionRepository.GetByMacAddressAsync(session.MacAddress);
        if (existingSession != null && existingSession.Username == session.Username)
        {
            // Revoke internet access via iptables
            await _networkControlService.BlockDeviceAsync(session.MacAddress);
            
            await _sessionRepository.DeleteAsync(session.MacAddress, session.Username);
            await _deviceRepository.DeleteAsync(session.MacAddress);
            _logger.LogInfo($"Session invalidated for MAC: {session.MacAddress}");
        }
        else
        {
            _logger.LogWarning($"Session not found for MAC: {session.MacAddress}, User: {session.Username}");
        }
    }

    public async Task<bool> IsActiveSession(SessionDto session)
    {
        if (string.IsNullOrWhiteSpace(session.MacAddress))
            return false;
            
        var existingSession = await _sessionRepository.GetByMacAddressAsync(session.MacAddress);
        return existingSession != null;
    }

    public async Task<SessionDto?> GetSessionByMacAsync(string macAddress)
    {
        var session = await _sessionRepository.GetByMacAddressAsync(macAddress);
        if (session == null)
            return null;

        return new SessionDto
        {
            MacAddress = session.DeviceMacAddress,
            Username = session.Username,
            IpAddress = session.IpAddress
        };
    }

    public async Task<IEnumerable<SessionDto>> GetAllSessionsAsync()
    {
        var sessions = await _sessionRepository.GetAllAsync();
        return sessions.Select(s => new SessionDto
        {
            MacAddress = s.DeviceMacAddress,
            Username = s.Username,
            IpAddress = s.IpAddress
        });
    }
}