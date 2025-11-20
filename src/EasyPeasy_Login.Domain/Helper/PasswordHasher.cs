
namespace EasyPeasy_Login.Domain.Helper;

public class PasswordHasher : IPasswordHasher
{
    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public bool VerifyPassword(string hashedPassword, string providedPassword)
    {
        var providedHashed = HashPassword(providedPassword);
        return hashedPassword == providedHashed;
    }
}