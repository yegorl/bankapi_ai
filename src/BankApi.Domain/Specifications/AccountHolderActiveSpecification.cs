using BankApi.Domain.Aggregates.AccountHolders;
using System.Linq.Expressions;

namespace BankApi.Domain.Specifications;

/// <summary>
/// Specification to check if an account holder is active (not deleted)
/// </summary>
public class AccountHolderActiveSpecification : ISpecification<AccountHolder>
{
    public Expression<Func<AccountHolder, bool>> ToExpression()
    {
        return holder => !holder.IsDeleted;
    }

    public bool IsSatisfiedBy(AccountHolder entity)
    {
        return !entity.IsDeleted;
    }
}
