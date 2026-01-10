using Biome.Domain.Users;
using Biome.Domain.Users.ValueObjects;
using Biome.SharedKernel.ValueObjects;
using FluentAssertions;
using Biome.SharedKernel.Abstractions;
using NSubstitute;
using Xunit;

namespace Biome.UnitTests.Users.Domain;

public class UserTests
{
    [Fact]
    public void Should_Create_User_With_Valid_Data()
    {
        // Arrange
        var email = Email.Create("test@example.com").Value;
        var firstName = FirstName.Create("John").Value;
        var lastName = LastName.Create("Doe").Value;
        var passwordHash = "hashedpassword";
        var roleId = Guid.NewGuid();
        var temporaryPassword = "temp123";
        
        // Act
        var user = User.Create(firstName, lastName, email, passwordHash, roleId, temporaryPassword);
        
        // Assert
        user.Should().NotBeNull();
        user.Email.Value.Should().Be(email.Value);
        user.FirstName.Value.Should().Be(firstName.Value);
        user.LastName.Value.Should().Be(lastName.Value);
        user.IsEmailVerified.Should().BeFalse();
        user.IsBanned.Should().BeFalse();
        user.TwoFactorEnabled.Should().BeFalse();
    }

    [Fact]
    public void Should_Register_User_With_Valid_Data()
    {
        // Arrange
        var email = Email.Create("test@example.com").Value;
        var firstName = FirstName.Create("John").Value;
        var lastName = LastName.Create("Doe").Value;
        var passwordHash = "hashedpassword";
        var roleId = Guid.NewGuid();
        
        // Act
        var user = User.Register(firstName, lastName, email, passwordHash, roleId);
        
        // Assert
        user.Should().NotBeNull();
        user.Email.Value.Should().Be(email.Value);
        user.FirstName.Value.Should().Be(firstName.Value);
        user.LastName.Value.Should().Be(lastName.Value);
        user.IsEmailVerified.Should().BeFalse();
        user.IsBanned.Should().BeFalse();
        user.TwoFactorEnabled.Should().BeFalse();
    }

    [Fact]
    public void Should_Change_Password_Successfully()
    {
        // Arrange
        var user = CreateTestUser();
        var newPasswordHash = "NewPassword123!";
        
        // Act
        user.ChangePassword(newPasswordHash);
        
        // Assert
        user.PasswordHash.Should().Be(newPasswordHash);
    }

    [Fact]
    public void Should_Not_Change_Password_When_Same()
    {
        // Arrange
        var user = CreateTestUser();
        var currentPasswordHash = user.PasswordHash;
        
        // Act
        user.ChangePassword(currentPasswordHash);
        
        // Assert
        user.PasswordHash.Should().Be(currentPasswordHash);
    }

    [Fact]
    public void Should_Update_Profile_Successfully()
    {
        // Arrange
        var user = CreateTestUser();
        var newFirstName = FirstName.Create("Jane").Value;
        var newLastName = LastName.Create("Smith").Value;
        
        // Act
        user.UpdateProfile(newFirstName, newLastName);
        
        // Assert
        user.FirstName.Value.Should().Be(newFirstName.Value);
        user.LastName.Value.Should().Be(newLastName.Value);
    }

    [Fact]
    public void Should_Verify_Email_Successfully()
    {
        // Arrange
        var user = CreateTestUser();
        
        // Act
        user.VerifyEmail();
        
        // Assert
        user.IsEmailVerified.Should().BeTrue();
    }

    [Fact]
    public void Should_Not_Verify_Email_When_Already_Verified()
    {
        // Arrange
        var user = CreateTestUser();
        user.VerifyEmail();
        
        // Act
        user.VerifyEmail();
        
        // Assert
        user.IsEmailVerified.Should().BeTrue();
    }

    [Fact]
    public void Should_Ban_User_Successfully()
    {
        // Arrange
        var user = CreateTestUser();
        
        // Act
        user.Ban();
        
        // Assert
        user.IsBanned.Should().BeTrue();
    }

    [Fact]
    public void Should_Not_Ban_User_When_Already_Banned()
    {
        // Arrange
        var user = CreateTestUser();
        user.Ban();
        
        // Act
        user.Ban();
        
        // Assert
        user.IsBanned.Should().BeTrue();
    }

    [Fact]
    public void Should_Enable_Two_Factor_Successfully()
    {
        // Arrange
        var user = CreateTestUser();
        var secret = "test-secret";
        var code = "123456";
        var twoFactorService = Substitute.For<ITwoFactorService>();
        twoFactorService.ValidateCode(secret, code).Returns(true);
        
        // Act
        var result = user.EnableTwoFactor(secret, code, twoFactorService);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        user.TwoFactorEnabled.Should().BeTrue();
        user.TwoFactorSecret.Should().Be(secret);
    }

    [Fact]
    public void Should_Not_Enable_Two_Factor_When_Already_Enabled()
    {
        // Arrange
        var user = CreateTestUser();
        var secret = "test-secret";
        var code = "123456";
        var twoFactorService = Substitute.For<ITwoFactorService>();
        twoFactorService.ValidateCode(secret, code).Returns(true);
        
        // Enable first time
        user.EnableTwoFactor(secret, code, twoFactorService);
        
        // Act
        var result = user.EnableTwoFactor(secret, code, twoFactorService);
        
        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("User.TwoFactorAlreadyEnabled");
    }

