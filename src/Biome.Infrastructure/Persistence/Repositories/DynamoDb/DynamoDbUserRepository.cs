using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using Biome.Domain.Users;
using Biome.Infrastructure.Persistence.Entities;
using Biome.Infrastructure.Persistence.Mappers;
using Biome.SharedKernel.Abstractions;
using Biome.SharedKernel.ValueObjects;
using MediatR;

namespace Biome.Infrastructure.Persistence.Repositories.DynamoDb;

public class DynamoDbUserRepository : IUserRepository, IUnitOfWork
{
    private readonly IDynamoDBContext _context;
    private readonly IAmazonDynamoDB _dynamoDbClient;
    private readonly IPublisher _publisher;

    public DynamoDbUserRepository(
        IDynamoDBContext context,
        IAmazonDynamoDB dynamoDbClient,
        IPublisher publisher)
    {
        _context = context;
        _dynamoDbClient = dynamoDbClient;
        _publisher = publisher;
    }

    public IUnitOfWork UnitOfWork => this;

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.LoadAsync<UserDynamoDbEntity>(id, cancellationToken);
        return UserMapper.ToDomainEntity(entity);
    }

    public async Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default)
    {
        var config = new DynamoDBOperationConfig
        {
            IndexName = "EmailIndex"
        };

        var search = _context.QueryAsync<UserDynamoDbEntity>(email.Value, config);
        var entities = await search.GetRemainingAsync(cancellationToken);
        
        return UserMapper.ToDomainEntity(entities.FirstOrDefault());
    }

    public async Task<User?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var config = new DynamoDBOperationConfig
        {
            IndexName = "RefreshTokenIndex"
        };

        var search = _context.QueryAsync<UserDynamoDbEntity>(refreshToken, config);
        var entities = await search.GetRemainingAsync(cancellationToken);
        
        return UserMapper.ToDomainEntity(entities.FirstOrDefault());
    }

    public void Add(User user)
    {
        var entity = UserMapper.ToDynamoDbEntity(user);
        _context.SaveAsync(entity).GetAwaiter().GetResult();
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // In a real implementation with domain events, we would:
        // 1. Ensure all entities are saved to DynamoDB
        // 2. Publish domain events after successful save
        // 3. Handle any failures and rollbacks
        
        // For now, we'll just return a success indicator
        // The actual entity persistence happens in individual operations
        return 1;
    }

    // Helper method for table creation (useful for LocalStack setup)
    public async Task EnsureTableExistsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _dynamoDbClient.DescribeTableAsync("Users", cancellationToken);
        }
        catch (ResourceNotFoundException)
        {
            var request = new CreateTableRequest
            {
                TableName = "Users",
                AttributeDefinitions = new List<AttributeDefinition>
                {
                    new AttributeDefinition { AttributeName = "Id", AttributeType = "S" },
                    new AttributeDefinition { AttributeName = "EmailIndex", AttributeType = "S" },
                    new AttributeDefinition { AttributeName = "RefreshTokenIndex", AttributeType = "S" }
                },
                KeySchema = new List<KeySchemaElement>
                {
                    new KeySchemaElement { AttributeName = "Id", KeyType = "HASH" }
                },
                GlobalSecondaryIndexes = new List<GlobalSecondaryIndex>
                {
                    new GlobalSecondaryIndex
                    {
                        IndexName = "EmailIndex",
                        KeySchema = new List<KeySchemaElement>
                        {
                            new KeySchemaElement { AttributeName = "EmailIndex", KeyType = "HASH" }
                        },
                        Projection = new Projection { ProjectionType = ProjectionType.ALL },
                        ProvisionedThroughput = new ProvisionedThroughput { ReadCapacityUnits = 1, WriteCapacityUnits = 1 }
                    },
                    new GlobalSecondaryIndex
                    {
                        IndexName = "RefreshTokenIndex",
                        KeySchema = new List<KeySchemaElement>
                        {
                            new KeySchemaElement { AttributeName = "RefreshTokenIndex", KeyType = "HASH" }
                        },
                        Projection = new Projection { ProjectionType = ProjectionType.ALL },
                        ProvisionedThroughput = new ProvisionedThroughput { ReadCapacityUnits = 1, WriteCapacityUnits = 1 }
                    }
                },
                ProvisionedThroughput = new ProvisionedThroughput { ReadCapacityUnits = 1, WriteCapacityUnits = 1 }
            };

            await _dynamoDbClient.CreateTableAsync(request, cancellationToken);
        }
    }
}
