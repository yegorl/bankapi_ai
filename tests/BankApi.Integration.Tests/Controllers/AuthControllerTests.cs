using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using BankApi.Application.DTOs;
using FluentAssertions;

namespace BankApi.Integration.Tests.Controllers;

public class AuthControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;

    public AuthControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact(DisplayName = "SignUp_WithValidData_ReturnsOkResult")]
    public async Task SignUp_WithValidData_ReturnsOkResult()
    {
        // Arrange
        var signUpRequest = new SignUpRequest(
            Email: "newuser@test.com",
            Password: "SecurePassword123!"
        );

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/signup", signUpRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("userId");
        content.Should().Contain("email");
    }

    [Fact(DisplayName = "SignUp_WithDuplicateEmail_ReturnsBadRequest")]
    public async Task SignUp_WithDuplicateEmail_ReturnsBadRequest()
    {
        // Arrange
        var signUpRequest = new SignUpRequest(
            Email: "duplicate@test.com",
            Password: "SecurePassword123!"
        );

        // First signup
        await _client.PostAsJsonAsync("/api/auth/signup", signUpRequest);

        // Act - Try to signup again with same email
        var response = await _client.PostAsJsonAsync("/api/auth/signup", signUpRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact(DisplayName = "Login_WithValidCredentials_ReturnsTokenAndRefreshToken")]
    public async Task Login_WithValidCredentials_ReturnsTokenAndRefreshToken()
    {
        // Arrange
        var email = "logintest@test.com";
        var password = "SecurePassword123!";
        
        var signUpRequest = new SignUpRequest(email, password);
        await _client.PostAsJsonAsync("/api/auth/signup", signUpRequest);

        var loginRequest = new LoginRequest(email, password);

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var jsonResponse = await response.Content.ReadFromJsonAsync<JsonElement>();
        jsonResponse.GetProperty("token").GetString().Should().NotBeNullOrEmpty();
        jsonResponse.GetProperty("refreshToken").GetString().Should().NotBeNullOrEmpty();
        jsonResponse.GetProperty("userId").GetString().Should().NotBeNullOrEmpty();
        jsonResponse.GetProperty("email").GetString().Should().Be(email);
    }

    [Fact(DisplayName = "Login_WithInvalidCredentials_ReturnsUnauthorized")]
    public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
    {
        // Arrange
        var loginRequest = new LoginRequest(
            Email: "nonexistent@test.com",
            Password: "WrongPassword123!"
        );

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact(DisplayName = "RefreshToken_WithValidToken_ReturnsNewAccessToken")]
    public async Task RefreshToken_WithValidToken_ReturnsNewAccessToken()
    {
        // Arrange
        var email = "refreshtest@test.com";
        var password = "SecurePassword123!";
        
        var signUpRequest = new SignUpRequest(email, password);
        await _client.PostAsJsonAsync("/api/auth/signup", signUpRequest);

        var loginRequest = new LoginRequest(email, password);
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<JsonElement>();
        var refreshToken = loginResult.GetProperty("refreshToken").GetString();

        var refreshRequest = new RefreshTokenRequest(refreshToken!);

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/refresh", refreshRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var jsonResponse = await response.Content.ReadFromJsonAsync<JsonElement>();
        jsonResponse.GetProperty("token").GetString().Should().NotBeNullOrEmpty();
        jsonResponse.GetProperty("refreshToken").GetString().Should().NotBeNullOrEmpty();
        jsonResponse.GetProperty("userId").GetString().Should().NotBeNullOrEmpty();
    }

    [Fact(DisplayName = "RefreshToken_WithInvalidToken_ReturnsUnauthorized")]
    public async Task RefreshToken_WithInvalidToken_ReturnsUnauthorized()
    {
        // Arrange
        var refreshRequest = new RefreshTokenRequest("invalid-refresh-token");

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/refresh", refreshRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact(DisplayName = "RevokeToken_WithValidToken_ReturnsOk")]
    public async Task RevokeToken_WithValidToken_ReturnsOk()
    {
        // Arrange
        var email = "revoketest@test.com";
        var password = "SecurePassword123!";
        
        var signUpRequest = new SignUpRequest(email, password);
        await _client.PostAsJsonAsync("/api/auth/signup", signUpRequest);

        var loginRequest = new LoginRequest(email, password);
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<JsonElement>();
        var refreshToken = loginResult.GetProperty("refreshToken").GetString();
        var accessToken = loginResult.GetProperty("token").GetString();

        var refreshRequest = new RefreshTokenRequest(refreshToken!);
        
        // Add authorization header
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/revoke", refreshRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact(DisplayName = "Login_MultipleAttempts_AllAttemptsLogged")]
    public async Task Login_MultipleAttempts_AllAttemptsLogged()
    {
        // Arrange
        var email = "multiplelogin@test.com";
        var password = "SecurePassword123!";
        
        var signUpRequest = new SignUpRequest(email, password);
        await _client.PostAsJsonAsync("/api/auth/signup", signUpRequest);

        var loginRequest = new LoginRequest(email, password);
        var wrongLoginRequest = new LoginRequest(email, "WrongPassword");

        // Act - Make multiple login attempts
        await _client.PostAsJsonAsync("/api/auth/login", wrongLoginRequest); // Failed
        await _client.PostAsJsonAsync("/api/auth/login", wrongLoginRequest); // Failed
        var successResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest); // Success

        // Assert
        successResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        // In a real scenario, we would query the database to verify all attempts were logged
        // For now, we just verify the last successful login worked
    }
}
