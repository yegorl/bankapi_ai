using BankApi.Domain.Aggregates.Users;
using BankApi.Domain.Repositories;
using BankApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BankApi.Infrastructure.Repositories;

public class UserRepository : Repository<User, Guid>, IUserRepository
{
    public UserRepository(BankDbContext context) : base(context)
    {
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await Context.Users
            .FirstOrDefaultAsync(u => u.Email.Value == email, cancellationToken);
    }
}
