using BankApi.Application.DTOs;
using MediatR;

namespace BankApi.Application.Commands.AccountHolders;

public record CreateAccountHolderCommand(CreateAccountHolderRequest Request) : IRequest<AccountHolderDto>;
