using System.Net;
using System.Net.Sockets;

namespace EasyPeasy_Login.Domain.ValueObjects;

public sealed class IpAddress : IEquatable<IpAddress>
{
    public string Value { get; }

    private IpAddress(string value)
    {
        Value = value;
    }

    public static IpAddress Create(string ipAddress)
    {
        if (string.IsNullOrWhiteSpace(ipAddress))
            throw new ArgumentException("IP address cannot be empty", nameof(ipAddress));

        if (!IPAddress.TryParse(ipAddress, out var parsed))
            throw new ArgumentException("Invalid IP address format", nameof(ipAddress));

        // Validate that it's IPv4
        if (parsed.AddressFamily != AddressFamily.InterNetwork)
            throw new ArgumentException("Only IPv4 addresses are supported", nameof(ipAddress));

        return new IpAddress(parsed.ToString());
    }

    public bool Equals(IpAddress? other) =>
        other is not null && Value == other.Value;

    public override bool Equals(object? obj) =>
        Equals(obj as IpAddress);

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value;

    public static implicit operator string(IpAddress ipAddress) => ipAddress.Value;
}