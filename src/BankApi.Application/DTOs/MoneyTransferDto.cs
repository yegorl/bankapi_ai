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

public record MoneyTransferResponse(
    Guid Id,
    string Status);

public record ExecuteCardTransferRequest(
    decimal Amount,
    string CardNumber);
