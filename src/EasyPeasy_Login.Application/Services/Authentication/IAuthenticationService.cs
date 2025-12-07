using EasyPeasy_Login.Application.DTOs;
namespace EasyPeasy_Login.Application.Services;
public interface IAuthenticationService
{
    Task<LoginResponseDto> AuthenticateAsync(LoginRequestDto loginRequest);
    Task<bool> IsAuthenticatedAsync(string username);
}