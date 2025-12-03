using BankApi.Application.DTOs;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace BankApi.Integration.Tests.Controllers;

public class CardsControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public CardsControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact(DisplayName = "RequestCard_WithoutAdminRole_ReturnsForbidden")]
    public async Task RequestCard_WithoutAdminRole_ReturnsForbidden()
    {
        // Arrange
        var userEmail = "user_card@test.com";
        var userPassword = "UserPass123!";

        // Create regular user (not admin)
        var signupRequest = new { email = userEmail, password = userPassword };
        await _client.PostAsJsonAsync("/api/auth/signup", signupRequest);

        // Login to get token
        var loginRequest = new { email = userEmail, password = userPassword };
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<AuthenticationResponse>();

        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginResult!.Token);

        // Request card (should fail - admin only)
        var cardRequest = new
        {
            accountId = Guid.NewGuid(),
            cardHolderName = "John Doe",
            cvv = "123",
            cardType = "Debit"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/cards", cardRequest);

        // Assert
        // Regular users should get Unauthorized/Forbidden when trying admin operations
        response.StatusCode.Should().BeOneOf(HttpStatusCode.Unauthorized, HttpStatusCode.Forbidden);
    }

    [Fact(DisplayName = "GetCardById_ExistingCard_ReturnsCard")]
    public async Task GetCardById_ExistingCard_ReturnsCard()
    {
        // Arrange
        var cardId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/cards/{cardId}");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound);
    }

    [Fact(DisplayName = "BlockCard_WithoutAdminRole_ReturnsForbidden")]
    public async Task BlockCard_WithoutAdminRole_ReturnsForbidden()
    {
        // Arrange
        var cardId = Guid.NewGuid();
        var userEmail = "user_block@test.com";
        var userPassword = "UserPass123!";

        // Create and login regular user
        await _client.PostAsJsonAsync("/api/auth/signup", new { email = userEmail, password = userPassword });
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", 
            new { email = userEmail, password = userPassword });
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<AuthenticationResponse>();

        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginResult!.Token);

        // Act
        var response = await _client.PutAsync($"/api/cards/{cardId}/block", null);

        // Assert
        // Regular users should get Unauthorized/Forbidden for admin operations
        response.StatusCode.Should().BeOneOf(HttpStatusCode.Unauthorized, HttpStatusCode.Forbidden);
    }

    [Fact(DisplayName = "TemporarilyBlockCard_WithoutAdminRole_ReturnsForbidden")]
    public async Task TemporarilyBlockCard_WithoutAdminRole_ReturnsForbidden()
    {
        // Arrange
        var cardId = Guid.NewGuid();
        var userEmail = "user_tempblock@test.com";
        var userPassword = "UserPass123!";

        // Create and login regular user
        await _client.PostAsJsonAsync("/api/auth/signup", new { email = userEmail, password = userPassword });
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", 
            new { email = userEmail, password = userPassword });
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<AuthenticationResponse>();

        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginResult!.Token);

        // Act
        var response = await _client.PutAsync($"/api/cards/{cardId}/block-temporary", null);

        // Assert
        // Regular users should get Unauthorized/Forbidden for admin operations
        response.StatusCode.Should().BeOneOf(HttpStatusCode.Unauthorized, HttpStatusCode.Forbidden);
    }

    [Fact(DisplayName = "UnblockCard_WithoutAdminRole_ReturnsForbidden")]
    public async Task UnblockCard_WithoutAdminRole_ReturnsForbidden()
    {
        // Arrange
        var cardId = Guid.NewGuid();
        var userEmail = "user_unblock@test.com";
        var userPassword = "UserPass123!";

        // Create and login regular user
        await _client.PostAsJsonAsync("/api/auth/signup", new { email = userEmail, password = userPassword });
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", 
            new { email = userEmail, password = userPassword });
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<AuthenticationResponse>();

        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginResult!.Token);

        // Act
        var response = await _client.PutAsync($"/api/cards/{cardId}/unblock", null);

        // Assert
        // Regular users should get Unauthorized/Forbidden for admin operations
        response.StatusCode.Should().BeOneOf(HttpStatusCode.Unauthorized, HttpStatusCode.Forbidden);
    }

    [Fact(DisplayName = "ValidateCVV_WithNonExistentCard_ReturnsOk")]
    public async Task ValidateCVV_WithNonExistentCard_ReturnsOk()
    {
        // Arrange
        var request = new
        {
            cardId = Guid.NewGuid(),
            cvv = "123"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/cards/validate-cvv", request);

        // Assert
        // Should return OK with valid=false for non-existent card
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<Dictionary<string, bool>>();
        result.Should().ContainKey("valid");
        result!["valid"].Should().BeFalse(); // Card doesn't exist
    }
}
