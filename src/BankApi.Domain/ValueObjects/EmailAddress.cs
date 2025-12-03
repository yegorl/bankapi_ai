using BankApi.Domain.Common;
using BankApi.Domain.Exceptions;
using System.Text.RegularExpressions;

namespace BankApi.Domain.ValueObjects;

/// <summary>
/// Represents an email address with validation
/// </summary>
public sealed partial class EmailAddress : ValueObject
{
    public string Value { get; }

    private EmailAddress(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Creates an EmailAddress with validation
    /// </summary>
    /// <param name="value">The email address</param>
    /// <returns>A new EmailAddress instance</returns>
    public static EmailAddress Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ValidationException("Email address cannot be empty");

        if (!EmailRegex().IsMatch(value))
            throw new ValidationException("Invalid email address format");

        return new EmailAddress(value.ToLowerInvariant());
    }

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex EmailRegex();

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    public static implicit operator string(EmailAddress email) => email.Value;
}
