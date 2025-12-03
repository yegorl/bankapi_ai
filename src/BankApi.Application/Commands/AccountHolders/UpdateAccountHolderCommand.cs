using BankApi.Application.DTOs;
using MediatR;

namespace BankApi.Application.Commands.AccountHolders;

public record UpdateAccountHolderCommand(string Id, UpdateAccountHolderRequest Request) : IRequest<AccountHolderDto?>;
