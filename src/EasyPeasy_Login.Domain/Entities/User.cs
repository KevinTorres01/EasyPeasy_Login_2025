public class User
{
    public string Username { get; set; }
    public string HashedPassword { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastLoginAt { get; set; }
    public bool IsActive { get; set; }

    public User(string username, string password)
    {
        Username = username;
        HashedPassword = password;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        LastLoginAt = DateTime.UtcNow;
    }

}