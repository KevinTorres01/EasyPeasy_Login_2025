public class Device
{
    public string IPAdress { get; set; }
    public string MacAddress { get; set; }  
    public string Hostname { get; set; }
    public DateTime FirstSeenAt { get; set; }
    public DateTime LastSeenAt { get; set; }
    public bool IsBlocked { get; set; }
    
}