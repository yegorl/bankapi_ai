using BankApi.Application.DTOs;
using MediatR;

namespace BankApi.Application.Commands.Accounts;

public record CreateAccountCommand(CreateAccountRequest Request) : IRequest<AccountDto>;
