using BankApi.Domain.Common;
using BankApi.Domain.Exceptions;

namespace BankApi.Domain.ValueObjects;

/// <summary>
/// Represents an account number with generation and validation logic
/// </summary>
public sealed class AccountNumber : ValueObject
{
    public string Value { get; }

    private AccountNumber(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Creates an AccountNumber from an existing value
    /// </summary>
    /// <param name="value">The account number value</param>
    /// <returns>A new AccountNumber instance</returns>
    public static AccountNumber Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ValidationException("Account number cannot be empty");

        if (!value.StartsWith("ACC-"))
            throw new ValidationException("Account number must start with 'ACC-'");

        if (value.Length != 12) // ACC-XXXXXXXX
            throw new ValidationException("Account number must be in format ACC-XXXXXXXX");

        return new AccountNumber(value);
    }

    /// <summary>
    /// Generates a new unique account number
    /// </summary>
    /// <returns>A new AccountNumber instance</returns>
    public static AccountNumber Generate()
    {
        var random = new Random();
        var number = random.Next(10000000, 99999999);
        return new AccountNumber($"ACC-{number}");
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    public static implicit operator string(AccountNumber accountNumber) => accountNumber.Value;
}
