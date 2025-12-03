namespace BankApi.Domain.Common;

/// <summary>
/// Base class for aggregate roots in the domain
/// </summary>
/// <typeparam name="TId">The type of the aggregate root identifier</typeparam>
public abstract class AggregateRoot<TId> : Entity<TId> where TId : notnull
{
    public DateTime CreatedAt { get; protected set; }
    public DateTime UpdatedAt { get; protected set; }
    public bool IsDeleted { get; protected set; }

    protected AggregateRoot()
    {
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        IsDeleted = false;
    }

    /// <summary>
    /// Marks the aggregate as deleted (soft delete)
    /// </summary>
    public virtual void MarkAsDeleted()
    {
        IsDeleted = true;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates the timestamp when the aggregate is modified
    /// </summary>
    protected void UpdateTimestamp()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}
