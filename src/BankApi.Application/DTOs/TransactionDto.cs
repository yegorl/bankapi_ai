namespace BankApi.Application.DTOs;

public record TransactionDto(
    Guid Id,
    Guid? SourceAccountId,
    Guid? TargetAccountId,
    decimal Amount,
    string Currency,
    string TransactionType,
    string Status,
    string? Description,
    DateTime CreatedAt);

public record CreateTransactionRequest(
    Guid? SourceAccountId,
    Guid? TargetAccountId,
    decimal Amount,
    string Currency,
    string TransactionType,
    string? Description);

public record ExecuteTransferRequest(
    Guid SourceAccountId,
    Guid TargetAccountId,
    decimal Amount,
    string? Description);
