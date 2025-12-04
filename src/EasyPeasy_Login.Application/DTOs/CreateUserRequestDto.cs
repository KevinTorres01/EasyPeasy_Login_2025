namespace EasyPeasy_Login.Application.DTOs;
public class CreateUserRequestDto
{
    public string Username { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}