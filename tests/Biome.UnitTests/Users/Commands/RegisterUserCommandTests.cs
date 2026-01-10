using Biome.Application.Users.Commands.RegisterUser;
using FluentAssertions;
using Xunit;

namespace Biome.UnitTests.Users.Commands;

public class RegisterUserCommandTests
{
    [Fact]
    public void Should_Create_Valid_RegisterUserCommand()
    {
        // Arrange
        var email = "test@example.com";
        var password = "Password123!";
        var firstName = "John";
        var lastName = "Doe";
        
        // Act
        var command = new RegisterUserCommand(email, password, firstName, lastName);
        
        // Assert
        command.Should().NotBeNull();
        command.Email.Should().Be(email);
        command.Password.Should().Be(password);
        command.FirstName.Should().Be(firstName);
        command.LastName.Should().Be(lastName);
    }

    [Theory]
    [InlineData("")]
    [InlineData("invalid-email")]
    [InlineData("@example.com")]
    [InlineData("test@")]
    [InlineData("test.example.com")]
    public void Should_Fail_Validation_When_Email_Is_Invalid(string invalidEmail)
    {
        // Arrange
        var command = new RegisterUserCommand(invalidEmail, "Password123!", "John", "Doe");
        var validator = new RegisterUserCommandValidator();
        
        // Act
        var result = validator.Validate(command);
        
        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(RegisterUserCommand.Email));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("123")] // Too short
    [InlineData("password")] // No uppercase, no number
    [InlineData("PASSWORD")] // No lowercase, no number
    [InlineData("Password")] // No number
    [InlineData("Password123")] // Missing special character
    public void Should_Fail_Validation_When_Password_Is_Invalid(string invalidPassword)
    {
        // Arrange
        var command = new RegisterUserCommand("test@example.com", invalidPassword, "John", "Doe");
        var validator = new RegisterUserCommandValidator();
        
        // Act
        var result = validator.Validate(command);
        
        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(RegisterUserCommand.Password));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Should_Fail_Validation_When_FirstName_Is_Invalid(string? invalidFirstName)
    {
        // Arrange
        var command = new RegisterUserCommand("test@example.com", "Password123!", invalidFirstName, "Doe");
        var validator = new RegisterUserCommandValidator();
        
        // Act
        var result = validator.Validate(command);
        
        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(RegisterUserCommand.FirstName));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Should_Fail_Validation_When_LastName_Is_Invalid(string? invalidLastName)
    {
        // Arrange
        var command = new RegisterUserCommand("test@example.com", "Password123!", "John", invalidLastName);
        var validator = new RegisterUserCommandValidator();
        
        // Act
        var result = validator.Validate(command);
        
        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(RegisterUserCommand.LastName));
    }

    [Fact]
    public void Should_Pass_Validation_With_Valid_Data()
    {
        // Arrange
        var command = new RegisterUserCommand("test@example.com", "Password123!", "John", "Doe");
        var validator = new RegisterUserCommandValidator();
        
        // Act
        var result = validator.Validate(command);
        
        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Should_Fail_Validation_When_FirstName_Is_Too_Long()
    {
        // Arrange
        var longFirstName = new string('A', 101); // Assuming max length is 100
        var command = new RegisterUserCommand("test@example.com", "Password123!", longFirstName, "Doe");
        var validator = new RegisterUserCommandValidator();
        
        // Act
        var result = validator.Validate(command);
        
        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(RegisterUserCommand.FirstName));
    }

    [Fact]
    public void Should_Fail_Validation_When_LastName_Is_Too_Long()
    {
        // Arrange
        var longLastName = new string('A', 101); // Assuming max length is 100
        var command = new RegisterUserCommand("test@example.com", "Password123!", "John", longLastName);
        var validator = new RegisterUserCommandValidator();
        
        // Act
        var result = validator.Validate(command);
        
        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(RegisterUserCommand.LastName));
    }

    [Fact]
    public void Should_Fail_Validation_With_Multiple_Invalid_Fields()
    {
        // Arrange
        var command = new RegisterUserCommand("invalid-email", "weak", "", "");
        var validator = new RegisterUserCommandValidator();
        
        // Act
        var result = validator.Validate(command);
        
        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(RegisterUserCommand.Email));
        result.Errors.Should().Contain(e => e.PropertyName == nameof(RegisterUserCommand.Password));
        result.Errors.Should().Contain(e => e.PropertyName == nameof(RegisterUserCommand.FirstName));
        result.Errors.Should().Contain(e => e.PropertyName == nameof(RegisterUserCommand.LastName));
    }

    [Theory]
    [InlineData("john.doe@example.com", "Password123!", "John", "Doe")]
    [InlineData("user+tag@domain.co.uk", "SecurePass456!", "Jane", "Smith")]
    [InlineData("test.email+alias@subdomain.example.com", "Complex!@Pass789", "Alice", "Johnson")]
    public void Should_Pass_Validation_With_Various_Valid_Data(string email, string password, string firstName, string lastName)
    {
        // Arrange
        var command = new RegisterUserCommand(email, password, firstName, lastName);
        var validator = new RegisterUserCommandValidator();
        
        // Act
        var result = validator.Validate(command);
        
        // Assert
        result.IsValid.Should().BeTrue();
    }
}
