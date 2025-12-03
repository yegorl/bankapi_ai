using BankApi.Domain.Aggregates.Transactions;
using BankApi.Domain.Repositories;
using BankApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BankApi.Infrastructure.Repositories;

public class TransactionRepository : Repository<Transaction, Guid>, ITransactionRepository
{
    public TransactionRepository(BankDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Transaction>> GetByAccountIdAsync(Guid accountId, DateTime? from = null, DateTime? to = null, CancellationToken cancellationToken = default)
    {
        var query = Context.Transactions
            .Where(t => (t.SourceAccountId == accountId || t.TargetAccountId == accountId) && !t.IsDeleted);

        if (from.HasValue)
            query = query.Where(t => t.CreatedAt >= from.Value);

        if (to.HasValue)
            query = query.Where(t => t.CreatedAt <= to.Value);

        return await query.OrderByDescending(t => t.CreatedAt).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Transaction>> GetBySourceAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default)
    {
        return await Context.Transactions
            .Where(t => t.SourceAccountId == accountId && !t.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Transaction>> GetByTargetAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default)
    {
        return await Context.Transactions
            .Where(t => t.TargetAccountId == accountId && !t.IsDeleted)
            .ToListAsync(cancellationToken);
    }
}
