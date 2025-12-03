using BankApi.Domain.Repositories;
using MediatR;

namespace BankApi.Application.Commands.Cards;

public class UnblockCardCommandHandler : IRequestHandler<UnblockCardCommand, Unit>
{
    private readonly ICardRepository _cardRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UnblockCardCommandHandler(ICardRepository cardRepository, IUnitOfWork unitOfWork)
    {
        _cardRepository = cardRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(UnblockCardCommand request, CancellationToken cancellationToken)
    {
        var card = await _cardRepository.GetByIdAsync(request.CardId);
        if (card == null)
            throw new InvalidOperationException($"Card with ID {request.CardId} not found");

        card.Unblock();
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
