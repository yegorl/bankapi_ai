using BankApi.Domain.Aggregates.LoginSessions;

namespace BankApi.Domain.Repositories;

/// <summary>
/// Repository interface for LoginSession aggregate
/// </summary>
public interface ILoginSessionRepository : IRepository<LoginSession, Guid>
{
    Task<IEnumerable<LoginSession>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<LoginSession>> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
}
