using BankApi.Application.Common;
using BankApi.Application.DTOs;
using BankApi.Domain.Aggregates.Cards;
using BankApi.Domain.Repositories;
using MediatR;

namespace BankApi.Application.Commands.Cards;

public class RequestCardCommandHandler : IRequestHandler<RequestCardCommand, CardDto>
{
    private readonly ICardRepository _cardRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RequestCardCommandHandler(
        ICardRepository cardRepository,
        IAccountRepository accountRepository,
        IUnitOfWork unitOfWork)
    {
        _cardRepository = cardRepository;
        _accountRepository = accountRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CardDto> Handle(RequestCardCommand request, CancellationToken cancellationToken)
    {
        // Verify account exists
        var account = await _accountRepository.GetByIdAsync(request.Request.AccountId);
        if (account == null)
            throw new InvalidOperationException($"Account with ID {request.Request.AccountId} not found");

        // Parse card type
        if (!Enum.TryParse<CardType>(request.Request.CardType, true, out var cardType))
            throw new ArgumentException($"Invalid card type: {request.Request.CardType}");

        // Create card
        var card = Card.Create(
            request.Request.AccountId,
            request.Request.CardHolderName,
            request.Request.CVV,
            cardType);

        await _cardRepository.AddAsync(card);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CardDto(
            card.Id,
            CardNumberFormatter.MaskCardNumber(card.CardNumber.Value),
            card.AccountId,
            card.CardHolderName,
            card.ExpirationDate,
            card.IsBlocked,
            card.IsTemporarilyBlocked,
            card.CardType.ToString(),
            card.CreatedAt);
    }
}
