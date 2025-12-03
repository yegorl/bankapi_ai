using MediatR;

namespace BankApi.Application.Commands.Cards;

public record UnblockCardCommand(Guid CardId) : IRequest<Unit>;
