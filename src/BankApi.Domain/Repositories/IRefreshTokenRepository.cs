using BankApi.Domain.Aggregates.RefreshTokens;

namespace BankApi.Domain.Repositories;

/// <summary>
/// Repository interface for RefreshToken aggregate
/// </summary>
public interface IRefreshTokenRepository : IRepository<RefreshToken, Guid>
{
    Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);
    Task<IEnumerable<RefreshToken>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task RevokeAllByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}
