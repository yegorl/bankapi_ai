namespace BankApi.Application.Common;

/// <summary>
/// Utility class for formatting card numbers
/// </summary>
public static class CardNumberFormatter
{
    /// <summary>
    /// Masks a card number to show only the last 4 digits
    /// </summary>
    /// <param name="cardNumber">The full card number</param>
    /// <returns>Masked card number in format ****-****-****-1234</returns>
    public static string MaskCardNumber(string cardNumber)
    {
        if (cardNumber.Length < 4)
            return cardNumber;

        return $"****-****-****-{cardNumber[^4..]}";
    }
}
