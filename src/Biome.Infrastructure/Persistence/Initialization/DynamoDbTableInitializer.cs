using Amazon.DynamoDBv2;
using Biome.Infrastructure.Persistence.Configurations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Biome.Infrastructure.Persistence.Initialization;

public class DynamoDbTableInitializer
{
    private readonly IAmazonDynamoDB _dynamoDbClient;
    private readonly IOptions<PersistenceSettings> _persistenceSettings;
    private readonly ILogger<DynamoDbTableInitializer> _logger;

    public DynamoDbTableInitializer(
        IAmazonDynamoDB dynamoDbClient,
        IOptions<PersistenceSettings> persistenceSettings,
        ILogger<DynamoDbTableInitializer> logger)
    {
        _dynamoDbClient = dynamoDbClient;
        _persistenceSettings = persistenceSettings;
        _logger = logger;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        if (!_persistenceSettings.Value.LocalStack.EnsureTablesCreated)
        {
            _logger.LogInformation("Table creation is disabled. Skipping DynamoDB table initialization.");
            return;
        }

        _logger.LogInformation("Initializing DynamoDB tables...");

        try
        {
            await CreateUsersTableAsync(cancellationToken);
            _logger.LogInformation("DynamoDB tables initialized successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initializing DynamoDB tables");
            throw;
        }
    }

    private async Task CreateUsersTableAsync(CancellationToken cancellationToken)
    {
        const string tableName = "Users";

        try
        {
            await _dynamoDbClient.DescribeTableAsync(tableName, cancellationToken);
            _logger.LogInformation("Table {TableName} already exists", tableName);
            return;
        }
        catch (Amazon.DynamoDBv2.Model.ResourceNotFoundException)
        {
            _logger.LogInformation("Creating table {TableName}", tableName);
        }

        var request = new Amazon.DynamoDBv2.Model.CreateTableRequest
        {
            TableName = tableName,
            AttributeDefinitions = new List<Amazon.DynamoDBv2.Model.AttributeDefinition>
            {
                new() { AttributeName = "Id", AttributeType = "S" }
            },
            KeySchema = new List<Amazon.DynamoDBv2.Model.KeySchemaElement>
            {
                new() { AttributeName = "Id", KeyType = "HASH" }
            },
            ProvisionedThroughput = new() { ReadCapacityUnits = 1, WriteCapacityUnits = 1 }
        };

        var response = await _dynamoDbClient.CreateTableAsync(request, cancellationToken);

        // Wait for table to become active
        var tableStatus = response.TableDescription.TableStatus;
        while (tableStatus != "ACTIVE")
        {
            await Task.Delay(1000, cancellationToken);
            var describeResponse = await _dynamoDbClient.DescribeTableAsync(tableName, cancellationToken);
            tableStatus = describeResponse.Table.TableStatus;
        }

        _logger.LogInformation("Table {TableName} created and is now active", tableName);
    }
}
