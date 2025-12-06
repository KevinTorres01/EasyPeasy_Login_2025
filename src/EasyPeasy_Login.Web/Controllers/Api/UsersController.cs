using Microsoft.AspNetCore.Mvc;
using EasyPeasy_Login.Application.Services.UserManagement;
using EasyPeasy_Login.Application.DTOs;

namespace EasyPeasy_Login.Web.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserManagementService _userManagementService;

    public UsersController(IUserManagementService userManagementService)
    {
        _userManagementService = userManagementService;
    }

    /// <summary>
    /// GET /api/users - Get all users
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        try
        {
            var users = await _userManagementService.GetAllUsersAsync();
            return Ok(users);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    /// <summary>
    /// GET /api/users/{username} - Get user by username
    /// </summary>
    [HttpGet("{username}")]
    public async Task<IActionResult> GetUser(string username)
    {
        try
        {
            var user = await _userManagementService.GetUserByNameAsync(username);
            if (user == null)
                return NotFound(new { error = "User not found" });
            
            return Ok(user);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    /// <summary>
    /// POST /api/users - Create new user
    /// Body: { "username": "...", "name": "...", "password": "..." }
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequestDto request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Username) || 
                string.IsNullOrWhiteSpace(request.Name) || 
                string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new { error = "Username, name, and password are required" });
            }

            var result = await _userManagementService.CreateUserAsync(
                request.Username, 
                request.Name, 
                request.Password);

            if (!result.Success)
                return BadRequest(new { error = result.Message });

            return CreatedAtAction(nameof(GetUser), new { username = request.Username }, result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    /// <summary>
    /// PUT /api/users/{username} - Update user
    /// Body: { "name": "...", "password": "...", "isActive": true }
    /// </summary>
    [HttpPut("{username}")]
    public async Task<IActionResult> UpdateUser(string username, [FromBody] UpdateUserRequestDto request)
    {
        try
        {
            // Ensure username matches
            request.Username = username;

            var result = await _userManagementService.UpdateUserAsync(request);

            if (!result.Success)
                return BadRequest(new { error = result.Message });

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    /// <summary>
    /// DELETE /api/users/{username} - Delete user
    /// </summary>
    [HttpDelete("{username}")]
    public async Task<IActionResult> DeleteUser(string username)
    {
        try
        {
            await _userManagementService.DeleteUserAsync(username);
            return Ok(new { success = true, message = "User deleted successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }
}
