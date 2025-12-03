using BankApi.Domain.Aggregates.Transactions;

namespace BankApi.Domain.Repositories;

/// <summary>
/// Repository interface for Transaction aggregate
/// </summary>
public interface ITransactionRepository : IRepository<Transaction, Guid>
{
    Task<IEnumerable<Transaction>> GetByAccountIdAsync(Guid accountId, DateTime? from = null, DateTime? to = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<Transaction>> GetBySourceAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Transaction>> GetByTargetAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default);
}
