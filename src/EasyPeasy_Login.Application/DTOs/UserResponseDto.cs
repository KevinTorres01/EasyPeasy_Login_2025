namespace EasyPeasy_Login.Application.DTOs;
public class UserResponseDto
{
    public string Username { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}