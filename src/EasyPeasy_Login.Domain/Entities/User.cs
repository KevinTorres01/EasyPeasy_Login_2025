using System.Text.Json.Serialization;

[JsonDerivedType(typeof(User), typeDiscriminator: "User")]
public class User
{
    public string Username { get; set; }
    public string HashedPassword { get; set; }
    public bool IsActive { get; set; }

    public User(string Username, string HashedPassword, bool IsActive = true)
    {
        this.Username = Username;
        this.HashedPassword = HashedPassword;
        this.IsActive = IsActive;

    }

}