using BankApi.Domain.Aggregates.MoneyTransfers;
using BankApi.Domain.Repositories;
using BankApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BankApi.Infrastructure.Repositories;

public class MoneyTransferRepository : Repository<MoneyTransfer, Guid>, IMoneyTransferRepository
{
    public MoneyTransferRepository(BankDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<MoneyTransfer>> GetByCardNumberAsync(string cardNumber, CancellationToken cancellationToken = default)
    {
        return await Context.Set<MoneyTransfer>()
            .Where(mt => mt.SourceCardNumber == cardNumber || mt.TargetCardNumber == cardNumber)
            .OrderByDescending(mt => mt.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
