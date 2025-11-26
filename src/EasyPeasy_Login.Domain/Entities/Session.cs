using System.Text.Json.Serialization;

[JsonDerivedType(typeof(Session), typeDiscriminator: "Session")]
public class Session
{
    public Session(string deviceMacAddress, string username)
    {
        DeviceMacAddress = deviceMacAddress;
        Username = username;
    }
    public string DeviceMacAddress { get; set; }
    public string Username { get; set; }
}