using BankApi.Domain.Repositories;
using MediatR;

namespace BankApi.Application.Commands.Cards;

public class TemporarilyBlockCardCommandHandler : IRequestHandler<TemporarilyBlockCardCommand, Unit>
{
    private readonly ICardRepository _cardRepository;
    private readonly IUnitOfWork _unitOfWork;

    public TemporarilyBlockCardCommandHandler(ICardRepository cardRepository, IUnitOfWork unitOfWork)
    {
        _cardRepository = cardRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(TemporarilyBlockCardCommand request, CancellationToken cancellationToken)
    {
        var card = await _cardRepository.GetByIdAsync(request.CardId);
        if (card == null)
            throw new InvalidOperationException($"Card with ID {request.CardId} not found");

        card.TemporarilyBlock();
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
