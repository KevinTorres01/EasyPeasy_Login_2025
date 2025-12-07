using System.Text.Json.Serialization;

[JsonDerivedType(typeof(Device), typeDiscriminator: "Device")]
public class Device
{
    public Device(string macAddress, string ipAddress)
    {
        MacAddress = macAddress;
        IPAddress = ipAddress;
    }
    public string IPAddress { get; set; }
    public string MacAddress { get; set; }

}