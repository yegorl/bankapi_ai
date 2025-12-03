using BankApi.Application.DTOs;
using MediatR;

namespace BankApi.Application.Commands.Transactions;

public record ExecuteTransferCommand(ExecuteTransferRequest Request) : IRequest<TransactionDto>;
