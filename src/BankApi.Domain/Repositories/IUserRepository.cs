using BankApi.Domain.Aggregates.Users;

namespace BankApi.Domain.Repositories;

/// <summary>
/// Repository interface for User aggregate
/// </summary>
public interface IUserRepository : IRepository<User, Guid>
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
}
