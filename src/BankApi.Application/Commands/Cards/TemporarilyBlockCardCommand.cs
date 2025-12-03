using MediatR;

namespace BankApi.Application.Commands.Cards;

public record TemporarilyBlockCardCommand(Guid CardId) : IRequest<Unit>;
