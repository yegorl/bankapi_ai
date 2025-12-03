using BankApi.Application.Common;
using BankApi.Application.DTOs;
using BankApi.Domain.Repositories;
using MediatR;

namespace BankApi.Application.Queries;

public class GetCardByIdQueryHandler : IRequestHandler<GetCardByIdQuery, CardDto?>
{
    private readonly ICardRepository _cardRepository;

    public GetCardByIdQueryHandler(ICardRepository cardRepository)
    {
        _cardRepository = cardRepository;
    }

    public async Task<CardDto?> Handle(GetCardByIdQuery request, CancellationToken cancellationToken)
    {
        var card = await _cardRepository.GetByIdAsync(request.CardId);
        if (card == null)
            return null;

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
