using BankApi.Application.DTOs;
using BankApi.Domain.Aggregates.Users;
using BankApi.Domain.Interfaces;
using BankApi.Domain.Repositories;
using BankApi.Domain.ValueObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;

namespace BankApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthenticationService _authService;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AuthController(IAuthenticationService authService, IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _authService = authService;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Sign up a new user
    /// </summary>
    [HttpPost("signup")]
    [AllowAnonymous]
    public async Task<IActionResult> SignUp([FromBody] SignUpRequest request)
    {
        try
        {
            var email = EmailAddress.Create(request.Email);
            
            // Check if user already exists
            var existingUser = await _userRepository.GetByEmailAsync(request.Email);
            if (existingUser is not null)
                return BadRequest(new { error = "User with this email already exists" });

            // Hash password
            var passwordHash = HashPassword(request.Password);

            // Create user with User role by default
            var user = BankApi.Domain.Aggregates.Users.User.Create(email, passwordHash, UserRole.User);
            await _userRepository.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return Ok(new { userId = user.Id, email = user.Email.Value, role = user.Role.ToString() });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Login and get JWT token
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var userAgent = HttpContext.Request.Headers.UserAgent.ToString();

            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user is null)
                return Unauthorized(new { error = "Invalid credentials" });

            // Verify password
            if (!VerifyPassword(request.Password, user.PasswordHash))
                return Unauthorized(new { error = "Invalid credentials" });

            var result = await _authService.LoginAsync(request.Email, request.Password, ipAddress, userAgent);
            
            if (!result.Success)
                return Unauthorized(new { error = result.Error });

            return Ok(new AuthenticationResponse(
                result.Token!,
                result.UserId!,
                request.Email,
                DateTime.UtcNow.AddMinutes(60)));
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // NOTE: SHA256 is used for demonstration purposes only
    // PRODUCTION: Use BCrypt, scrypt, or Argon2 for password hashing
    // These algorithms are designed to be computationally expensive and resistant to brute force
    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }

    private static bool VerifyPassword(string password, string hash)
    {
        return HashPassword(password) == hash;
    }
}
