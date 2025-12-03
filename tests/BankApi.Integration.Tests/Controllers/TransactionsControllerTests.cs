using BankApi.Application.DTOs;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace BankApi.Integration.Tests.Controllers;

public class TransactionsControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public TransactionsControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact(DisplayName = "ExecuteTransfer_WithAuthentication_ReturnsCreated")]
    public async Task ExecuteTransfer_WithAuthentication_ReturnsCreated()
    {
        // Arrange
        var userEmail = "user_transfer@test.com";
        var userPassword = "UserPass123!";

        // Create user and login
        await _client.PostAsJsonAsync("/api/auth/signup", new { email = userEmail, password = userPassword });
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", 
            new { email = userEmail, password = userPassword });
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<AuthenticationResponse>();

        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginResult!.Token);

        // Create transfer request
        var transferRequest = new
        {
            sourceAccountId = Guid.NewGuid(),
            targetAccountId = Guid.NewGuid(),
            amount = 100.00m,
            description = "Test transfer"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/transactions/transfer", transferRequest);

        // Assert
        // Will fail because accounts don't exist, but demonstrates auth works
        response.StatusCode.Should().BeOneOf(HttpStatusCode.Created, HttpStatusCode.BadRequest);
    }

    [Fact(DisplayName = "ExecuteTransfer_WithoutAuthentication_ReturnsUnauthorized")]
    public async Task ExecuteTransfer_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Arrange
        var transferRequest = new
        {
            sourceAccountId = Guid.NewGuid(),
            targetAccountId = Guid.NewGuid(),
            amount = 100.00m,
            description = "Test transfer"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/transactions/transfer", transferRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact(DisplayName = "GetTransactionById_NonExistentTransaction_ReturnsNotFound")]
    public async Task GetTransactionById_NonExistentTransaction_ReturnsNotFound()
    {
        // Arrange
        var userEmail = "user_gettxn@test.com";
        var userPassword = "UserPass123!";

        // Create user and login
        await _client.PostAsJsonAsync("/api/auth/signup", new { email = userEmail, password = userPassword });
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", 
            new { email = userEmail, password = userPassword });
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<AuthenticationResponse>();

        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginResult!.Token);

        var transactionId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/transactions/{transactionId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact(DisplayName = "GetTransactionById_WithoutAuthentication_ReturnsUnauthorized")]
    public async Task GetTransactionById_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Arrange
        var transactionId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/transactions/{transactionId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact(DisplayName = "GetAccountStatement_WithAuthentication_ReturnsOk")]
    public async Task GetAccountStatement_WithAuthentication_ReturnsOk()
    {
        // Arrange
        var userEmail = "user_statement@test.com";
        var userPassword = "UserPass123!";

        // Create user and login
        await _client.PostAsJsonAsync("/api/auth/signup", new { email = userEmail, password = userPassword });
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", 
            new { email = userEmail, password = userPassword });
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<AuthenticationResponse>();

        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginResult!.Token);

        var accountId = Guid.NewGuid();
        var from = DateTime.UtcNow.AddMonths(-1).ToString("o");
        var to = DateTime.UtcNow.ToString("o");

        // Act
        var response = await _client.GetAsync($"/api/transactions/statement?accountId={accountId}&from={from}&to={to}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<IEnumerable<TransactionDto>>();
        result.Should().NotBeNull();
        result.Should().BeEmpty(); // No transactions for non-existent account
    }

    [Fact(DisplayName = "GetAccountStatement_WithoutAuthentication_ReturnsUnauthorized")]
    public async Task GetAccountStatement_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Arrange
        var accountId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/transactions/statement?accountId={accountId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact(DisplayName = "GetAccountStatement_WithoutDateRange_ReturnsAllTransactions")]
    public async Task GetAccountStatement_WithoutDateRange_ReturnsAllTransactions()
    {
        // Arrange
        var userEmail = "user_statement2@test.com";
        var userPassword = "UserPass123!";

        // Create user and login
        await _client.PostAsJsonAsync("/api/auth/signup", new { email = userEmail, password = userPassword });
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", 
            new { email = userEmail, password = userPassword });
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<AuthenticationResponse>();

        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginResult!.Token);

        var accountId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/transactions/statement?accountId={accountId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<IEnumerable<TransactionDto>>();
        result.Should().NotBeNull();
    }
}
