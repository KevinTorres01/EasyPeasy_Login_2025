using System.Text.Json.Serialization;

[JsonDerivedType(typeof(Session), typeDiscriminator: "Session")]
public class Session
{
    public string DeviceMacAddress { get; set; }
    public string Username { get; set; }
    public DateTime CreatedAt { get; set; }
}