using BankApi.Application.DTOs;
using MediatR;

namespace BankApi.Application.Queries;

public record GetTransactionByIdQuery(Guid TransactionId) : IRequest<TransactionDto?>;
