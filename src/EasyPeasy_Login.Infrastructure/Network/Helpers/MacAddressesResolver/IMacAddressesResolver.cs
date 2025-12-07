public interface IMacAddressResolver
{
    Task<string?> GetMacAddressFromIpAsync(string ipAddress);
}
