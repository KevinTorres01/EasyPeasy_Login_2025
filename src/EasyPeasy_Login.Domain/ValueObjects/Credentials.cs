namespace EasyPeasy_Login.Domain.ValueObjects;

public sealed class Credentials : IEquatable<Credentials>
{
    public string Username { get; }
    public string Password { get; }

    private Credentials(string username, string password)
    {
        Username = username;
        Password = password;
    }

    public static Credentials Create(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username cannot be empty", nameof(username));

        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password cannot be empty", nameof(password));

        if (username.Length < 3)
            throw new ArgumentException("Username must be at least 3 characters", nameof(username));

        if (password.Length < 6)
            throw new ArgumentException("Password must be at least 6 characters", nameof(password));

        return new Credentials(username.Trim(), password);
    }

    public bool Equals(Credentials? other) =>
        other is not null && 
        Username == other.Username && 
        Password == other.Password;

    public override bool Equals(object? obj) =>
        Equals(obj as Credentials);

    public override int GetHashCode() =>
        HashCode.Combine(Username, Password);

    public override string ToString() =>
        $"Credentials for user: {Username}";
}