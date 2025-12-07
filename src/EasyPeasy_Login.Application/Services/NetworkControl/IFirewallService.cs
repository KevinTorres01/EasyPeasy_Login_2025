namespace EasyPeasy_Login.Application.Services.NetworkControl;

public interface IFirewallService
{
    /// Grants internet access to a device by its MAC address.
    /// Adds an ACCEPT rule to the AUTHENTICATED iptables chain.
    Task<bool> GrantInternetAccessAsync(string macAddress);

    /// Revokes internet access from a device by its MAC address.
    /// Removes the ACCEPT rule from the AUTHENTICATED iptables chain.
    Task<bool> RevokeInternetAccessAsync(string macAddress);

    Task<bool> HasInternetAccessAsync(string macAddress);

    /// Gets a list of all MAC addresses that currently have internet access granted.
    Task<IEnumerable<string>> GetAuthenticatedMacAddressesAsync();
}
