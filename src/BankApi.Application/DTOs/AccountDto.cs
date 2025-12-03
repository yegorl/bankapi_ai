namespace BankApi.Application.DTOs;

public record AccountDto(
    Guid Id,
    string AccountNumber,
    string AccountHolderId,
    decimal Balance,
    string Currency,
    string? Description,
    DateTime CreatedAt,
    DateTime UpdatedAt);

public record CreateAccountRequest(
    string AccountHolderId,
    string Currency,
    string? Description);

public record UpdateAccountRequest(
    string? Description);
