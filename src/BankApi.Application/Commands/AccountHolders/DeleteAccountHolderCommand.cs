using MediatR;

namespace BankApi.Application.Commands.AccountHolders;

public record DeleteAccountHolderCommand(string Id) : IRequest<bool>;
