public class ConnectionHistory
{
    public string DeviceMacAddress { get; set; }
    public string Username { get; set; }
    public DateTime ConnectionTime { get; set; }
    public DateTime? DisconnectionTime { get; set; }
}