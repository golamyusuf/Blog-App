using Xunit;
using FluentAssertions;
using BlogApplication.Application.Features.Auth.Validators;
using BlogApplication.Application.Features.Auth.Commands;

namespace BlogApplication.Tests.Validators;

public class RegisterCommandValidatorTests
{
    private readonly RegisterCommandValidator _validator;

    public RegisterCommandValidatorTests()
    {
        _validator = new RegisterCommandValidator();
    }

    [Fact]
    public void Validate_ValidCommand_ShouldPass()
    {
        // Arrange
        var command = new RegisterCommand
        {
            Username = "testuser",
            Email = "test@test.com",
            Password = "Password123"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("", "Must be at least 3 characters")]
    [InlineData("ab", "Must be at least 3 characters")]
    public void Validate_InvalidUsername_ShouldFail(string username, string expectedError)
    {
        // Arrange
        var command = new RegisterCommand
        {
            Username = username,
            Email = "test@test.com",
            Password = "Password123"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains("3 characters"));
    }

    [Fact]
    public void Validate_InvalidEmail_ShouldFail()
    {
        // Arrange
        var command = new RegisterCommand
        {
            Username = "testuser",
            Email = "invalid-email",
            Password = "Password123"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains("email"));
    }

    [Theory]
    [InlineData("short", "at least 6 characters")]
    [InlineData("nouppercase123", "uppercase letter")]
    [InlineData("NOLOWERCASE123", "lowercase letter")]
    [InlineData("NoNumbers", "number")]
    public void Validate_WeakPassword_ShouldFail(string password, string expectedError)
    {
        // Arrange
        var command = new RegisterCommand
        {
            Username = "testuser",
            Email = "test@test.com",
            Password = password
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage.ToLower().Contains(expectedError.ToLower().Split(' ')[0]));
    }
}
