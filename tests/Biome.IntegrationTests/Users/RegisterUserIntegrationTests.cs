using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Biome.Api;
using Biome.Application.Users.Commands.RegisterUser;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Biome.IntegrationTests.Users;

public class RegisterUserIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public RegisterUserIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((context, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Persistence:Provider"] = "DynamoDb",
                    ["Persistence:LocalStack:EnsureTablesCreated"] = "true",
                    ["AWS:Region"] = "us-east-1",
                    ["AWS:AccessKey"] = "test",
                    ["AWS:SecretKey"] = "test",
                    ["AWS:ServiceURL"] = "http://localhost:8000"
                });
            });
        });

        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task Should_Register_User_Successfully()
    {
        // Arrange
        var command = new RegisterUserCommand(
            "test@example.com",
            "Password123!",
            "John",
            "Doe");

        // Act
        var response = await _client.PostAsJsonAsync("/api/users/register", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var result = await response.Content.ReadFromJsonAsync<JsonElement>();
        result.GetProperty("success").GetBoolean().Should().BeTrue();
        result.GetProperty("data").GetProperty("id").GetGuid().Should().NotBe(Guid.Empty);
        result.GetProperty("data").GetProperty("email").GetString().Should().Be(command.Email);
        result.GetProperty("data").GetProperty("firstName").GetString().Should().Be(command.FirstName);
        result.GetProperty("data").GetProperty("lastName").GetString().Should().Be(command.LastName);
    }

    [Fact]
    public async Task Should_Return_Bad_Request_When_Email_Is_Invalid()
    {
        // Arrange
        var command = new RegisterUserCommand(
            "invalid-email",
            "Password123!",
            "John",
            "Doe");

        // Act
        var response = await _client.PostAsJsonAsync("/api/users/register", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        
        var result = await response.Content.ReadFromJsonAsync<JsonElement>();
        result.GetProperty("success").GetBoolean().Should().BeFalse();
        result.GetProperty("errors").GetArrayLength().Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task Should_Return_Bad_Request_When_Password_Is_Weak()
    {
        // Arrange
        var command = new RegisterUserCommand(
            "test@example.com",
            "weak",
            "John",
            "Doe");

        // Act
        var response = await _client.PostAsJsonAsync("/api/users/register", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        
        var result = await response.Content.ReadFromJsonAsync<JsonElement>();
        result.GetProperty("success").GetBoolean().Should().BeFalse();
        result.GetProperty("errors").GetArrayLength().Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task Should_Return_Bad_Request_When_First_Name_Is_Empty()
    {
        // Arrange
        var command = new RegisterUserCommand(
            "test@example.com",
            "Password123!",
            "",
            "Doe");

        // Act
        var response = await _client.PostAsJsonAsync("/api/users/register", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        
        var result = await response.Content.ReadFromJsonAsync<JsonElement>();
        result.GetProperty("success").GetBoolean().Should().BeFalse();
        result.GetProperty("errors").GetArrayLength().Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task Should_Return_Bad_Request_When_Last_Name_Is_Empty()
    {
        // Arrange
        var command = new RegisterUserCommand(
            "test@example.com",
            "Password123!",
            "John",
            "");

        // Act
        var response = await _client.PostAsJsonAsync("/api/users/register", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        
        var result = await response.Content.ReadFromJsonAsync<JsonElement>();
        result.GetProperty("success").GetBoolean().Should().BeFalse();
        result.GetProperty("errors").GetArrayLength().Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task Should_Return_Conflict_When_Email_Already_Exists()
    {
        // Arrange
        var email = "duplicate@example.com";
        var command1 = new RegisterUserCommand(email, "Password123!", "John", "Doe");
        var command2 = new RegisterUserCommand(email, "Password456!", "Jane", "Smith");

        // Act - First registration
        var response1 = await _client.PostAsJsonAsync("/api/users/register", command1);
        response1.StatusCode.Should().Be(HttpStatusCode.OK);

        // Act - Second registration with same email
        var response2 = await _client.PostAsJsonAsync("/api/users/register", command2);

        // Assert
        response2.StatusCode.Should().Be(HttpStatusCode.Conflict);
        
        var result = await response2.Content.ReadFromJsonAsync<JsonElement>();
        result.GetProperty("success").GetBoolean().Should().BeFalse();
    }

    [Fact]
    public async Task Should_Handle_Multiple_Validation_Errors()
    {
        // Arrange
        var command = new RegisterUserCommand(
            "invalid-email",
            "weak",
            "",
            "");

        // Act
        var response = await _client.PostAsJsonAsync("/api/users/register", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        
        var result = await response.Content.ReadFromJsonAsync<JsonElement>();
        result.GetProperty("success").GetBoolean().Should().BeFalse();
        result.GetProperty("errors").GetArrayLength().Should().BeGreaterThan(2); // At least 3-4 errors
    }

    [Fact]
    public async Task Should_Register_User_With_Max_Length_Names()
    {
        // Arrange
        var firstName = new string('A', 100); // Max length
        var lastName = new string('B', 100); // Max length
        var command = new RegisterUserCommand(
            "maxlength@example.com",
            "Password123!",
            firstName,
            lastName);

        // Act
        var response = await _client.PostAsJsonAsync("/api/users/register", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var result = await response.Content.ReadFromJsonAsync<JsonElement>();
        result.GetProperty("success").GetBoolean().Should().BeTrue();
        result.GetProperty("data").GetProperty("firstName").GetString().Should().Be(firstName);
        result.GetProperty("data").GetProperty("lastName").GetString().Should().Be(lastName);
    }

    [Fact]
    public async Task Should_Handle_Unicode_Characters_In_Names()
    {
        // Arrange
        var command = new RegisterUserCommand(
            "unicode@example.com",
            "Password123!",
            "Jöhn",
            "Smïth");

        // Act
        var response = await _client.PostAsJsonAsync("/api/users/register", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var result = await response.Content.ReadFromJsonAsync<JsonElement>();
        result.GetProperty("success").GetBoolean().Should().BeTrue();
        result.GetProperty("data").GetProperty("firstName").GetString().Should().Be("Jöhn");
        result.GetProperty("data").GetProperty("lastName").GetString().Should().Be("Smïth");
    }

    [Fact]
    public async Task Should_Return_Bad_Request_For_Too_Long_Names()
    {
        // Arrange
        var firstName = new string('A', 101); // Too long
        var command = new RegisterUserCommand(
            "toolong@example.com",
            "Password123!",
            firstName,
            "Doe");

        // Act
        var response = await _client.PostAsJsonAsync("/api/users/register", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        
        var result = await response.Content.ReadFromJsonAsync<JsonElement>();
        result.GetProperty("success").GetBoolean().Should().BeFalse();
    }
}
