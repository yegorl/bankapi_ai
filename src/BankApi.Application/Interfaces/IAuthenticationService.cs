namespace BankApi.Domain.Interfaces;

/// <summary>
/// Authentication service interface
/// </summary>
public interface IAuthenticationService
{
    Task<AuthenticationResult> SignUpAsync(string email, string password, CancellationToken cancellationToken = default);
    Task<AuthenticationResult> LoginAsync(string email, string password, string ipAddress, string userAgentSnapshot, CancellationToken cancellationToken = default);
    Task<AuthenticationResult> RefreshTokenAsync(string refreshToken, string ipAddress, string userAgentSnapshot, CancellationToken cancellationToken = default);
    Task<bool> ValidateTokenAsync(string token, string ipAddress, string userAgentSnapshot, CancellationToken cancellationToken = default);
    Task RevokeRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
}

/// <summary>
/// Result of an authentication operation
/// </summary>
public record AuthenticationResult(bool Success, string? Token, string? RefreshToken, string? Error, string? UserId);
