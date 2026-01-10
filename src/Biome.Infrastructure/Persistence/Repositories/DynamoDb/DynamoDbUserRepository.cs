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
        // For now, we'll use a basic scan operation to find user by email
        // In production, you would create proper GSIs and use query operations for better performance
        var scanRequest = new ScanRequest
        {
            TableName = "Users",
            FilterExpression = "Email = :email",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                [":email"] = new AttributeValue { S = email.Value }
            }
        };

        var response = await _dynamoDbClient.ScanAsync(scanRequest, cancellationToken);
        var item = response.Items.FirstOrDefault();
        
        if (item == null) return null;
        
        // Convert DynamoDB item to entity
        var entity = new UserDynamoDbEntity
        {
            Id = Guid.Parse(item["Id"].S),
            FirstName = item["FirstName"].S,
            LastName = item["LastName"].S,
            Email = item["Email"].S,
            PasswordHash = item["PasswordHash"].S,
            RoleId = Guid.Parse(item["RoleId"].S),
            IsEmailVerified = bool.Parse(item["IsEmailVerified"].BOOL.ToString()),
            IsBanned = bool.Parse(item["IsBanned"].BOOL.ToString()),
            TwoFactorEnabled = bool.Parse(item["TwoFactorEnabled"].BOOL.ToString()),
            TwoFactorSecret = item.ContainsKey("TwoFactorSecret") ? item["TwoFactorSecret"].S : null
        };

        return UserMapper.ToDomainEntity(entity);
    }

    public async Task<User?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        // For now, we'll use a basic scan operation to find user by refresh token
        // In production, you would create proper GSIs and use query operations for better performance
        var scanRequest = new ScanRequest
        {
            TableName = "Users",
            FilterExpression = "RefreshToken.Token = :refreshToken",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                [":refreshToken"] = new AttributeValue { S = refreshToken }
            }
        };

        var response = await _dynamoDbClient.ScanAsync(scanRequest, cancellationToken);
        var item = response.Items.FirstOrDefault();
        
        if (item == null) return null;
        
        // Convert DynamoDB item to entity
        var entity = new UserDynamoDbEntity
        {
            Id = Guid.Parse(item["Id"].S),
            FirstName = item["FirstName"].S,
            LastName = item["LastName"].S,
            Email = item["Email"].S,
            PasswordHash = item["PasswordHash"].S,
            RoleId = Guid.Parse(item["RoleId"].S),
            IsEmailVerified = bool.Parse(item["IsEmailVerified"].BOOL.ToString()),
            IsBanned = bool.Parse(item["IsBanned"].BOOL.ToString()),
            TwoFactorEnabled = bool.Parse(item["TwoFactorEnabled"].BOOL.ToString()),
            TwoFactorSecret = item.ContainsKey("TwoFactorSecret") ? item["TwoFactorSecret"].S : null
        };

        return UserMapper.ToDomainEntity(entity);
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
