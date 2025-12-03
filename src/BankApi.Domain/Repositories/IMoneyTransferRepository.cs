using BankApi.Domain.Aggregates.MoneyTransfers;

namespace BankApi.Domain.Repositories;

/// <summary>
/// Repository interface for MoneyTransfer aggregate
/// </summary>
public interface IMoneyTransferRepository : IRepository<MoneyTransfer, Guid>
{
    Task<IEnumerable<MoneyTransfer>> GetByCardNumberAsync(string cardNumber, CancellationToken cancellationToken = default);
}
