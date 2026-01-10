using Amazon.DynamoDBv2.DataModel;

namespace Biome.Infrastructure.Persistence.Entities;

[DynamoDBTable("Users")]
public class UserDynamoDbEntity
{
    [DynamoDBHashKey]
    public Guid Id { get; set; }

    [DynamoDBProperty]
    public string FirstName { get; set; } = string.Empty;

    [DynamoDBProperty]
    public string LastName { get; set; } = string.Empty;

    [DynamoDBProperty]
    public string Email { get; set; } = string.Empty;

    [DynamoDBProperty]
    public string PasswordHash { get; set; } = string.Empty;

    [DynamoDBProperty]
    public Guid RoleId { get; set; }

    [DynamoDBProperty]
    public bool IsEmailVerified { get; set; }

    [DynamoDBProperty]
    public bool IsBanned { get; set; }

    [DynamoDBProperty]
    public bool TwoFactorEnabled { get; set; }

    [DynamoDBProperty]
    public string? TwoFactorSecret { get; set; }

    [DynamoDBProperty]
    public RefreshTokenDynamoDbEntity? RefreshToken { get; set; }

    [DynamoDBProperty]
    public PasswordResetDynamoDbEntity? PasswordReset { get; set; }
}

public class RefreshTokenDynamoDbEntity
{
    [DynamoDBProperty]
    public string Token { get; set; } = string.Empty;

    [DynamoDBProperty]
    public DateTime ExpiryOnUtc { get; set; }
}

public class PasswordResetDynamoDbEntity
{
    [DynamoDBProperty]
    public string Token { get; set; } = string.Empty;

    [DynamoDBProperty]
    public DateTime Expiry { get; set; }
}
