using BankApi.Application.DTOs;
using MediatR;

namespace BankApi.Application.Queries;

public record GetAccountStatementQuery(Guid AccountId, DateTime? From, DateTime? To) : IRequest<IEnumerable<TransactionDto>>;
