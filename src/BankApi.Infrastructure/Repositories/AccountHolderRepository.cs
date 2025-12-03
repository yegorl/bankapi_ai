using BankApi.Domain.Aggregates.AccountHolders;
using BankApi.Domain.Repositories;
using BankApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BankApi.Infrastructure.Repositories;

public class AccountHolderRepository : Repository<AccountHolder, string>, IAccountHolderRepository
{
    public AccountHolderRepository(BankDbContext context) : base(context)
    {
    }

    public async Task<AccountHolder?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await Context.AccountHolders
            .FirstOrDefaultAsync(ah => ah.Email.Value == email, cancellationToken);
    }

    public async Task<IEnumerable<AccountHolder>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        return await Context.AccountHolders
            .Where(ah => !ah.IsDeleted)
            .ToListAsync(cancellationToken);
    }
}
