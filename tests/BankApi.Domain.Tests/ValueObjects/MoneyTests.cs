using BankApi.Domain.Exceptions;
using BankApi.Domain.ValueObjects;
using FluentAssertions;

namespace BankApi.Domain.Tests.ValueObjects;

public class MoneyTests
{
    [Fact(DisplayName = "Should create money with valid amount and currency")]
    public void Create_WithValidData_ReturnsMoney()
    {
        // Arrange & Act
        var money = Money.Create(100.50m, "USD");

        // Assert
        money.Should().NotBeNull();
        money.Amount.Should().Be(100.50m);
        money.Currency.Should().Be("USD");
    }

    [Fact(DisplayName = "Should add money with same currency")]
    public void Add_WithSameCurrency_ReturnsSum()
    {
        // Arrange
        var money1 = Money.Create(100m, "USD");
        var money2 = Money.Create(50m, "USD");

        // Act
        var result = money1.Add(money2);

        // Assert
        result.Amount.Should().Be(150m);
        result.Currency.Should().Be("USD");
    }

    [Fact(DisplayName = "Should throw exception when adding different currencies")]
    public void Add_WithDifferentCurrencies_ThrowsInvalidOperationException()
    {
        // Arrange
        var money1 = Money.Create(100m, "USD");
        var money2 = Money.Create(50m, "EUR");

        // Act & Assert
        var act = () => money1.Add(money2);
        act.Should().Throw<InvalidOperationDomainException>()
            .WithMessage("*currency*");
    }

    [Fact(DisplayName = "Should subtract money with same currency")]
    public void Subtract_WithSameCurrency_ReturnsDifference()
    {
        // Arrange
        var money1 = Money.Create(100m, "USD");
        var money2 = Money.Create(30m, "USD");

        // Act
        var result = money1.Subtract(money2);

        // Assert
        result.Amount.Should().Be(70m);
    }

    [Fact(DisplayName = "Should correctly identify positive amount")]
    public void IsPositive_WithPositiveAmount_ReturnsTrue()
    {
        // Arrange
        var money = Money.Create(100m, "USD");

        // Act & Assert
        money.IsPositive().Should().BeTrue();
    }

    [Fact(DisplayName = "Should correctly identify negative amount")]
    public void IsNegative_WithNegativeAmount_ReturnsTrue()
    {
        // Arrange
        var money = Money.Create(-100m, "USD");

        // Act & Assert
        money.IsNegative().Should().BeTrue();
    }
}
