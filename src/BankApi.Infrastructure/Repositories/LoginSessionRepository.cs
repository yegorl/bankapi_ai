using BankApi.Domain.Aggregates.LoginSessions;
using BankApi.Domain.Repositories;
using BankApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BankApi.Infrastructure.Repositories;

public class LoginSessionRepository : Repository<LoginSession, Guid>, ILoginSessionRepository
{
    public LoginSessionRepository(BankDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<LoginSession>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await Context.LoginSessions
            .Where(ls => ls.UserId == userId)
            .OrderByDescending(ls => ls.AttemptedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<LoginSession>> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await Context.LoginSessions
            .Where(ls => ls.Email == email)
            .OrderByDescending(ls => ls.AttemptedAt)
            .ToListAsync(cancellationToken);
    }
}
