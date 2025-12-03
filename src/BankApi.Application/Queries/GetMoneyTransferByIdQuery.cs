using BankApi.Application.DTOs;
using MediatR;

namespace BankApi.Application.Queries;

public record GetMoneyTransferByIdQuery(Guid Id) : IRequest<MoneyTransferDto?>;
