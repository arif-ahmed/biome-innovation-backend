namespace Biome.Domain.Users;

using Biome.Domain.Users.Events;
using Biome.Domain.Users.ValueObjects;
using Biome.SharedKernel.Core;
using Biome.SharedKernel.ValueObjects;

public sealed class User : AggregateRoot
{
    private User(Guid id, FirstName firstName, LastName lastName, Email email, string passwordHash, UserRole role)
        : base(id)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PasswordHash = passwordHash;
        Role = role;
    }

    public FirstName FirstName { get; private set; }
    public LastName LastName { get; private set; }
    public Email Email { get; private set; }
    public string PasswordHash { get; private set; }
    public UserRole Role { get; private set; }
    public bool IsEmailVerified { get; private set; }
    public bool IsBanned { get; private set; }

    public static User Create(FirstName firstName, LastName lastName, Email email, string passwordHash, UserRole role)
    {
        var user = new User(Guid.NewGuid(), firstName, lastName, email, passwordHash, role);

        user.RaiseDomainEvent(new UserRegisteredDomainEvent(user.Id));

        return user;
    }

    public void VerifyEmail()
    {
        if (IsEmailVerified)
        {
            return;
        }

        IsEmailVerified = true;
        RaiseDomainEvent(new UserEmailVerifiedDomainEvent(Id));
    }

    public void Ban()
    {
        if (IsBanned)
        {
            return;
        }

        IsBanned = true;
        RaiseDomainEvent(new UserBannedDomainEvent(Id));
    }

    public void ChangePassword(string newPasswordHash)
    {
        if (newPasswordHash == PasswordHash)
        {
            return;
        }

        PasswordHash = newPasswordHash;
    }

    public void UpdateProfile(FirstName firstName, LastName lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }
}
