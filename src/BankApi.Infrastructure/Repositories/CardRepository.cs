using BankApi.Domain.Aggregates.Cards;
using BankApi.Domain.Repositories;
using BankApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BankApi.Infrastructure.Repositories;

public class CardRepository : Repository<Card, Guid>, ICardRepository
{
    public CardRepository(BankDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Card>> GetByAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default)
    {
        return await Context.Cards
            .Where(c => c.AccountId == accountId && !c.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<Card?> GetByCardNumberAsync(string cardNumber, CancellationToken cancellationToken = default)
    {
        return await Context.Cards
            .FirstOrDefaultAsync(c => c.CardNumber.Value == cardNumber, cancellationToken);
    }
}
