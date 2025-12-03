using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BankApi.Domain.Aggregates.LoginSessions;
using BankApi.Domain.Aggregates.RefreshTokens;
using BankApi.Domain.Interfaces;
using BankApi.Domain.Repositories;
using BankApi.Domain.ValueObjects;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace BankApi.Infrastructure.Authentication;

/// <summary>
/// JWT token service for authentication with IP and User-Agent binding
/// </summary>
public class JwtTokenService : IAuthenticationService
{
    private readonly JwtSettings _jwtSettings;
    private readonly IUserRepository _userRepository;
    private readonly ILoginSessionRepository _loginSessionRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUnitOfWork _unitOfWork;
    
    // NOTE: In-memory storage for demonstration purposes only
    // PRODUCTION: Replace with proper database tables or distributed cache (Redis)
    // Security concern: Data lost on restart, not suitable for horizontal scaling
    private static readonly Dictionary<string, string> _passwords = new();
    private static readonly Dictionary<string, (string IpHash, string UaHash)> _tokenBindings = new();

    public JwtTokenService(
        IOptions<JwtSettings> jwtSettings,
        IUserRepository userRepository,
        ILoginSessionRepository loginSessionRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IUnitOfWork unitOfWork)
    {
        _jwtSettings = jwtSettings.Value;
        _userRepository = userRepository;
        _loginSessionRepository = loginSessionRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<AuthenticationResult> SignUpAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var existingUser = await _userRepository.GetByEmailAsync(email, cancellationToken);
            if (existingUser is not null)
                return new AuthenticationResult(false, null, null, "Email already registered", null);

            var emailValue = EmailAddress.Create(email);
            
            // Store password hash (use BCrypt in production)
            var passwordHash = HashPassword(password);
            _passwords[email] = passwordHash;

            return new AuthenticationResult(true, null, null, null, "user-created");
        }
        catch (Exception ex)
        {
            return new AuthenticationResult(false, null, null, ex.Message, null);
        }
    }

    public async Task<AuthenticationResult> LoginAsync(
        string email,
        string password,
        string ipAddress,
        string userAgentSnapshot,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userRepository.GetByEmailAsync(email, cancellationToken);
            
            // Log failed attempt if user not found
            // NOTE: Using generic internal reason to prevent username enumeration
            if (user is null)
            {
                var failedSession = LoginSession.CreateFailed(email, ipAddress, userAgentSnapshot, "Authentication failed");
                await _loginSessionRepository.AddAsync(failedSession, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return new AuthenticationResult(false, null, null, "Invalid credentials", null);
            }

            // Verify password
            if (!_passwords.ContainsKey(email) || _passwords[email] != HashPassword(password))
            {
                var failedSession = LoginSession.CreateFailed(email, ipAddress, userAgentSnapshot, "Authentication failed");
                await _loginSessionRepository.AddAsync(failedSession, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return new AuthenticationResult(false, null, null, "Invalid credentials", null);
            }

            var ipHash = HashString(ipAddress);
            var uaHash = HashString(userAgentSnapshot);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim("ip_hash", ipHash),
                new Claim("ua_hash", uaHash)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes);

            var token = new JwtSecurityToken(
                _jwtSettings.Issuer,
                _jwtSettings.Audience,
                claims,
                expires: expires,
                signingCredentials: creds);

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            
            // Store binding
            _tokenBindings[tokenString] = (ipHash, uaHash);

            // Create refresh token
            var refreshToken = RefreshToken.Create(user.Id, ipAddress, userAgentSnapshot);
            await _refreshTokenRepository.AddAsync(refreshToken, cancellationToken);

            // Log successful login
            var successSession = LoginSession.CreateSuccessful(user.Id, email, ipAddress, userAgentSnapshot, expires);
            await _loginSessionRepository.AddAsync(successSession, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new AuthenticationResult(true, tokenString, refreshToken.Token, null, user.Id.ToString());
        }
        catch (Exception ex)
        {
            return new AuthenticationResult(false, null, null, ex.Message, null);
        }
    }

    public async Task<AuthenticationResult> RefreshTokenAsync(
        string refreshToken,
        string ipAddress,
        string userAgentSnapshot,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var token = await _refreshTokenRepository.GetByTokenAsync(refreshToken, cancellationToken);
            
            if (token is null || !token.IsValid())
                return new AuthenticationResult(false, null, null, "Invalid or expired refresh token", null);

            // Mark the token as used
            token.MarkAsUsed();

            // Get the user
            var user = await _userRepository.GetByIdAsync(token.UserId, cancellationToken);
            if (user is null)
                return new AuthenticationResult(false, null, null, "User not found", null);

            // Generate new access token
            var ipHash = HashString(ipAddress);
            var uaHash = HashString(userAgentSnapshot);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email.Value),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim("ip_hash", ipHash),
                new Claim("ua_hash", uaHash)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes);

            var jwtToken = new JwtSecurityToken(
                _jwtSettings.Issuer,
                _jwtSettings.Audience,
                claims,
                expires: expires,
                signingCredentials: creds);

            var tokenString = new JwtSecurityTokenHandler().WriteToken(jwtToken);
            _tokenBindings[tokenString] = (ipHash, uaHash);

            // Generate new refresh token
            var newRefreshToken = RefreshToken.Create(user.Id, ipAddress, userAgentSnapshot);
            await _refreshTokenRepository.AddAsync(newRefreshToken, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new AuthenticationResult(true, tokenString, newRefreshToken.Token, null, user.Id.ToString());
        }
        catch (Exception ex)
        {
            return new AuthenticationResult(false, null, null, ex.Message, null);
        }
    }

    public async Task RevokeRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var token = await _refreshTokenRepository.GetByTokenAsync(refreshToken, cancellationToken);
        if (token is not null && !token.IsRevoked)
        {
            token.Revoke();
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }

    public Task<bool> ValidateTokenAsync(
        string token,
        string ipAddress,
        string userAgentSnapshot,
        CancellationToken cancellationToken = default)
    {
        if (!_tokenBindings.ContainsKey(token))
            return Task.FromResult(false);

        var (storedIpHash, storedUaHash) = _tokenBindings[token];
        var currentIpHash = HashString(ipAddress);
        var currentUaHash = HashString(userAgentSnapshot);

        return Task.FromResult(storedIpHash == currentIpHash && storedUaHash == currentUaHash);
    }

    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }

    private static string HashString(string value)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(value);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}
