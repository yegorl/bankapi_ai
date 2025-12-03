using BankApi.Application.DTOs;
using BankApi.Domain.Repositories;
using MediatR;

namespace BankApi.Application.Queries;

public class GetTransactionByIdQueryHandler : IRequestHandler<GetTransactionByIdQuery, TransactionDto?>
{
    private readonly ITransactionRepository _transactionRepository;

    public GetTransactionByIdQueryHandler(ITransactionRepository transactionRepository)
    {
        _transactionRepository = transactionRepository;
    }

    public async Task<TransactionDto?> Handle(GetTransactionByIdQuery request, CancellationToken cancellationToken)
    {
        var transaction = await _transactionRepository.GetByIdAsync(request.TransactionId);
        if (transaction == null)
            return null;

        return new TransactionDto(
            transaction.Id,
            transaction.SourceAccountId,
            transaction.TargetAccountId,
            transaction.Amount.Amount,
            transaction.Amount.Currency,
            transaction.TransactionType.ToString(),
            transaction.Status.ToString(),
            transaction.Description,
            transaction.CreatedAt);
    }
}
