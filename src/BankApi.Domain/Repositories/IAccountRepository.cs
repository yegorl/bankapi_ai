using BankApi.Domain.Aggregates.Accounts;

namespace BankApi.Domain.Repositories;

/// <summary>
/// Repository interface for Account aggregate
/// </summary>
public interface IAccountRepository : IRepository<Account, Guid>
{
    Task<Account?> GetByAccountNumberAsync(string accountNumber, CancellationToken cancellationToken = default);
    Task<IEnumerable<Account>> GetByHolderIdAsync(string accountHolderId, CancellationToken cancellationToken = default);
}
