using BankApi.Application.DTOs;
using MediatR;

namespace BankApi.Application.Queries;

public record GetAccountByIdQuery(Guid Id) : IRequest<AccountDto?>;
