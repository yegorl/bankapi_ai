using BankApi.Application.DTOs;
using MediatR;

namespace BankApi.Application.Queries;

public record GetAccountHolderByIdQuery(string Id) : IRequest<AccountHolderDto?>;
