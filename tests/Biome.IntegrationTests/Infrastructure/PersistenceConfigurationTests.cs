using Biome.Domain.Support;
using Biome.Infrastructure.Persistence.Repositories.DynamoDb;
using Biome.Infrastructure.Persistence.Repositories;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Biome.IntegrationTests.Infrastructure;

public class PersistenceConfigurationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public PersistenceConfigurationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public void DI_ShouldResolveInMemoryRepository_WhenProviderIsInMemory()
    {
        // Arrange
        using var factory = _factory.WithWebHostBuilder(builder =>
        {
            builder.UseSetting("Persistence:Provider", "InMemory");
        });

        // Act
        using var scope = factory.Services.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<ITicketRepository>();

        // Assert
        repository.Should().BeOfType<InMemoryTicketRepository>();
    }

    [Fact]
    public void DI_ShouldResolveDynamoDbRepository_WhenProviderIsDynamoDb()
    {
        // Arrange
        using var factory = _factory.WithWebHostBuilder(builder =>
        {
            builder.UseSetting("Persistence:Provider", "DynamoDb");
            builder.UseSetting("AWS:Region", "us-east-1");
            builder.UseSetting("AWS:AccessKey", "test");
            builder.UseSetting("AWS:SecretKey", "test");
            builder.UseSetting("AWS:ServiceURL", "http://localhost:8000");
        });

        // Act
        using var scope = factory.Services.CreateScope();
        
        var settings = scope.ServiceProvider.GetService<Microsoft.Extensions.Options.IOptions<Biome.Infrastructure.Persistence.Configurations.PersistenceSettings>>();
        settings.Value.Provider.Should().Be("DynamoDb", "Provider should be configured to DynamoDb");

        // Verify dependencies
        var amazonDynamo = scope.ServiceProvider.GetService<Amazon.DynamoDBv2.IAmazonDynamoDB>();
        amazonDynamo.Should().NotBeNull("IAmazonDynamoDB should be registered");

        var repository = scope.ServiceProvider.GetRequiredService<ITicketRepository>();

        // Assert
        repository.Should().BeOfType<DynamoDbTicketRepository>();
    }
}
