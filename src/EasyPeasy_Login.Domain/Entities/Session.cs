public class Session
{
    public string DeviceMacAddress { get; set; }
    public string Username { get; set; }
    public DateTime LoginTime { get; set; }
    public DateTime? LogoutTime { get; set; }
}