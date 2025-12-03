using BankApi.Domain.Common;
using BankApi.Domain.Exceptions;

namespace BankApi.Domain.Aggregates.RefreshTokens;

/// <summary>
/// Represents a refresh token aggregate
/// </summary>
public class RefreshToken : AggregateRoot<Guid>
{
    public Guid UserId { get; private set; }
    public string Token { get; private set; } = string.Empty;
    public string IpAddress { get; private set; } = string.Empty;
    public string UserAgentSnapshot { get; private set; } = string.Empty;
    public DateTime ExpiresAt { get; private set; }
    public bool IsRevoked { get; private set; }
    public DateTime? RevokedAt { get; private set; }
    public bool IsUsed { get; private set; }
    public DateTime? UsedAt { get; private set; }

    // Required by EF Core
    private RefreshToken() { }

    private RefreshToken(Guid id, Guid userId, string token, string ipAddress, string userAgentSnapshot, DateTime expiresAt)
    {
        Id = id;
        UserId = userId;
        Token = token;
        IpAddress = ipAddress;
        UserAgentSnapshot = userAgentSnapshot;
        ExpiresAt = expiresAt;
        IsRevoked = false;
        IsUsed = false;
    }

    /// <summary>
    /// Creates a new refresh token
    /// </summary>
    public static RefreshToken Create(Guid userId, string ipAddress, string userAgentSnapshot, int expirationDays = 30)
    {
        var token = GenerateToken();
        var expiresAt = DateTime.UtcNow.AddDays(expirationDays);
        return new RefreshToken(Guid.NewGuid(), userId, token, ipAddress, userAgentSnapshot, expiresAt);
    }

    /// <summary>
    /// Marks the refresh token as used
    /// </summary>
    public void MarkAsUsed()
    {
        if (IsRevoked)
            throw new InvalidOperationDomainException("Cannot use a revoked refresh token");

        if (IsUsed)
            throw new InvalidOperationDomainException("Refresh token has already been used");

        if (DateTime.UtcNow > ExpiresAt)
            throw new InvalidOperationDomainException("Refresh token has expired");

        IsUsed = true;
        UsedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Revokes the refresh token
    /// </summary>
    public void Revoke()
    {
        if (IsRevoked)
            throw new InvalidOperationDomainException("Refresh token is already revoked");

        IsRevoked = true;
        RevokedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Checks if the refresh token is valid
    /// </summary>
    public bool IsValid()
    {
        return !IsRevoked && !IsUsed && DateTime.UtcNow <= ExpiresAt;
    }

    private static string GenerateToken()
    {
        return Convert.ToBase64String(Guid.NewGuid().ToByteArray()) + Convert.ToBase64String(Guid.NewGuid().ToByteArray());
    }
}
