using EasyPeasy_Login.Application.DTOs;
using EasyPeasy_Login.Application.Services.DeviceManagement;
using EasyPeasy_Login.Application.Services.SessionManagement;
using EasyPeasy_Login.Domain.Helper;
using EasyPeasy_Login.Domain.Interfaces;
using EasyPeasy_Login.Shared;

namespace EasyPeasy_Login.Application.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IDeviceManagement _deviceManagement;
    private readonly ISessionManagementService _sessionManagementService;
    private readonly ISessionRepository _sessionRepository;
    private readonly ILogger _logger;

    public AuthenticationService(
        IUserRepository userRepository, 
        IPasswordHasher passwordHasher, 
        IDeviceManagement deviceManagement, 
        ISessionManagementService sessionManagementService,
        ISessionRepository sessionRepository,
        ILogger logger)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _deviceManagement = deviceManagement;
        _sessionManagementService = sessionManagementService;
        _sessionRepository = sessionRepository;
        _logger = logger;
    }

    public async Task<LoginResponseDto> AuthenticateAsync(LoginRequestDto loginRequest)
    {
        _logger.LogInfo($"Authentication attempt for user: {loginRequest.Username} from MAC: {loginRequest.MacAddress}");

        // Validación de entrada
        if (string.IsNullOrWhiteSpace(loginRequest.Username))
        {
            _logger.LogWarning("Authentication failed: Username is empty");
            return new LoginResponseDto
            {
                Success = false,
                Message = "Username cannot be empty."
            };
        }

        if (string.IsNullOrWhiteSpace(loginRequest.Password))
        {
            _logger.LogWarning($"Authentication failed for {loginRequest.Username}: Password is empty");
            return new LoginResponseDto
            {
                Success = false,
                Message = "Password cannot be empty."
            };
        }

        if (string.IsNullOrWhiteSpace(loginRequest.MacAddress))
        {
            _logger.LogWarning($"Authentication failed for {loginRequest.Username}: MAC address is empty");
            return new LoginResponseDto
            {
                Success = false,
                Message = "MAC address cannot be empty."
            };
        }

        var user = await _userRepository.GetByUsernameAsync(loginRequest.Username);
        if (user == null)
        {
            _logger.LogWarning($"Authentication failed: User '{loginRequest.Username}' not found");
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
            _logger.LogWarning($"Authentication failed for {loginRequest.Username}: Invalid password");
            return new LoginResponseDto
            {
                Success = false,
                Message = "Invalid password."
            };
        }

        await _sessionManagementService.CreateSession(new CreateSessionRequestDto
        {
            MacAddress = loginRequest.MacAddress,
            Name = loginRequest.Username,
            IPAddress = loginRequest.IpAddress
        });

        _logger.LogInfo($"Authentication successful for user: {loginRequest.Username} from MAC: {loginRequest.MacAddress}");
        return new LoginResponseDto
        {
            Success = true,
            Message = "Authentication successful."
        };
    }

    public async Task<bool> IsAuthenticatedAsync(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            return false;

        // Verificar si hay alguna sesión activa para este usuario
        var sessions = await _sessionRepository.GetByUsernameAsync(username);
        return sessions.Any();
    }
}
