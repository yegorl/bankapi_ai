using BankApi.Domain.Aggregates.Cards;
using System.Linq.Expressions;

namespace BankApi.Domain.Specifications;

/// <summary>
/// Specification to check if a card is valid for use
/// </summary>
public class CardIsValidSpecification : ISpecification<Card>
{
    public Expression<Func<Card, bool>> ToExpression()
    {
        var now = DateTime.UtcNow;
        return card => !card.IsDeleted && !card.IsBlocked && !card.IsTemporarilyBlocked && card.ExpirationDate > now;
    }

    public bool IsSatisfiedBy(Card entity)
    {
        return entity.IsUsable();
    }
}
