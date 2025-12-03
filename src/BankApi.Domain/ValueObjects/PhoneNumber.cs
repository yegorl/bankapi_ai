using BankApi.Domain.Common;
using BankApi.Domain.Exceptions;
using System.Text.RegularExpressions;

namespace BankApi.Domain.ValueObjects;

/// <summary>
/// Represents a phone number with validation
/// </summary>
public sealed partial class PhoneNumber : ValueObject
{
    public string Value { get; }

    private PhoneNumber(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Creates a PhoneNumber with validation
    /// </summary>
    /// <param name="value">The phone number</param>
    /// <returns>A new PhoneNumber instance</returns>
    public static PhoneNumber Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ValidationException("Phone number cannot be empty");

        // Remove common formatting characters
        var cleanValue = value.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "");

        if (!PhoneRegex().IsMatch(cleanValue))
            throw new ValidationException("Invalid phone number format. Use international format with +");

        return new PhoneNumber(cleanValue);
    }

    [GeneratedRegex(@"^\+[1-9]\d{6,14}$")]
    private static partial Regex PhoneRegex();

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    public static implicit operator string(PhoneNumber phone) => phone.Value;
}
