using BankApi.Domain.Common;
using BankApi.Domain.Events;
using BankApi.Domain.Exceptions;
using BankApi.Domain.ValueObjects;

namespace BankApi.Domain.Aggregates.Accounts;

/// <summary>
/// Represents a bank account aggregate root
/// </summary>
public class Account : AggregateRoot<Guid>
{
    public AccountNumber AccountNumber { get; private set; } = null!;
    public string AccountHolderId { get; private set; } = string.Empty;
    public Money Balance { get; private set; } = null!;
    public string? Description { get; private set; }

    // Required by EF Core
    private Account() { }

    private Account(Guid id, AccountNumber accountNumber, string accountHolderId, Money initialBalance, string? description)
    {
        Id = id;
        AccountNumber = accountNumber;
        AccountHolderId = accountHolderId;
        Balance = initialBalance;
        Description = description;
    }

    /// <summary>
    /// Creates a new Account
    /// </summary>
    public static Account Create(string accountHolderId, string currency, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(accountHolderId))
            throw new ValidationException("Account holder ID cannot be empty");

        var id = Guid.NewGuid();
        var accountNumber = AccountNumber.Generate();
        var initialBalance = Money.Zero(currency);

        var account = new Account(id, accountNumber, accountHolderId, initialBalance, description);

        account.AddDomainEvent(new AccountCreatedEvent(
            id,
            accountNumber.Value,
            accountHolderId,
            currency,
            DateTime.UtcNow));

        return account;
    }

    /// <summary>
    /// Updates the account description
    /// </summary>
    public void UpdateDescription(string? description)
    {
        if (IsDeleted)
            throw new InvalidOperationDomainException("Cannot update a deleted account");

        Description = description;
        UpdateTimestamp();
    }

    /// <summary>
    /// Debits the account (withdraws money)
    /// </summary>
    public void Debit(Money amount)
    {
        if (IsDeleted)
            throw new InvalidOperationDomainException("Cannot debit a deleted account");

        if (amount.Currency != Balance.Currency)
            throw new InvalidOperationDomainException($"Currency mismatch: account currency is {Balance.Currency}, transaction currency is {amount.Currency}");

        if (!amount.IsPositive())
            throw new ValidationException("Debit amount must be positive");

        if (!Balance.IsGreaterThanOrEqual(amount))
            throw new InsufficientBalanceException(Balance.Amount, amount.Amount);

        var previousBalance = Balance;
        Balance = Balance.Subtract(amount);
        UpdateTimestamp();

        AddDomainEvent(new BalanceChangedEvent(
            Id,
            AccountNumber.Value,
            previousBalance.Amount,
            Balance.Amount,
            Balance.Currency,
            "Debit",
            DateTime.UtcNow));
    }

    /// <summary>
    /// Credits the account (deposits money)
    /// </summary>
    public void Credit(Money amount)
    {
        if (IsDeleted)
            throw new InvalidOperationDomainException("Cannot credit a deleted account");

        if (amount.Currency != Balance.Currency)
            throw new InvalidOperationDomainException($"Currency mismatch: account currency is {Balance.Currency}, transaction currency is {amount.Currency}");

        if (!amount.IsPositive())
            throw new ValidationException("Credit amount must be positive");

        var previousBalance = Balance;
        Balance = Balance.Add(amount);
        UpdateTimestamp();

        AddDomainEvent(new BalanceChangedEvent(
            Id,
            AccountNumber.Value,
            previousBalance.Amount,
            Balance.Amount,
            Balance.Currency,
            "Credit",
            DateTime.UtcNow));
    }

    /// <summary>
    /// Marks the account as deleted
    /// </summary>
    public override void MarkAsDeleted()
    {
        base.MarkAsDeleted();

        AddDomainEvent(new AccountDeletedEvent(Id, AccountNumber.Value, AccountHolderId, DateTime.UtcNow));
    }
}
