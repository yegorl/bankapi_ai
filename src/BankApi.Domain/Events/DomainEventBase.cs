using BankApi.Domain.Common;

namespace BankApi.Domain.Events;

/// <summary>
/// Base class for domain events
/// </summary>
public abstract record DomainEventBase : IDomainEvent
{
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
}

// AccountHolder Events
public record AccountHolderCreatedEvent(string AccountHolderId, string Email, string FullName, DateTime OccurredOn) : DomainEventBase
{
    public DateTime OccurredOn { get; init; } = OccurredOn;
}
public record AccountHolderDeletedEvent(string AccountHolderId, string Email, DateTime OccurredOn) : DomainEventBase
{
    public DateTime OccurredOn { get; init; } = OccurredOn;
}

// Account Events
public record AccountCreatedEvent(Guid AccountId, string AccountNumber, string AccountHolderId, string Currency, DateTime OccurredOn) : DomainEventBase
{
    public DateTime OccurredOn { get; init; } = OccurredOn;
}
public record AccountDeletedEvent(Guid AccountId, string AccountNumber, string AccountHolderId, DateTime OccurredOn) : DomainEventBase
{
    public DateTime OccurredOn { get; init; } = OccurredOn;
}
public record BalanceChangedEvent(Guid AccountId, string AccountNumber, decimal PreviousBalance, decimal NewBalance, string Currency, string ChangeType, DateTime OccurredOn) : DomainEventBase
{
    public DateTime OccurredOn { get; init; } = OccurredOn;
}

// Card Events
public record CardCreatedEvent(Guid CardId, string CardNumber, Guid AccountId, string CardHolderName, string CardType, DateTime OccurredOn) : DomainEventBase
{
    public DateTime OccurredOn { get; init; } = OccurredOn;
}
public record CardBlockedEvent(Guid CardId, string CardNumber, bool IsPermanent, DateTime OccurredOn) : DomainEventBase
{
    public DateTime OccurredOn { get; init; } = OccurredOn;
}
public record CardUnblockedEvent(Guid CardId, string CardNumber, DateTime OccurredOn) : DomainEventBase
{
    public DateTime OccurredOn { get; init; } = OccurredOn;
}

// Transaction Events
public record TransactionCreatedEvent(Guid TransactionId, Guid? SourceAccountId, Guid? TargetAccountId, decimal Amount, string Currency, string TransactionType, DateTime OccurredOn) : DomainEventBase
{
    public DateTime OccurredOn { get; init; } = OccurredOn;
}
public record TransactionExecutedEvent(Guid TransactionId, Guid? SourceAccountId, Guid? TargetAccountId, decimal Amount, string Currency, DateTime OccurredOn) : DomainEventBase
{
    public DateTime OccurredOn { get; init; } = OccurredOn;
}
public record TransactionFailedEvent(Guid TransactionId, Guid? SourceAccountId, Guid? TargetAccountId, decimal Amount, string Currency, string Reason, DateTime OccurredOn) : DomainEventBase
{
    public DateTime OccurredOn { get; init; } = OccurredOn;
}

// User Events
public record UserCreatedEvent(Guid UserId, string Email, string Role, DateTime OccurredOn) : DomainEventBase
{
    public DateTime OccurredOn { get; init; } = OccurredOn;
}

// MoneyTransfer Events
public record MoneyTransferCreatedEvent(Guid MoneyTransferId, string SourceCardNumber, string TargetCardNumber, decimal Amount, string Currency, DateTime OccurredOn) : DomainEventBase
{
    public DateTime OccurredOn { get; init; } = OccurredOn;
}
public record MoneyTransferCompletedEvent(Guid MoneyTransferId, string SourceCardNumber, string TargetCardNumber, decimal Amount, string Currency, DateTime OccurredOn) : DomainEventBase
{
    public DateTime OccurredOn { get; init; } = OccurredOn;
}
public record MoneyTransferFailedEvent(Guid MoneyTransferId, string SourceCardNumber, string TargetCardNumber, string Reason, DateTime OccurredOn) : DomainEventBase
{
    public DateTime OccurredOn { get; init; } = OccurredOn;
}
