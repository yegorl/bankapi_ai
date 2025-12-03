using BankApi.Application.DTOs;
using MediatR;

namespace BankApi.Application.Queries;

public record GetAccountsByHolderIdQuery(string HolderId) : IRequest<IEnumerable<AccountDto>>;
