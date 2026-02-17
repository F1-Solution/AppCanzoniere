using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace AppCanzoniere.Backend.IntegrationTests;

public class AccountIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public AccountIntegrationTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task Register_ValidUser_ReturnsOkWithToken()
    {
        // Arrange
        var registerRequest = new
        {
            Email = $"testuser{Guid.NewGuid()}@test.com",
            Password = "Test123!",
            ConfirmPassword = "Test123!"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/account/register", registerRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var authResponse = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
        authResponse.Should().NotBeNull();
        authResponse!.Token.Should().NotBeNullOrEmpty();
        authResponse.Email.Should().Be(registerRequest.Email);
        authResponse.Expiration.Should().BeAfter(DateTime.UtcNow);
    }

    [Fact]
    public async Task Register_DuplicateEmail_ReturnsBadRequest()
    {
        // Arrange
        var email = $"duplicate{Guid.NewGuid()}@test.com";
        var registerRequest = new
        {
            Email = email,
            Password = "Test123!",
            ConfirmPassword = "Test123!"
        };

        // Act - Register first time
        var firstResponse = await _client.PostAsJsonAsync("/api/account/register", registerRequest);
        firstResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Act - Register second time with same email
        var secondResponse = await _client.PostAsJsonAsync("/api/account/register", registerRequest);

        // Assert
        secondResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsOkWithToken()
    {
        // Arrange - Register a user first
        var email = $"logintest{Guid.NewGuid()}@test.com";
        var password = "Login123!";
        var registerRequest = new
        {
            Email = email,
            Password = password,
            ConfirmPassword = password
        };
        await _client.PostAsJsonAsync("/api/account/register", registerRequest);

        var loginRequest = new
        {
            Email = email,
            Password = password
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/account/login", loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var authResponse = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
        authResponse.Should().NotBeNull();
        authResponse!.Token.Should().NotBeNullOrEmpty();
        authResponse.Email.Should().Be(email);
    }

    [Fact]
    public async Task Login_InvalidCredentials_ReturnsUnauthorized()
    {
        // Arrange
        var loginRequest = new
        {
            Email = "nonexistent@test.com",
            Password = "WrongPassword123!"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/account/login", loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Register_PasswordMismatch_ReturnsBadRequest()
    {
        // Arrange
        var registerRequest = new
        {
            Email = $"mismatch{Guid.NewGuid()}@test.com",
            Password = "Test123!",
            ConfirmPassword = "DifferentPassword123!"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/account/register", registerRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
