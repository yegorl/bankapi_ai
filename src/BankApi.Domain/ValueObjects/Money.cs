using BankApi.Domain.Common;
using BankApi.Domain.Exceptions;

namespace BankApi.Domain.ValueObjects;

/// <summary>
/// Represents a monetary value with currency
/// </summary>
public sealed class Money : ValueObject
{
    public decimal Amount { get; }
    public string Currency { get; }

    private Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }

    /// <summary>
    /// Creates a Money instance with validation
    /// </summary>
    /// <param name="amount">The monetary amount</param>
    /// <param name="currency">The currency code (e.g., USD, EUR)</param>
    /// <returns>A new Money instance</returns>
    public static Money Create(decimal amount, string currency)
    {
        if (string.IsNullOrWhiteSpace(currency))
            throw new ValidationException("Currency cannot be empty");

        if (currency.Length != 3)
            throw new ValidationException("Currency must be a 3-letter code");

        return new Money(amount, currency.ToUpperInvariant());
    }

    public static Money Zero(string currency) => new(0, currency);

    public Money Add(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationDomainException($"Cannot add different currencies: {Currency} and {other.Currency}");

        return new Money(Amount + other.Amount, Currency);
    }

    public Money Subtract(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationDomainException($"Cannot subtract different currencies: {Currency} and {other.Currency}");

        return new Money(Amount - other.Amount, Currency);
    }

    public bool IsGreaterThanOrEqual(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationDomainException($"Cannot compare different currencies: {Currency} and {other.Currency}");

        return Amount >= other.Amount;
    }

    public bool IsPositive() => Amount > 0;
    public bool IsNegative() => Amount < 0;
    public bool IsZero() => Amount == 0;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }

    public override string ToString() => $"{Amount:F2} {Currency}";
}
