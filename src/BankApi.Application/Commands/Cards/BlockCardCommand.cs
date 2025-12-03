using MediatR;

namespace BankApi.Application.Commands.Cards;

public record BlockCardCommand(Guid CardId) : IRequest<Unit>;
