using BankApi.Domain.Common;
using BankApi.Domain.Events;
using BankApi.Domain.Exceptions;
using BankApi.Domain.ValueObjects;

namespace BankApi.Domain.Aggregates.Transactions;

/// <summary>
/// Represents a financial transaction aggregate root
/// </summary>
public class Transaction : AggregateRoot<Guid>
{
    public Guid? SourceAccountId { get; private set; }
    public Guid? TargetAccountId { get; private set; }
    public Money Amount { get; private set; } = null!;
    public TransactionType TransactionType { get; private set; }
    public string? Description { get; private set; }
    public TransactionStatus Status { get; private set; }

    // Required by EF Core
    private Transaction() { }

    private Transaction(Guid id, Guid? sourceAccountId, Guid? targetAccountId, Money amount, TransactionType type, string? description)
    {
        Id = id;
        SourceAccountId = sourceAccountId;
        TargetAccountId = targetAccountId;
        Amount = amount;
        TransactionType = type;
        Description = description;
        Status = TransactionStatus.Pending;
    }

    /// <summary>
    /// Creates a new Transaction
    /// </summary>
    public static Transaction Create(Guid? sourceAccountId, Guid? targetAccountId, Money amount, TransactionType type, string? description = null)
    {
        // Validation based on transaction type
        switch (type)
        {
            case TransactionType.Transfer:
                if (sourceAccountId is null || targetAccountId is null)
                    throw new ValidationException("Transfer requires both source and target account");
                if (sourceAccountId == targetAccountId)
                    throw new ValidationException("Cannot transfer to the same account");
                break;

            case TransactionType.Deposit:
                if (targetAccountId is null)
                    throw new ValidationException("Deposit requires a target account");
                break;

            case TransactionType.Withdrawal:
                if (sourceAccountId is null)
                    throw new ValidationException("Withdrawal requires a source account");
                break;
        }

        if (!amount.IsPositive())
            throw new ValidationException("Transaction amount must be positive");

        var id = Guid.NewGuid();
        var transaction = new Transaction(id, sourceAccountId, targetAccountId, amount, type, description);

        transaction.AddDomainEvent(new TransactionCreatedEvent(
            id,
            sourceAccountId,
            targetAccountId,
            amount.Amount,
            amount.Currency,
            type.ToString(),
            DateTime.UtcNow));

        return transaction;
    }

    /// <summary>
    /// Marks the transaction as executed
    /// </summary>
    public void Execute()
    {
        if (Status != TransactionStatus.Pending)
            throw new InvalidOperationDomainException($"Cannot execute transaction in {Status} status");

        Status = TransactionStatus.Completed;
        UpdateTimestamp();

        AddDomainEvent(new TransactionExecutedEvent(
            Id,
            SourceAccountId,
            TargetAccountId,
            Amount.Amount,
            Amount.Currency,
            DateTime.UtcNow));
    }

    /// <summary>
    /// Marks the transaction as failed and initiates rollback
    /// </summary>
    public void Rollback(string reason)
    {
        if (Status == TransactionStatus.Completed)
            throw new InvalidOperationDomainException("Cannot rollback a completed transaction");

        Status = TransactionStatus.Failed;
        Description = $"{Description} [Rollback: {reason}]";
        UpdateTimestamp();

        AddDomainEvent(new TransactionFailedEvent(
            Id,
            SourceAccountId,
            TargetAccountId,
            Amount.Amount,
            Amount.Currency,
            reason,
            DateTime.UtcNow));
    }
}

/// <summary>
/// Represents the type of transaction
/// </summary>
public enum TransactionType
{
    Deposit,
    Withdrawal,
    Transfer
}

/// <summary>
/// Represents the status of a transaction
/// </summary>
public enum TransactionStatus
{
    Pending,
    Completed,
    Failed
}
