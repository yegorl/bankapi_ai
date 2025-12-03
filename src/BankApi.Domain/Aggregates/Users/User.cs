using BankApi.Domain.Common;
using BankApi.Domain.Events;
using BankApi.Domain.Exceptions;
using BankApi.Domain.ValueObjects;

namespace BankApi.Domain.Aggregates.Users;

/// <summary>
/// Represents a user aggregate root with authentication and authorization
/// </summary>
public class User : AggregateRoot<Guid>
{
    public EmailAddress Email { get; private set; } = null!;
    public string PasswordHash { get; private set; } = string.Empty;
    public UserRole Role { get; private set; }

    // Required by EF Core
    private User() { }

    private User(Guid id, EmailAddress email, string passwordHash, UserRole role)
    {
        Id = id;
        Email = email;
        PasswordHash = passwordHash;
        Role = role;
    }

    /// <summary>
    /// Creates a new User
    /// </summary>
    public static User Create(EmailAddress email, string passwordHash, UserRole role)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ValidationException("Password hash cannot be empty");

        var id = Guid.NewGuid();
        var user = new User(id, email, passwordHash, role);

        user.AddDomainEvent(new UserCreatedEvent(id, email.Value, role.ToString(), DateTime.UtcNow));

        return user;
    }

    /// <summary>
    /// Updates the password hash
    /// </summary>
    public void UpdatePassword(string passwordHash)
    {
        if (IsDeleted)
            throw new InvalidOperationDomainException("Cannot update password for deleted user");

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ValidationException("Password hash cannot be empty");

        PasswordHash = passwordHash;
        UpdateTimestamp();
    }

    /// <summary>
    /// Updates the user role
    /// </summary>
    public void UpdateRole(UserRole role)
    {
        if (IsDeleted)
            throw new InvalidOperationDomainException("Cannot update role for deleted user");

        Role = role;
        UpdateTimestamp();
    }

    /// <summary>
    /// Checks if user has admin role
    /// </summary>
    public bool IsAdmin() => Role == UserRole.Admin;

    /// <summary>
    /// Checks if user has regular user role
    /// </summary>
    public bool IsRegularUser() => Role == UserRole.User;
}

/// <summary>
/// Represents user roles in the system
/// </summary>
public enum UserRole
{
    User,
    Admin
}
