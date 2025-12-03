using BankApi.Domain.Aggregates.AccountHolders;
using BankApi.Domain.Exceptions;
using BankApi.Domain.ValueObjects;
using FluentAssertions;

namespace BankApi.Domain.Tests.Aggregates;

public class AccountHolderTests
{
    [Fact(DisplayName = "Should create account holder with valid data")]
    public void Create_WithValidData_ReturnsAccountHolder()
    {
        // Arrange
        var email = EmailAddress.Create("test@example.com");
        var phone = PhoneNumber.Create("+1234567890");
        var dateOfBirth = DateTime.UtcNow.AddYears(-30);

        // Act
        var userId = Guid.NewGuid();
        var holder = AccountHolder.Create(userId, "John", "Doe", email, phone, dateOfBirth);

        // Assert
        holder.Should().NotBeNull();
        holder.Id.Should().StartWith("AH-");
        holder.FirstName.Should().Be("John");
        holder.LastName.Should().Be("Doe");
        holder.Email.Should().Be(email);
        holder.Phone.Should().Be(phone);
        holder.DateOfBirth.Should().Be(dateOfBirth);
        holder.IsDeleted.Should().BeFalse();
    }

    [Fact(DisplayName = "Should throw exception when creating with empty first name")]
    public void Create_WithEmptyFirstName_ThrowsValidationException()
    {
        // Arrange
        var email = EmailAddress.Create("test@example.com");
        var phone = PhoneNumber.Create("+1234567890");
        var dateOfBirth = DateTime.UtcNow.AddYears(-30);

        // Act & Assert
        var userId = Guid.NewGuid();
        var act = () => AccountHolder.Create(userId, "", "Doe", email, phone, dateOfBirth);
        act.Should().Throw<ValidationException>()
            .WithMessage("*first name*");
    }

    [Fact(DisplayName = "Should throw exception when creating with age less than 18")]
    public void Create_WithAgeLessThan18_ThrowsValidationException()
    {
        // Arrange
        var email = EmailAddress.Create("test@example.com");
        var phone = PhoneNumber.Create("+1234567890");
        var dateOfBirth = DateTime.UtcNow.AddYears(-17);

        // Act & Assert
        var userId = Guid.NewGuid();
        var act = () => AccountHolder.Create(userId, "John", "Doe", email, phone, dateOfBirth);
        act.Should().Throw<ValidationException>()
            .WithMessage("*18 years old*");
    }

    [Fact(DisplayName = "Should update contact information successfully")]
    public void UpdateContactInfo_WithValidData_UpdatesContact()
    {
        // Arrange
        var email = EmailAddress.Create("test@example.com");
        var phone = PhoneNumber.Create("+1234567890");
        var userId = Guid.NewGuid();
        var holder = AccountHolder.Create(userId, "John", "Doe", email, phone, DateTime.UtcNow.AddYears(-30));
        
        var newEmail = EmailAddress.Create("newemail@example.com");
        var newPhone = PhoneNumber.Create("+9876543210");

        // Act
        holder.UpdateContactInfo(newEmail, newPhone, null);

        // Assert
        holder.Email.Should().Be(newEmail);
        holder.Phone.Should().Be(newPhone);
    }

    [Fact(DisplayName = "Should mark account holder as deleted")]
    public void MarkAsDeleted_WhenCalled_SetsIsDeletedTrue()
    {
        // Arrange
        var email = EmailAddress.Create("test@example.com");
        var phone = PhoneNumber.Create("+1234567890");
        var userId = Guid.NewGuid();
        var holder = AccountHolder.Create(userId, "John", "Doe", email, phone, DateTime.UtcNow.AddYears(-30));

        // Act
        holder.MarkAsDeleted();

        // Assert
        holder.IsDeleted.Should().BeTrue();
    }
}
