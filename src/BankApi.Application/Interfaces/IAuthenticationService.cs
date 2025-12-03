namespace BankApi.Domain.Interfaces;

/// <summary>
/// Authentication service interface
/// </summary>
public interface IAuthenticationService
{
    Task<AuthenticationResult> SignUpAsync(string email, string password, string firstName, string lastName, string phone, CancellationToken cancellationToken = default);
    Task<AuthenticationResult> LoginAsync(string email, string password, string ipAddress, string userAgent, CancellationToken cancellationToken = default);
    Task<bool> ValidateTokenAsync(string token, string ipAddress, string userAgent, CancellationToken cancellationToken = default);
}

/// <summary>
/// Result of an authentication operation
/// </summary>
public record AuthenticationResult(bool Success, string? Token, string? Error, string? UserId);
