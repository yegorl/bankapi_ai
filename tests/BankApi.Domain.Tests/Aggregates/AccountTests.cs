using BankApi.Domain.Aggregates.Accounts;
using BankApi.Domain.Exceptions;
using BankApi.Domain.ValueObjects;
using FluentAssertions;

namespace BankApi.Domain.Tests.Aggregates;

public class AccountTests
{
    [Fact(DisplayName = "Should create account with valid data")]
    public void Create_WithValidData_ReturnsAccount()
    {
        // Arrange
        var accountHolderId = "AH-00001";
        var currency = "USD";

        // Act
        var account = Account.Create(accountHolderId, currency);

        // Assert
        account.Should().NotBeNull();
        account.AccountHolderId.Should().Be(accountHolderId);
        account.Balance.Amount.Should().Be(0);
        account.Balance.Currency.Should().Be(currency);
        account.IsDeleted.Should().BeFalse();
    }

    [Fact(DisplayName = "Should credit account successfully")]
    public void Credit_WithValidAmount_IncreasesBalance()
    {
        // Arrange
        var account = Account.Create("AH-00001", "USD");
        var amount = Money.Create(100m, "USD");

        // Act
        account.Credit(amount);

        // Assert
        account.Balance.Amount.Should().Be(100m);
    }

    [Fact(DisplayName = "Should debit account successfully with sufficient balance")]
    public void Debit_WithSufficientBalance_DecreasesBalance()
    {
        // Arrange
        var account = Account.Create("AH-00001", "USD");
        account.Credit(Money.Create(100m, "USD"));
        var debitAmount = Money.Create(50m, "USD");

        // Act
        account.Debit(debitAmount);

        // Assert
        account.Balance.Amount.Should().Be(50m);
    }

    [Fact(DisplayName = "Should throw exception when debiting with insufficient balance")]
    public void Debit_WithInsufficientBalance_ThrowsInsufficientBalanceException()
    {
        // Arrange
        var account = Account.Create("AH-00001", "USD");
        account.Credit(Money.Create(50m, "USD"));
        var debitAmount = Money.Create(100m, "USD");

        // Act & Assert
        var act = () => account.Debit(debitAmount);
        act.Should().Throw<InsufficientBalanceException>();
    }

    [Fact(DisplayName = "Should throw exception when crediting with different currency")]
    public void Credit_WithDifferentCurrency_ThrowsInvalidOperationException()
    {
        // Arrange
        var account = Account.Create("AH-00001", "USD");
        var amount = Money.Create(100m, "EUR");

        // Act & Assert
        var act = () => account.Credit(amount);
        act.Should().Throw<InvalidOperationDomainException>()
            .WithMessage("*currency*");
    }

    [Fact(DisplayName = "Should update description successfully")]
    public void UpdateDescription_WithValidDescription_UpdatesDescription()
    {
        // Arrange
        var account = Account.Create("AH-00001", "USD");
        var newDescription = "Personal Savings Account";

        // Act
        account.UpdateDescription(newDescription);

        // Assert
        account.Description.Should().Be(newDescription);
    }
}
