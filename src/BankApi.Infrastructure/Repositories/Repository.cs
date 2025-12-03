using BankApi.Domain.Common;
using BankApi.Domain.Repositories;
using BankApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BankApi.Infrastructure.Repositories;

public abstract class Repository<TEntity, TId> : IRepository<TEntity, TId>
    where TEntity : AggregateRoot<TId>
    where TId : notnull
{
    protected readonly BankDbContext Context;

    protected Repository(BankDbContext context)
    {
        Context = context;
    }

    public virtual async Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default)
    {
        return await Context.Set<TEntity>().FindAsync([id], cancellationToken);
    }

    public virtual async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await Context.Set<TEntity>().ToListAsync(cancellationToken);
    }

    public virtual async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await Context.Set<TEntity>().AddAsync(entity, cancellationToken);
    }

    public virtual Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        Context.Set<TEntity>().Update(entity);
        return Task.CompletedTask;
    }

    public virtual Task DeleteAsync(TId id, CancellationToken cancellationToken = default)
    {
        var entity = Context.Set<TEntity>().Find(id);
        if (entity is not null)
        {
            Context.Set<TEntity>().Remove(entity);
        }
        return Task.CompletedTask;
    }
}
