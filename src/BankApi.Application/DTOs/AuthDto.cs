namespace BankApi.Application.DTOs;

public record SignUpRequest(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    string Phone,
    DateTime DateOfBirth);

public record LoginRequest(
    string Email,
    string Password);

public record AuthenticationResponse(
    string Token,
    string UserId,
    string Email,
    DateTime ExpiresAt);
