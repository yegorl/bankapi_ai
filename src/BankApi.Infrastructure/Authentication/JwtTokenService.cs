using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
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
    private readonly IUnitOfWork _unitOfWork;
    
    // NOTE: In-memory storage for demonstration purposes only
    // PRODUCTION: Replace with proper database tables or distributed cache (Redis)
    // Security concern: Data lost on restart, not suitable for horizontal scaling
    private static readonly Dictionary<string, string> _passwords = new();
    private static readonly Dictionary<string, (string IpHash, string UaHash)> _tokenBindings = new();

    public JwtTokenService(
        IOptions<JwtSettings> jwtSettings,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _jwtSettings = jwtSettings.Value;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<AuthenticationResult> SignUpAsync(
        string email,
        string password,
        string firstName,
        string lastName,
        string phone,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var existingUser = await _userRepository.GetByEmailAsync(email, cancellationToken);
            if (existingUser is not null)
                return new AuthenticationResult(false, null, "Email already registered", null);

            var emailValue = EmailAddress.Create(email);
            
            // Store password hash (use BCrypt in production)
            var passwordHash = HashPassword(password);
            _passwords[email] = passwordHash;

            return new AuthenticationResult(true, null, null, "user-created");
        }
        catch (Exception ex)
        {
            return new AuthenticationResult(false, null, ex.Message, null);
        }
    }

    public async Task<AuthenticationResult> LoginAsync(
        string email,
        string password,
        string ipAddress,
        string userAgent,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userRepository.GetByEmailAsync(email, cancellationToken);
            if (user is null)
                return new AuthenticationResult(false, null, "Invalid credentials", null);

            if (!_passwords.ContainsKey(email) || _passwords[email] != HashPassword(password))
                return new AuthenticationResult(false, null, "Invalid credentials", null);

            var ipHash = HashString(ipAddress);
            var uaHash = HashString(userAgent);

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

            return new AuthenticationResult(true, tokenString, null, user.Id.ToString());
        }
        catch (Exception ex)
        {
            return new AuthenticationResult(false, null, ex.Message, null);
        }
    }

    public Task<bool> ValidateTokenAsync(
        string token,
        string ipAddress,
        string userAgent,
        CancellationToken cancellationToken = default)
    {
        if (!_tokenBindings.ContainsKey(token))
            return Task.FromResult(false);

        var (storedIpHash, storedUaHash) = _tokenBindings[token];
        var currentIpHash = HashString(ipAddress);
        var currentUaHash = HashString(userAgent);

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
