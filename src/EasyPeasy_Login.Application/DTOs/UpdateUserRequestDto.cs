namespace EasyPeasy_Login.Application.DTOs;
public class UpdateUserRequestDto
{
    public string Username { get; set; } = string.Empty;
    public string? Name { get; set; }
    public string? Password { get; set; }
    public bool? IsActive { get; set; }
} 