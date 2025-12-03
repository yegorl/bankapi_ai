using BankApi.Domain.Aggregates.AccountHolders;

namespace BankApi.Domain.Repositories;

/// <summary>
/// Repository interface for AccountHolder aggregate
/// </summary>
public interface IAccountHolderRepository : IRepository<AccountHolder, string>
{
    Task<AccountHolder?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<IEnumerable<AccountHolder>> GetActiveAsync(CancellationToken cancellationToken = default);
}
