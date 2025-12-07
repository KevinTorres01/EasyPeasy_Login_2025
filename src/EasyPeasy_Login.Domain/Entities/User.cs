using System.Text.Json.Serialization;

[JsonDerivedType(typeof(User), typeDiscriminator: "User")]
public class User
{
    public string Username { get; set; }
    public string Name { get; set; }
    public string HashedPassword { get; set; }
    public bool IsActive { get; set; }

    public User(string Username, string Name, string HashedPassword, bool IsActive = true)
    {
        this.Username = Username;
        this.Name = Name;
        this.HashedPassword = HashedPassword;
        this.IsActive = IsActive;

    }

}