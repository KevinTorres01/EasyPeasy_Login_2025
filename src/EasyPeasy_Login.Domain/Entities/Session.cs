using System.Text.Json.Serialization;

[JsonDerivedType(typeof(Session), typeDiscriminator: "Session")]
public class Session
{
    public Session(string deviceMacAddress, string username, string ipAddress)
    {
        DeviceMacAddress = deviceMacAddress;
        Username = username;
        IpAddress = ipAddress;
    }
    public string DeviceMacAddress { get; set; }
    public string Username { get; set; }
    public string IpAddress { get; set; }
}