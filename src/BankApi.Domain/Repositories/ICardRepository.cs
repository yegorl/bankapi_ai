using BankApi.Domain.Aggregates.Cards;

namespace BankApi.Domain.Repositories;

/// <summary>
/// Repository interface for Card aggregate
/// </summary>
public interface ICardRepository : IRepository<Card, Guid>
{
    Task<IEnumerable<Card>> GetByAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default);
    Task<Card?> GetByCardNumberAsync(string cardNumber, CancellationToken cancellationToken = default);
}
