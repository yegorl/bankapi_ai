using BankApi.Application.DTOs;
using BankApi.Domain.Repositories;
using MediatR;

namespace BankApi.Application.Queries;

public class GetAccountStatementQueryHandler : IRequestHandler<GetAccountStatementQuery, IEnumerable<TransactionDto>>
{
    private readonly ITransactionRepository _transactionRepository;

    public GetAccountStatementQueryHandler(ITransactionRepository transactionRepository)
    {
        _transactionRepository = transactionRepository;
    }

    public async Task<IEnumerable<TransactionDto>> Handle(GetAccountStatementQuery request, CancellationToken cancellationToken)
    {
        var transactions = await _transactionRepository.GetByAccountIdAsync(
            request.AccountId,
            request.From,
            request.To);

        return transactions.Select(t => new TransactionDto(
            t.Id,
            t.SourceAccountId,
            t.TargetAccountId,
            t.Amount.Amount,
            t.Amount.Currency,
            t.TransactionType.ToString(),
            t.Status.ToString(),
            t.Description,
            t.CreatedAt));
    }
}
