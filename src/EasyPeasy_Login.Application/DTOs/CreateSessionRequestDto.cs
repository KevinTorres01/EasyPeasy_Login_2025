namespace EasyPeasy_Login.Application.DTOs;

public class CreateSessionRequestDto
{
    public string Name { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string MacAddress { get; set; } = string.Empty;
    public string IPAddress { get; set; } = string.Empty;
}