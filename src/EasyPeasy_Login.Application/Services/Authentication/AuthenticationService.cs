using EasyPeasy_Login.Application.DTOs;
using EasyPeasy_Login.Domain.Helper;
namespace EasyPeasy_Login.Application.Services;

public class AuthenticationService : IAuthenticationService
{
    private List<User> _loggedInUsers = new List<User>();
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    public AuthenticationService(IUserRepository userRepository , IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
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
