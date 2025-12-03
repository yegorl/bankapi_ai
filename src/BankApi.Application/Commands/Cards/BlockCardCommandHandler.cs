using BankApi.Domain.Repositories;
using MediatR;

namespace BankApi.Application.Commands.Cards;

public class BlockCardCommandHandler : IRequestHandler<BlockCardCommand, Unit>
{
    private readonly ICardRepository _cardRepository;
    private readonly IUnitOfWork _unitOfWork;

    public BlockCardCommandHandler(ICardRepository cardRepository, IUnitOfWork unitOfWork)
    {
        _cardRepository = cardRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(BlockCardCommand request, CancellationToken cancellationToken)
    {
        var card = await _cardRepository.GetByIdAsync(request.CardId);
        if (card == null)
            throw new InvalidOperationException($"Card with ID {request.CardId} not found");

        card.Block();
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
