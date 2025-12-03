using BankApi.Domain.Aggregates.Accounts;
using BankApi.Domain.Repositories;
using BankApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BankApi.Infrastructure.Repositories;

public class AccountRepository : Repository<Account, Guid>, IAccountRepository
{
    public AccountRepository(BankDbContext context) : base(context)
    {
    }

    public async Task<Account?> GetByAccountNumberAsync(string accountNumber, CancellationToken cancellationToken = default)
    {
        return await Context.Accounts
            .FirstOrDefaultAsync(a => a.AccountNumber.Value == accountNumber, cancellationToken);
    }

    public async Task<IEnumerable<Account>> GetByHolderIdAsync(string accountHolderId, CancellationToken cancellationToken = default)
    {
        return await Context.Accounts
            .Where(a => a.AccountHolderId == accountHolderId && !a.IsDeleted)
            .ToListAsync(cancellationToken);
    }
}
