
using System.Collections.Generic;
using System.Threading.Tasks;
using EasyPeasy_Login.Application.DTOs;
using EasyPeasy_Login.Domain.Helper;
using EasyPeasy_Login.Domain.Exceptions;
using EasyPeasy_Login.Domain.Interfaces;

namespace EasyPeasy_Login.Application.Services.UserManagement;

public class UserManagementService : IUserManagementService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ISessionRepository _sessionRepository;

    public UserManagementService(IUserRepository userRepository, IPasswordHasher passwordHasher, ISessionRepository sessionRepository)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _sessionRepository = sessionRepository;
    }

    public async Task<CreateUserResponseDto> CreateUserAsync(string username, string name, string password)
    {
        // Input Validation
        if (string.IsNullOrWhiteSpace(username))
        {
            return new CreateUserResponseDto
            {
                Success = false,
                Message = "Username cannot be empty."
            };
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            return new CreateUserResponseDto
            {
                Success = false,
                Message = "Name cannot be empty."
            };
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            return new CreateUserResponseDto
            {
                Success = false,
                Message = "Password cannot be empty."
            };
        }

        if (password.Length < 4)
        {
            return new CreateUserResponseDto
            {
                Success = false,
                Message = "Password must be at least 4 characters long."
            };
        }

        var existingUser = await _userRepository.GetByUsernameAsync(username);
        if (existingUser != null)
        {
            return new CreateUserResponseDto
            {
                Success = false,
                Message = "Username already exists."
            };
        }

        var hashedPassword = _passwordHasher.HashPassword(password);
        var newUser = new User(username, name, hashedPassword);

        await _userRepository.AddAsync(newUser);

        return new CreateUserResponseDto
        {
            Success = true,
            Message = "User created successfully."
        };
    }

    public async Task DeleteUserAsync(string username)
    {
        var user = await _userRepository.GetByUsernameAsync(username);
        if (user != null)
        {
            // Delete all user session before delete the user
            var sessions = await _sessionRepository.GetByUsernameAsync(username);
            foreach (var session in sessions)
            {
                await _sessionRepository.DeleteAsync(session.DeviceMacAddress, session.Username);
            }
            
            await _userRepository.DeleteByUsernameAsync(username);
        }
        else
        {
            throw new UserNotFoundException(username);
        }
    }

    public async Task<IEnumerable<UserResponseDto>> GetAllUsersAsync()
    {
        var users = await _userRepository.GetAllAsync();
        var userDtos = new List<UserResponseDto>();
        foreach (var user in users)
        {
            userDtos.Add(new UserResponseDto
            {
                Username = user.Username,
                Name = user.Name,
                IsActive = user.IsActive
            });
        }
        return userDtos;
    }

    public async Task<UserResponseDto?> GetUserByNameAsync(string username)
    {
        var user = await _userRepository.GetByUsernameAsync(username);
        if (user != null)
        {
            return new UserResponseDto
            {
                Username = user.Username,
                Name = user.Name,
                IsActive = user.IsActive
            };
        }
        return null;
    }

    public async Task<UpdateUserResponseDto> UpdateUserAsync(UpdateUserRequestDto updateUserRequest)
    {
        if (string.IsNullOrWhiteSpace(updateUserRequest.Username))
        {
            return new UpdateUserResponseDto
            {
                Success = false,
                Message = "Username cannot be empty."
            };
        }

        var user = await _userRepository.GetByUsernameAsync(updateUserRequest.Username);
        if (user == null)
        {
            return new UpdateUserResponseDto
            {
                Success = false,
                Message = "User not found."
            };
        }

        if (!string.IsNullOrWhiteSpace(updateUserRequest.Name))
        {
            user.Name = updateUserRequest.Name;
        }

        if (!string.IsNullOrEmpty(updateUserRequest.Password))
        {
            if (updateUserRequest.Password.Length < 4)
            {
                return new UpdateUserResponseDto
                {
                    Success = false,
                    Message = "Password must be at least 4 characters long."
                };
            }
            user.HashedPassword = _passwordHasher.HashPassword(updateUserRequest.Password);
        }

        if (updateUserRequest.IsActive.HasValue)
        {
            user.IsActive = updateUserRequest.IsActive.Value;
        }
        
        await _userRepository.UpdateAsync(user);
        return new UpdateUserResponseDto
        {
            Success = true,
            Message = "User updated successfully."
        };
    }
}