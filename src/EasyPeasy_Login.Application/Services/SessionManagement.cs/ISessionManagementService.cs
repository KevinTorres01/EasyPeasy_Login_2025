using EasyPeasy_Login.Application.DTOs;
namespace EasyPeasy_Login.Application.Services.SessionManagement;

public interface ISessionManagementService
{
    Task<CreateSessionResponseDto> CreateSession(CreateSessionRequestDto createSessionRequest);
    Task<bool> IsActiveSession(SessionDto session);
    Task InvalidateSession(SessionDto session);

}