
using System.Collections.Generic;
using System.Threading.Tasks;
using EasyPeasy_Login.Application.DTOs;
using EasyPeasy_Login.Domain.Helper;
using EasyPeasy_Login.Domain.Exceptions;
namespace EasyPeasy_Login.Application.Services.UserManagement;

public class UserManagementService : IUserManagementService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public UserManagementService(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<CreateUserResponseDto> CreateUserAsync(string username, string password)
    {
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
        var newUser = new User(username, hashedPassword);

        await _userRepository.AddAsync(newUser);

        return new CreateUserResponseDto
        {
            Success = true,
            Message = "User created successfully."
        };
    }

    public Task DeleteUserAsync(string username)
    {
        var user = _userRepository.GetByUsernameAsync(username);
        if (user != null)
        {
            return _userRepository.DeleteByUsernameAsync(username);
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
                Username = user.Username
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
                Username = user.Username
            };
        }
        return null;
    }

    public Task<UpdateUserResponseDto> UpdateUserAsync(UpdateUserRequestDto updateUserRequest)
    {
        return Task.Run(async () =>
        {
            var user = await _userRepository.GetByUsernameAsync(updateUserRequest.Name);
            if (user == null)
            {
                return new UpdateUserResponseDto
                {
                    Success = false,
                    Message = "User not found."
                };
            }

            if (!string.IsNullOrEmpty(updateUserRequest.Password))
            {
                user.HashedPassword = _passwordHasher.HashPassword(updateUserRequest.Password);
            }
            await _userRepository.UpdateAsync(user);
            return new UpdateUserResponseDto
            {
                Success = true,
                Message = "User updated successfully."
            };
        });
    }
}