namespace Biome.Domain.Users;

using Biome.Domain.Users.Enums;
using Biome.Domain.Users.Events;
using Biome.Domain.Users.ValueObjects;
using Biome.SharedKernel.Core;
using Biome.SharedKernel.Primitives;
using Biome.SharedKernel.ValueObjects;

public sealed class User : AggregateRoot
{
    private User(Guid id, FirstName firstName, LastName lastName, Email email, string passwordHash, Guid roleId)
        : base(id)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PasswordHash = passwordHash;
        RoleId = roleId;
    }

    public FirstName FirstName { get; private set; }
    public LastName LastName { get; private set; }
    public Email Email { get; private set; }
    public string PasswordHash { get; private set; }
    public Guid RoleId { get; private set; }
    public bool IsEmailVerified { get; private set; }
    public bool IsBanned { get; private set; }

    public static User Register(FirstName firstName, LastName lastName, Email email, string passwordHash, Guid roleId)
    {
        var user = new User(Guid.NewGuid(), firstName, lastName, email, passwordHash, roleId);

        user.RaiseDomainEvent(new UserRegisteredDomainEvent(user.Id));

        return user;
    }

    public static User Create(FirstName firstName, LastName lastName, Email email, string passwordHash, Guid roleId, string temporaryPassword)
    {
        var user = new User(Guid.NewGuid(), firstName, lastName, email, passwordHash, roleId);

        user.RaiseDomainEvent(new UserCreatedDomainEvent(user.Id, temporaryPassword));

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

    public void AssignRole(Guid roleId)
    {
        RoleId = roleId;
    }

    public Result EnsureLoginEligibility()
    {
        if (IsBanned)
        {
            return Result.Failure(new Error("User.Banned", "The user account is banned."));
        }

        return Result.Success();
    }

    public RefreshToken? RefreshToken { get; private set; }
    public PasswordReset? PasswordReset { get; private set; }

    public void SetRefreshToken(RefreshToken refreshToken)
    {
        RefreshToken = refreshToken;
    }

    public void RevokeRefreshToken()
    {
        RefreshToken = null;
    }

    public void RequestPasswordReset(string token, DateTime expiry)
    {
        PasswordReset = new PasswordReset(token, expiry);

        RaiseDomainEvent(new UserPasswordResetRequestedDomainEvent(Id, token));
    }

    public Result ResetPassword(string token, string newPasswordHash)
    {
        if (PasswordReset is null || PasswordReset.Token != token || PasswordReset.Expiry < DateTime.UtcNow)
        {
            return Result.Failure(new Error("User.InvalidResetToken", "Invalid or expired password reset token."));
        }

        PasswordHash = newPasswordHash;
        PasswordReset = null;

        RaiseDomainEvent(new UserPasswordChangedDomainEvent(Id));

        return Result.Success();
    }
}
