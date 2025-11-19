using System.Text.RegularExpressions;

namespace EasyPeasy_Login.Domain.ValueObjects;

public sealed class MacAddress : IEquatable<MacAddress>
{
    private static readonly Regex MacRegex = new(
        @"^([0-9A-Fa-f]{2}[:-]){5}([0-9A-Fa-f]{2})$|^([0-9A-Fa-f]{12})$",
        RegexOptions.Compiled);

    public string Value { get; }

    private MacAddress(string value)
    {
        Value = value;
    }

    public static MacAddress Create(string macAddress)
    {
        if (string.IsNullOrWhiteSpace(macAddress))
            throw new ArgumentException("MAC address cannot be empty", nameof(macAddress));

        var normalized = Normalize(macAddress);

        if (!MacRegex.IsMatch(normalized))
            throw new ArgumentException("Invalid MAC address format", nameof(macAddress));

        return new MacAddress(normalized);
    }

    private static string Normalize(string mac)
    {
        // Remove common separators and convert to uppercase
        var cleaned = mac.Replace(":", "").Replace("-", "").Replace(".", "").ToUpperInvariant();

        if (cleaned.Length != 12)
            return mac; // Return original if length is wrong for further validation

        // Format as XX:XX:XX:XX:XX:XX
        return string.Join(":", Enumerable.Range(0, 6)
            .Select(i => cleaned.Substring(i * 2, 2)));
    }

    public bool Equals(MacAddress? other) =>
        other is not null && Value == other.Value;

    public override bool Equals(object? obj) =>
        Equals(obj as MacAddress);

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value;

    public static implicit operator string(MacAddress macAddress) => macAddress.Value;
}