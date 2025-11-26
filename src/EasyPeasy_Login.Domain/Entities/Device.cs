using System.Text.Json.Serialization;

[JsonDerivedType(typeof(Device), typeDiscriminator: "Device")]
public class Device
{
    public Device(string ipAddress, string macAddress)
    {
        IPAddress = ipAddress;
        MacAddress = macAddress;

    }
    public string IPAddress { get; set; }
    public string MacAddress { get; set; }

}