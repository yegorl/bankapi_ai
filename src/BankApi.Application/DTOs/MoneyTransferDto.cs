namespace BankApi.Application.DTOs;

public record MoneyTransferDto(
    Guid Id,
    string SourceCardNumber,
    string TargetCardNumber,
    Guid SourceAccountId,
    Guid TargetAccountId,
    decimal Amount,
    string Currency,
    string Status,
    string? Description,
    string? FailureReason,
    DateTime CreatedAt);

public record ExecuteCardTransferRequest(
    string SourceCardNumber,
    string TargetCardNumber,
    decimal Amount,
    string? Description);
