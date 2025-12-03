using BankApi.Domain.Common;
using BankApi.Domain.Exceptions;

namespace BankApi.Domain.ValueObjects;

/// <summary>
/// Represents a credit card number with validation
/// </summary>
public sealed class CardNumber : ValueObject
{
    public string Value { get; }

    private CardNumber(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Creates a CardNumber with validation
    /// </summary>
    /// <param name="value">The 16-digit card number</param>
    /// <returns>A new CardNumber instance</returns>
    public static CardNumber Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ValidationException("Card number cannot be empty");

        var cleanValue = value.Replace(" ", "").Replace("-", "");

        if (cleanValue.Length != 16)
            throw new ValidationException("Card number must be 16 digits");

        if (!cleanValue.All(char.IsDigit))
            throw new ValidationException("Card number must contain only digits");

        if (!IsValidLuhn(cleanValue))
            throw new ValidationException("Card number failed Luhn validation");

        return new CardNumber(cleanValue);
    }

    /// <summary>
    /// Generates a new random card number
    /// </summary>
    /// <returns>A new CardNumber instance</returns>
    public static CardNumber Generate()
    {
        var random = new Random();
        var number = "4"; // Visa prefix
        
        for (int i = 0; i < 14; i++)
        {
            number += random.Next(0, 10);
        }

        // Calculate Luhn check digit
        number += CalculateLuhnCheckDigit(number);

        return new CardNumber(number);
    }

    /// <summary>
    /// Gets the masked version of the card number (shows only last 4 digits)
    /// </summary>
    /// <returns>Masked card number</returns>
    public string GetMasked()
    {
        return $"****-****-****-{Value[^4..]}";
    }

    private static bool IsValidLuhn(string number)
    {
        int sum = 0;
        bool alternate = false;

        for (int i = number.Length - 1; i >= 0; i--)
        {
            int digit = int.Parse(number[i].ToString());

            if (alternate)
            {
                digit *= 2;
                if (digit > 9)
                    digit -= 9;
            }

            sum += digit;
            alternate = !alternate;
        }

        return sum % 10 == 0;
    }

    private static int CalculateLuhnCheckDigit(string number)
    {
        int sum = 0;
        bool alternate = true;

        for (int i = number.Length - 1; i >= 0; i--)
        {
            int digit = int.Parse(number[i].ToString());

            if (alternate)
            {
                digit *= 2;
                if (digit > 9)
                    digit -= 9;
            }

            sum += digit;
            alternate = !alternate;
        }

        return (10 - (sum % 10)) % 10;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => GetMasked();

    public static implicit operator string(CardNumber cardNumber) => cardNumber.Value;
}
