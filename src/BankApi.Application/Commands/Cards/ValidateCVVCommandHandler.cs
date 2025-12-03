using BankApi.Domain.Repositories;
using MediatR;

namespace BankApi.Application.Commands.Cards;

public class ValidateCVVCommandHandler : IRequestHandler<ValidateCVVCommand, bool>
{
    private readonly ICardRepository _cardRepository;

    public ValidateCVVCommandHandler(ICardRepository cardRepository)
    {
        _cardRepository = cardRepository;
    }

    public async Task<bool> Handle(ValidateCVVCommand request, CancellationToken cancellationToken)
    {
        var card = await _cardRepository.GetByIdAsync(request.Request.CardId);
        if (card == null)
            return false;

        return card.ValidateCVV(request.Request.CVV);
    }
}
