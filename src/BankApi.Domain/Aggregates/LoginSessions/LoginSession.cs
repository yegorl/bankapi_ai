using BankApi.Domain.Common;

namespace BankApi.Domain.Aggregates.LoginSessions;

/// <summary>
/// Represents a login session/attempt aggregate
/// </summary>
public class LoginSession : AggregateRoot<Guid>
{
    public Guid UserId { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string IpAddress { get; private set; } = string.Empty;
    public string UserAgentSnapshot { get; private set; } = string.Empty;
    public bool Success { get; private set; }
    public string? FailureReason { get; private set; }
    public DateTime AttemptedAt { get; private set; }
    public DateTime? ExpiresAt { get; private set; }

    // Required by EF Core
    private LoginSession() { }

    private LoginSession(Guid id, Guid userId, string email, string ipAddress, string userAgentSnapshot, bool success, string? failureReason, DateTime? expiresAt)
    {
        Id = id;
        UserId = userId;
        Email = email;
        IpAddress = ipAddress;
        UserAgentSnapshot = userAgentSnapshot;
        Success = success;
        FailureReason = failureReason;
        AttemptedAt = DateTime.UtcNow;
        ExpiresAt = expiresAt;
    }

    /// <summary>
    /// Creates a successful login session
    /// </summary>
    public static LoginSession CreateSuccessful(Guid userId, string email, string ipAddress, string userAgentSnapshot, DateTime expiresAt)
    {
        return new LoginSession(Guid.NewGuid(), userId, email, ipAddress, userAgentSnapshot, true, null, expiresAt);
    }

    /// <summary>
    /// Creates a failed login attempt
    /// </summary>
    public static LoginSession CreateFailed(string email, string ipAddress, string userAgentSnapshot, string failureReason)
    {
        return new LoginSession(Guid.NewGuid(), Guid.Empty, email, ipAddress, userAgentSnapshot, false, failureReason, null);
    }
}
