using BankApi.Domain.Aggregates.Accounts;
using BankApi.Domain.ValueObjects;
using System.Linq.Expressions;

namespace BankApi.Domain.Specifications;

/// <summary>
/// Specification to check if an account has sufficient balance
/// </summary>
public class AccountHasSufficientBalanceSpecification : ISpecification<Account>
{
    private readonly Money _requiredAmount;

    public AccountHasSufficientBalanceSpecification(Money requiredAmount)
    {
        _requiredAmount = requiredAmount;
    }

    public Expression<Func<Account, bool>> ToExpression()
    {
        // Cannot be easily expressed as an expression for LINQ to Entities
        throw new NotImplementedException("Use IsSatisfiedBy for in-memory evaluation");
    }

    public bool IsSatisfiedBy(Account entity)
    {
        return entity.Balance.IsGreaterThanOrEqual(_requiredAmount);
    }
}
