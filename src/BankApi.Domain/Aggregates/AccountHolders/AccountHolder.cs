using BankApi.Domain.Common;
using BankApi.Domain.Events;
using BankApi.Domain.Exceptions;
using BankApi.Domain.ValueObjects;

namespace BankApi.Domain.Aggregates.AccountHolders;

/// <summary>
/// Represents an account holder aggregate root
/// </summary>
public class AccountHolder : AggregateRoot<string>
{
    private static int _lastId = 0;
    private static readonly object _lockObject = new();

    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public EmailAddress Email { get; private set; } = null!;
    public PhoneNumber Phone { get; private set; } = null!;
    public DateTime DateOfBirth { get; private set; }
    public Address? Address { get; private set; }

    // Required by EF Core
    private AccountHolder() { }

    private AccountHolder(string id, string firstName, string lastName, EmailAddress email, PhoneNumber phone, DateTime dateOfBirth, Address? address)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Phone = phone;
        DateOfBirth = dateOfBirth;
        Address = address;
    }

    /// <summary>
    /// Creates a new AccountHolder
    /// </summary>
    public static AccountHolder Create(string firstName, string lastName, EmailAddress email, PhoneNumber phone, DateTime dateOfBirth, Address? address = null)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ValidationException("First name cannot be empty");

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ValidationException("Last name cannot be empty");

        if (dateOfBirth > DateTime.UtcNow)
            throw new ValidationException("Date of birth cannot be in the future");

        var age = DateTime.UtcNow.Year - dateOfBirth.Year;
        if (age < 18)
            throw new ValidationException("Account holder must be at least 18 years old");

        var id = GenerateAccountHolderId();
        var accountHolder = new AccountHolder(id, firstName, lastName, email, phone, dateOfBirth, address);

        accountHolder.AddDomainEvent(new AccountHolderCreatedEvent(
            id,
            email.Value,
            $"{firstName} {lastName}",
            DateTime.UtcNow));

        return accountHolder;
    }

    /// <summary>
    /// Updates contact information
    /// </summary>
    public void UpdateContactInfo(EmailAddress? email, PhoneNumber? phone, Address? address)
    {
        if (IsDeleted)
            throw new InvalidOperationDomainException("Cannot update a deleted account holder");

        if (email is not null)
            Email = email;

        if (phone is not null)
            Phone = phone;

        if (address is not null)
            Address = address;

        UpdateTimestamp();
    }

    /// <summary>
    /// Marks the account holder as deleted
    /// </summary>
    public override void MarkAsDeleted()
    {
        base.MarkAsDeleted();
        
        AddDomainEvent(new AccountHolderDeletedEvent(Id, Email.Value, DateTime.UtcNow));
    }

    /// <summary>
    /// Generates a new unique account holder ID in the format AH-XXXXX
    /// </summary>
    private static string GenerateAccountHolderId()
    {
        lock (_lockObject)
        {
            _lastId++;
            return $"AH-{_lastId:D5}";
        }
    }
}
