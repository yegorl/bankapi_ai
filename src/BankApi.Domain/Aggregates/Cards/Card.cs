using BankApi.Domain.Common;
using BankApi.Domain.Events;
using BankApi.Domain.Exceptions;
using BankApi.Domain.ValueObjects;

namespace BankApi.Domain.Aggregates.Cards;

/// <summary>
/// Represents a payment card aggregate root
/// </summary>
public class Card : AggregateRoot<Guid>
{
    public CardNumber CardNumber { get; private set; } = null!;
    public Guid AccountId { get; private set; }
    public string CardHolderName { get; private set; } = string.Empty;
    public DateTime ExpirationDate { get; private set; }
    public string CVVHash { get; private set; } = string.Empty;
    public bool IsBlocked { get; private set; }
    public bool IsTemporarilyBlocked { get; private set; }
    public CardType CardType { get; private set; }

    // Required by EF Core
    private Card() { }

    private Card(Guid id, CardNumber cardNumber, Guid accountId, string cardHolderName, DateTime expirationDate, string cvvHash, CardType cardType)
    {
        Id = id;
        CardNumber = cardNumber;
        AccountId = accountId;
        CardHolderName = cardHolderName;
        ExpirationDate = expirationDate;
        CVVHash = cvvHash;
        CardType = cardType;
        IsBlocked = false;
        IsTemporarilyBlocked = false;
    }

    /// <summary>
    /// Creates a new Card
    /// </summary>
    public static Card Create(Guid accountId, string cardHolderName, string cvv, CardType cardType)
    {
        if (accountId == Guid.Empty)
            throw new ValidationException("Account ID cannot be empty");

        if (string.IsNullOrWhiteSpace(cardHolderName))
            throw new ValidationException("Card holder name cannot be empty");

        if (string.IsNullOrWhiteSpace(cvv) || cvv.Length != 3 || !cvv.All(char.IsDigit))
            throw new ValidationException("CVV must be a 3-digit number");

        var id = Guid.NewGuid();
        var cardNumber = CardNumber.Generate();
        var expirationDate = DateTime.UtcNow.AddYears(3);
        var cvvHash = HashCVV(cvv);

        var card = new Card(id, cardNumber, accountId, cardHolderName, expirationDate, cvvHash, cardType);

        card.AddDomainEvent(new CardCreatedEvent(
            id,
            cardNumber.Value,
            accountId,
            cardHolderName,
            cardType.ToString(),
            DateTime.UtcNow));

        return card;
    }

    /// <summary>
    /// Blocks the card permanently
    /// </summary>
    public void Block()
    {
        if (IsDeleted)
            throw new InvalidOperationDomainException("Cannot block a deleted card");

        if (IsBlocked)
            throw new InvalidOperationDomainException("Card is already blocked");

        IsBlocked = true;
        IsTemporarilyBlocked = false;
        UpdateTimestamp();

        AddDomainEvent(new CardBlockedEvent(Id, CardNumber.Value, IsPermanent: true, DateTime.UtcNow));
    }

    /// <summary>
    /// Temporarily blocks the card
    /// </summary>
    public void TemporarilyBlock()
    {
        if (IsDeleted)
            throw new InvalidOperationDomainException("Cannot block a deleted card");

        if (IsBlocked)
            throw new InvalidOperationDomainException("Card is permanently blocked");

        if (IsTemporarilyBlocked)
            throw new InvalidOperationDomainException("Card is already temporarily blocked");

        IsTemporarilyBlocked = true;
        UpdateTimestamp();

        AddDomainEvent(new CardBlockedEvent(Id, CardNumber.Value, IsPermanent: false, DateTime.UtcNow));
    }

    /// <summary>
    /// Unblocks the card (only if temporarily blocked)
    /// </summary>
    public void Unblock()
    {
        if (IsDeleted)
            throw new InvalidOperationDomainException("Cannot unblock a deleted card");

        if (IsBlocked)
            throw new InvalidOperationDomainException("Cannot unblock a permanently blocked card");

        if (!IsTemporarilyBlocked)
            throw new InvalidOperationDomainException("Card is not blocked");

        IsTemporarilyBlocked = false;
        UpdateTimestamp();

        AddDomainEvent(new CardUnblockedEvent(Id, CardNumber.Value, DateTime.UtcNow));
    }

    /// <summary>
    /// Validates the provided CVV against the stored hash
    /// </summary>
    public bool ValidateCVV(string cvv)
    {
        if (string.IsNullOrWhiteSpace(cvv))
            return false;

        var hash = HashCVV(cvv);
        return hash == CVVHash;
    }

    /// <summary>
    /// Checks if the card is usable (not expired, not blocked)
    /// </summary>
    public bool IsUsable()
    {
        return !IsDeleted && !IsBlocked && !IsTemporarilyBlocked && ExpirationDate > DateTime.UtcNow;
    }

    private static string HashCVV(string cvv)
    {
        // In production, use a proper hashing algorithm like BCrypt
        // For now, using a simple hash (NOT SECURE - just for demonstration)
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var bytes = System.Text.Encoding.UTF8.GetBytes(cvv);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}

/// <summary>
/// Represents the type of card
/// </summary>
public enum CardType
{
    Debit,
    Credit,
    Prepaid
}
