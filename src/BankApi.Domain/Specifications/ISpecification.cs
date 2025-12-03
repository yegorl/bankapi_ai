using System.Linq.Expressions;

namespace BankApi.Domain.Specifications;

/// <summary>
/// Specification pattern interface
/// </summary>
/// <typeparam name="T">The entity type</typeparam>
public interface ISpecification<T>
{
    Expression<Func<T, bool>> ToExpression();
    bool IsSatisfiedBy(T entity);
}
