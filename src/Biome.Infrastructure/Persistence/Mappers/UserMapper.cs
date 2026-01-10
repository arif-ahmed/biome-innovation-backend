using Biome.Domain.Users;
using Biome.Domain.Users.ValueObjects;
using Biome.Infrastructure.Persistence.Entities;
using Biome.SharedKernel.ValueObjects;

namespace Biome.Infrastructure.Persistence.Mappers;

public static class UserMapper
{
    public static UserDynamoDbEntity ToDynamoDbEntity(User user)
    {
        return new UserDynamoDbEntity
        {
            Id = user.Id,
            FirstName = user.FirstName.Value,
            LastName = user.LastName.Value,
            Email = user.Email.Value,
            PasswordHash = user.PasswordHash,
            RoleId = user.RoleId,
            IsEmailVerified = user.IsEmailVerified,
            IsBanned = user.IsBanned,
            TwoFactorEnabled = user.TwoFactorEnabled,
            TwoFactorSecret = user.TwoFactorSecret,
            RefreshToken = user.RefreshToken != null 
                ? new RefreshTokenDynamoDbEntity
                {
                    Token = user.RefreshToken.Token,
                    ExpiryOnUtc = user.RefreshToken.ExpiryOnUtc
                }
                : null,
            PasswordReset = user.PasswordReset != null
                ? new PasswordResetDynamoDbEntity
                {
                    Token = user.PasswordReset.Token,
                    Expiry = user.PasswordReset.Expiry
                }
                : null
        };
    }

    public static User? ToDomainEntity(UserDynamoDbEntity? entity)
    {
        if (entity == null)
            return null;

        // Create user instance using reflection or factory method
        // Since User has private constructors, we need to use a different approach
        var user = (User)Activator.CreateInstance(
            typeof(User),
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public,
            null,
            new object[]
            {
                entity.Id,
                FirstName.Create(entity.FirstName).Value,
                LastName.Create(entity.LastName).Value,
                Email.Create(entity.Email).Value,
                entity.PasswordHash,
                entity.RoleId
            },
            null)!;

        // Set private properties using reflection
        typeof(User).GetProperty(nameof(User.IsEmailVerified))?.SetValue(user, entity.IsEmailVerified);
        typeof(User).GetProperty(nameof(User.IsBanned))?.SetValue(user, entity.IsBanned);
        typeof(User).GetProperty(nameof(User.TwoFactorEnabled))?.SetValue(user, entity.TwoFactorEnabled);
        typeof(User).GetProperty(nameof(User.TwoFactorSecret))?.SetValue(user, entity.TwoFactorSecret);

        // Set RefreshToken if exists
        if (entity.RefreshToken != null)
        {
            var refreshToken = RefreshToken.Create(entity.RefreshToken.Token, entity.RefreshToken.ExpiryOnUtc);
            typeof(User).GetMethod(nameof(User.SetRefreshToken))?.Invoke(user, new object[] { refreshToken });
        }

        // Set PasswordReset if exists
        if (entity.PasswordReset != null)
        {
            var passwordReset = new PasswordReset(entity.PasswordReset.Token, entity.PasswordReset.Expiry);
            typeof(User).GetProperty(nameof(User.PasswordReset))?.SetValue(user, passwordReset);
        }

        return user;
    }
}
