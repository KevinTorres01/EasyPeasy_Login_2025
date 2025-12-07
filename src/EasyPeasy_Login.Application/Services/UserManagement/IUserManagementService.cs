using EasyPeasy_Login.Application.DTOs;
namespace EasyPeasy_Login.Application.Services.UserManagement;
public interface IUserManagementService
{
    Task<CreateUserResponseDto> CreateUserAsync(string username, string name, string password);
    Task DeleteUserAsync(string username);
    Task<IEnumerable<UserResponseDto>> GetAllUsersAsync();
    Task<UserResponseDto?> GetUserByNameAsync(string username);
    Task<UpdateUserResponseDto> UpdateUserAsync(UpdateUserRequestDto updateUserRequest);
}