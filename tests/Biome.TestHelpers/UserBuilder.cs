using Biome.Domain.Users;
using Biome.Domain.Users.ValueObjects;
using Biome.SharedKernel.ValueObjects;

namespace Biome.TestHelpers;

/// <summary>
/// Builder pattern for creating User entities for testing
/// </summary>
public class UserBuilder
{
    private Guid _id = Guid.NewGuid();
    private string _email = "test@example.com";
    private string _firstName = "Test";
    private string _lastName = "User";
    private string _passwordHash = "hashedPassword";
    private bool _isEmailVerified = false;
    private bool _isBanned = false;
    private bool _twoFactorEnabled = false;
    private string? _twoFactorSecret = null;
    private Guid _roleId = Guid.NewGuid();

    public UserBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public UserBuilder WithEmail(string email)
    {
        _email = email;
        return this;
    }

    public UserBuilder WithFirstName(string firstName)
    {
        _firstName = firstName;
        return this;
    }

    public UserBuilder WithLastName(string lastName)
    {
        _lastName = lastName;
        return this;
    }

    public UserBuilder WithPasswordHash(string passwordHash)
    {
        _passwordHash = passwordHash;
        return this;
    }

    public UserBuilder WithEmailVerified(bool isEmailVerified = true)
    {
        _isEmailVerified = isEmailVerified;
        return this;
    }

    public UserBuilder WithBanned(bool isBanned = true)
    {
        _isBanned = isBanned;
        return this;
    }

    public UserBuilder WithTwoFactorEnabled(bool twoFactorEnabled = true, string? secret = null)
    {
        _twoFactorEnabled = twoFactorEnabled;
        _twoFactorSecret = secret ?? "test-secret";
        return this;
    }

    public UserBuilder WithRoleId(Guid roleId)
    {
        _roleId = roleId;
        return this;
    }

    public User Build()
    {
        var email = Email.Create(_email).Value;
        var firstName = FirstName.Create(_firstName).Value;
        var lastName = LastName.Create(_lastName).Value;
        var temporaryPassword = "temp123";
        
        var user = User.Create(firstName, lastName, email, _passwordHash, _roleId, temporaryPassword);
        
        // Use reflection to set private properties for testing
        var idProperty = typeof(User).GetProperty("Id");
        if (idProperty != null && idProperty.CanWrite)
        {
            idProperty.SetValue(user, _id);
        }
        
        var isEmailVerifiedProperty = typeof(User).GetProperty("IsEmailVerified");
        if (isEmailVerifiedProperty != null && isEmailVerifiedProperty.CanWrite)
        {
            isEmailVerifiedProperty.SetValue(user, _isEmailVerified);
        }
        
        var isBannedProperty = typeof(User).GetProperty("IsBanned");
        if (isBannedProperty != null && isBannedProperty.CanWrite)
        {
            isBannedProperty.SetValue(user, _isBanned);
        }
        
        var twoFactorEnabledProperty = typeof(User).GetProperty("TwoFactorEnabled");
        if (twoFactorEnabledProperty != null && twoFactorEnabledProperty.CanWrite)
        {
            twoFactorEnabledProperty.SetValue(user, _twoFactorEnabled);
        }
        
        var twoFactorSecretProperty = typeof(User).GetProperty("TwoFactorSecret");
        if (twoFactorSecretProperty != null && twoFactorSecretProperty.CanWrite)
        {
            twoFactorSecretProperty.SetValue(user, _twoFactorSecret);
        }
        
        return user;
    }

    /// <summary>
    /// Creates a valid user with default values
    /// </summary>
    public static UserBuilder ValidUser() => new UserBuilder();

    /// <summary>
    /// Creates an admin user
    /// </summary>
    public static UserBuilder AdminUser() => new UserBuilder()
        .WithEmail("admin@example.com")
        .WithFirstName("Admin")
        .WithLastName("User")
        .WithEmailVerified(true);

    /// <summary>
    /// Creates a banned user
    /// </summary>
    public static UserBuilder BannedUser() => new UserBuilder()
        .WithEmail("banned@example.com")
        .WithFirstName("Banned")
        .WithLastName("User")
        .WithBanned(true);

    /// <summary>
    /// Creates a user with two-factor authentication enabled
    /// </summary>
    public static UserBuilder TwoFactorUser() => new UserBuilder()
        .WithEmail("2fa@example.com")
        .WithFirstName("TwoFactor")
        .WithLastName("User")
        .WithTwoFactorEnabled(true);

    /// <summary>
    /// Creates a verified user
    /// </summary>
    public static UserBuilder VerifiedUser() => new UserBuilder()
        .WithEmail("verified@example.com")
        .WithFirstName("Verified")
        .WithLastName("User")
        .WithEmailVerified(true);
}

/// <summary>
/// Factory for creating common test user scenarios
/// </summary>
public static class TestUserFactory
{
    public static User CreateStandardUser() => UserBuilder.ValidUser().Build();
    
    public static User CreateAdminUser() => UserBuilder.AdminUser().Build();
    
    public static User CreateBannedUser() => UserBuilder.BannedUser().Build();
    
    public static User CreateTwoFactorUser() => UserBuilder.TwoFactorUser().Build();
    
    public static User CreateVerifiedUser() => UserBuilder.VerifiedUser().Build();
    
    public static User CreateUserWithEmail(string email) => 
        UserBuilder.ValidUser().WithEmail(email).Build();
    
    public static User CreateUserWithNames(string firstName, string lastName) => 
        UserBuilder.ValidUser().WithFirstName(firstName).WithLastName(lastName).Build();
    
    public static List<User> CreateMultipleUsers(int count)
    {
        var users = new List<User>();
        for (int i = 1; i <= count; i++)
        {
            users.Add(UserBuilder.ValidUser()
                .WithEmail($"user{i}@example.com")
                .WithFirstName($"User{i}")
                .WithLastName($"Test{i}")
                .Build());
        }
        return users;
    }
}
