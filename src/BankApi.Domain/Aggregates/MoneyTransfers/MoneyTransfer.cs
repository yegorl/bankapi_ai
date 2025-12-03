using BankApi.Domain.Common;
using BankApi.Domain.Events;
using BankApi.Domain.Exceptions;
using BankApi.Domain.ValueObjects;

namespace BankApi.Domain.Aggregates.MoneyTransfers;

/// <summary>
/// Represents a money transfer between cards aggregate root
/// </summary>
public class MoneyTransfer : AggregateRoot<Guid>
{
    public string SourceCardNumber { get; private set; } = string.Empty;
    public string TargetCardNumber { get; private set; } = string.Empty;
    public Guid SourceAccountId { get; private set; }
    public Guid TargetAccountId { get; private set; }
    public Money Amount { get; private set; } = null!;
    public MoneyTransferStatus Status { get; private set; }
    public string? Description { get; private set; }
    public string? FailureReason { get; private set; }

    // Required by EF Core
    private MoneyTransfer() { }

    private MoneyTransfer(
        Guid id,
        string sourceCardNumber,
        string targetCardNumber,
        Guid sourceAccountId,
        Guid targetAccountId,
        Money amount,
        string? description)
    {
        Id = id;
        SourceCardNumber = sourceCardNumber;
        TargetCardNumber = targetCardNumber;
        SourceAccountId = sourceAccountId;
        TargetAccountId = targetAccountId;
        Amount = amount;
        Status = MoneyTransferStatus.Pending;
        Description = description;
    }

    /// <summary>
    /// Creates a new MoneyTransfer
    /// </summary>
    public static MoneyTransfer Create(
        string sourceCardNumber,
        string targetCardNumber,
        Guid sourceAccountId,
        Guid targetAccountId,
        Money amount,
        string? description = null)
    {
        if (string.IsNullOrWhiteSpace(sourceCardNumber))
            throw new ValidationException("Source card number cannot be empty");

        if (string.IsNullOrWhiteSpace(targetCardNumber))
            throw new ValidationException("Target card number cannot be empty");

        if (sourceCardNumber == targetCardNumber)
            throw new ValidationException("Source and target cards cannot be the same");

        if (sourceAccountId == Guid.Empty)
            throw new ValidationException("Source account ID cannot be empty");

        if (targetAccountId == Guid.Empty)
            throw new ValidationException("Target account ID cannot be empty");

        if (sourceAccountId == targetAccountId)
            throw new ValidationException("Source and target accounts cannot be the same");

        if (!amount.IsPositive())
            throw new ValidationException("Transfer amount must be positive");

        var id = Guid.NewGuid();
        var transfer = new MoneyTransfer(
            id,
            sourceCardNumber,
            targetCardNumber,
            sourceAccountId,
            targetAccountId,
            amount,
            description);

        transfer.AddDomainEvent(new MoneyTransferCreatedEvent(
            id,
            sourceCardNumber,
            targetCardNumber,
            amount.Amount,
            amount.Currency,
            DateTime.UtcNow));

        return transfer;
    }

    /// <summary>
    /// Marks the transfer as executed
    /// </summary>
    public void Execute()
    {
        if (IsDeleted)
            throw new InvalidOperationDomainException("Cannot execute a deleted transfer");

        if (Status != MoneyTransferStatus.Pending)
            throw new InvalidOperationDomainException($"Cannot execute transfer in {Status} status");

        Status = MoneyTransferStatus.Completed;
        UpdateTimestamp();

        AddDomainEvent(new MoneyTransferCompletedEvent(
            Id,
            SourceCardNumber,
            TargetCardNumber,
            Amount.Amount,
            Amount.Currency,
            DateTime.UtcNow));
    }

    /// <summary>
    /// Marks the transfer as failed
    /// </summary>
    public void Fail(string reason)
    {
        if (IsDeleted)
            throw new InvalidOperationDomainException("Cannot fail a deleted transfer");

        if (Status != MoneyTransferStatus.Pending)
            throw new InvalidOperationDomainException($"Cannot fail transfer in {Status} status");

        Status = MoneyTransferStatus.Failed;
        FailureReason = reason;
        UpdateTimestamp();

        AddDomainEvent(new MoneyTransferFailedEvent(
            Id,
            SourceCardNumber,
            TargetCardNumber,
            reason,
            DateTime.UtcNow));
    }
}

/// <summary>
/// Represents the status of a money transfer
/// </summary>
public enum MoneyTransferStatus
{
    Pending,
    Completed,
    Failed
}
