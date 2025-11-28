using EasyPeasy_Login.Application.DTOs;
using EasyPeasy_Login.Application.Services.DeviceManagement;
using EasyPeasy_Login.Application.Services.SessionManagement;
using EasyPeasy_Login.Domain.Helper;
namespace EasyPeasy_Login.Application.Services;

public class AuthenticationService : IAuthenticationService
{
    private List<User> _loggedInUsers = new List<User>();
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IDeviceManagement deviceManagement;
    private readonly ISessionManagementService sessionManagementService;
    public AuthenticationService(IUserRepository userRepository, IPasswordHasher passwordHasher, IDeviceManagement deviceManagement, ISessionManagementService sessionManagementService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        this.deviceManagement = deviceManagement;
        this.sessionManagementService = sessionManagementService;
    }

    async Task<LoginResponseDto> IAuthenticationService.AuthenticateAsync(LoginRequestDto loginRequest)
    {
        var user = await _userRepository.GetByUsernameAsync(loginRequest.Username);
        if (user == null)
        {
            return new LoginResponseDto
            {
                Success = false,
                Message = "User not found."
            };
        }
        var isPasswordValid = _passwordHasher.VerifyPassword(
            user.HashedPassword, loginRequest.Password);

        if (!isPasswordValid)
        {
            return new LoginResponseDto
            {
                Success = false,
                Message = "Invalid password."
            };
        }
        if (!_loggedInUsers.Contains(user))
        {
            _loggedInUsers.Add(user);
        }
        await sessionManagementService.CreateSession(new CreateSessionRequestDto
        {
            MacAddress = loginRequest.MacAddress,
            Name = loginRequest.Username,
            IPAddress = loginRequest.IpAddress
        });
        return new LoginResponseDto
        {
            Success = true,
            Message = "Authentication successful."
        };
    }

    public Task<bool> IsAuthenticatedAsync(string username)
    {
        var isAuthenticated = _loggedInUsers.Any(u => u.Username == username);
        return Task.FromResult(isAuthenticated);
    }
}
