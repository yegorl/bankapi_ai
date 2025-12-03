using BankApi.Application.DTOs;
using MediatR;

namespace BankApi.Application.Commands.MoneyTransfers;

public record ExecuteCardTransferCommand(
    ExecuteCardTransferRequest Request, 
    string SourceCardNumber) : IRequest<MoneyTransferResponse>;
