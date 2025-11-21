using System.Threading.Tasks;
using EasyPeasy_Login.Application.DTOs;
using EasyPeasy_Login.Domain.Interfaces;

namespace EasyPeasy_Login.Application.Services.SessionManagement;

public class SessionManagementService : ISessionManagementService
{

    private readonly ISessionRepository _sessionRepository;
    private readonly IDeviceRepository _deviceRepository;

    public SessionManagementService(ISessionRepository sessionRepository, IDeviceRepository deviceRepository)
    {
        _sessionRepository = sessionRepository;
        _deviceRepository = deviceRepository;
    }
    public async Task<CreateSessionResponseDto> CreateSession(CreateSessionRequestDto createSessionRequest)
    {
        var session = await _sessionRepository.GetByMacAddressAsync(createSessionRequest.MacAddress);
        if (session != null)
        {
            return new CreateSessionResponseDto
            {
                Success = false,
                Message = "Session already exists for this device."
            };
        }
        else
        {
            var newSession = new Session
            {
                DeviceMacAddress = createSessionRequest.MacAddress,
                Username = createSessionRequest.Name
            };
            await _sessionRepository.AddAsync(newSession);
            return new CreateSessionResponseDto
            {
                Success = true,
                Message = "Session created successfully."
            };
        }
    }

    public async Task InvalidateSession(SessionDto session)
    {
        var existingSession = await _sessionRepository.GetByMacAddressAsync(session.MacAddress);
        if (existingSession != null && existingSession.Username == session.Username)
        {
            await _sessionRepository.DeleteAsync(session.MacAddress, session.Username);
            await _deviceRepository.DeleteAsync(session.MacAddress);
        }
    }

    public async Task<bool> IsActiveSession(SessionDto session)
    {
        var existingSession = await _sessionRepository.GetByMacAddressAsync(session.MacAddress);
        return existingSession != null && existingSession.Username == session.Username;
    }
}