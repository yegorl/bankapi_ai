namespace BankApi.Application.DTOs;

public record CardDto(
    Guid Id,
    string CardNumberMasked,
    Guid AccountId,
    string CardHolderName,
    DateTime ExpirationDate,
    bool IsBlocked,
    bool IsTemporarilyBlocked,
    string CardType,
    DateTime CreatedAt);

public record RequestCardRequest(
    Guid AccountId,
    string CardHolderName,
    string CVV,
    string CardType);

public record ValidateCVVRequest(
    Guid CardId,
    string CVV);