    [Fact]
    public void Should_Not_Enable_Two_Factor_With_Invalid_Code()
    {
        // Arrange
        var user = CreateTestUser();
        var secret = "test-secret";
        var code = "invalid";
        var twoFactorService = Substitute.For<ITwoFactorService>();
        twoFactorService.ValidateCode(secret, code).Returns(false);
        
        // Act
        var result = user.EnableTwoFactor(secret, code, twoFactorService);
        
        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("User.InvalidTwoFactorCode");
        user.TwoFactorEnabled.Should().BeFalse();
    }

    [Fact]
    public void Should_Verify_Two_Factor_Login_Successfully()
    {
        // Arrange
        var user = CreateTestUser();
        var secret = "test-secret";
        var code = "123456";
        var twoFactorService = Substitute.For<ITwoFactorService>();
        twoFactorService.ValidateCode(secret, code).Returns(true);
        
        // Enable 2FA first
        user.EnableTwoFactor(secret, code, twoFactorService);
        
        // Act
        var result = user.VerifyTwoFactorLogin(code, twoFactorService);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void Should_Set_Refresh_Token_Successfully()
    {
        // Arrange
        var user = CreateTestUser();
        var refreshToken = RefreshToken.Create("test-token", DateTime.UtcNow.AddHours(1));
        
        // Act
        user.SetRefreshToken(refreshToken);
        
        // Assert
        user.RefreshToken.Should().NotBeNull();
        user.RefreshToken!.Token.Should().Be("test-token");
        user.RefreshToken!.ExpiryOnUtc.Should().BeCloseTo(DateTime.UtcNow.AddHours(1), TimeSpan.FromMinutes(1));
    }

    [Fact]
    public void Should_Revoke_Refresh_Token_Successfully()
    {
        // Arrange
        var user = CreateTestUser();
        var refreshToken = RefreshToken.Create("test-token", DateTime.UtcNow.AddHours(1));
        user.SetRefreshToken(refreshToken);
        
        // Act
        user.RevokeRefreshToken();
        
        // Assert
        user.RefreshToken.Should().BeNull();
    }

    [Fact]
    public void Should_Request_Password_Reset_Successfully()
    {
        // Arrange
        var user = CreateTestUser();
        var token = "reset-token";
        var expiry = DateTime.UtcNow.AddHours(1);
        
        // Act
        user.RequestPasswordReset(token, expiry);
        
        // Assert
        user.PasswordReset.Should().NotBeNull();
        user.PasswordReset!.Token.Should().Be(token);
        user.PasswordReset!.Expiry.Should().BeCloseTo(expiry, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Should_Reset_Password_Successfully()
    {
        // Arrange
        var user = CreateTestUser();
        var token = "reset-token";
        var newPasswordHash = "newhashedpassword";
        var expiry = DateTime.UtcNow.AddHours(1);
        
        user.RequestPasswordReset(token, expiry);
        
        // Act
        var result = user.ResetPassword(token, newPasswordHash);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        user.PasswordHash.Should().Be(newPasswordHash);
        user.PasswordReset.Should().BeNull();
    }

    [Fact]
    public void Should_Not_Reset_Password_With_Invalid_Token()
    {
        // Arrange
        var user = CreateTestUser();
        var invalidToken = "invalid-token";
        var newPasswordHash = "newhashedpassword";
        
        // Act
        var result = user.ResetPassword(invalidToken, newPasswordHash);
        
        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("User.InvalidResetToken");
        user.PasswordHash.Should().NotBe(newPasswordHash);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Should_Fail_First_Name_Creation_When_Invalid(string? invalidFirstName)
    {
        // Act
        var result = FirstName.Create(invalidFirstName);
        
        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Should_Fail_Last_Name_Creation_When_Invalid(string? invalidLastName)
    {
        // Act
        var result = LastName.Create(invalidLastName);
        
        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Should_Assign_Role_Successfully()
    {
        // Arrange
        var user = CreateTestUser();
        var newRoleId = Guid.NewGuid();
        
        // Act
        user.AssignRole(newRoleId);
        
        // Assert
        user.RoleId.Should().Be(newRoleId);
    }

    [Fact]
    public void Should_Ensure_Login_Eligibility_Successfully()
    {
        // Arrange
        var user = CreateTestUser();
        
        // Act
        var result = user.EnsureLoginEligibility();
        
        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void Should_Not_Allow_Login_When_Banned()
    {
        // Arrange
        var user = CreateTestUser();
        user.Ban();
        
        // Act
        var result = user.EnsureLoginEligibility();
        
        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("User.Banned");
    }

    private static User CreateTestUser()
    {
        var email = Email.Create("test@example.com").Value;
        var firstName = FirstName.Create("John").Value;
        var lastName = LastName.Create("Doe").Value;
        var passwordHash = "hashedpassword";
        var roleId = Guid.NewGuid();
        var temporaryPassword = "temp123";
        
        return User.Create(firstName, lastName, email, passwordHash, roleId, temporaryPassword);
    }
}
