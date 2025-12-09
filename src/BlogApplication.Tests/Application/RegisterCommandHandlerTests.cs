using Xunit;
using Moq;
using FluentAssertions;
using BlogApplication.Domain.Entities;
using BlogApplication.Domain.Interfaces;
using BlogApplication.Application.Features.Auth.Commands;
using BlogApplication.Application.Interfaces;

namespace BlogApplication.Tests.Application;

public class RegisterCommandHandlerTests
{
    private readonly Mock<IAuthService> _authServiceMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly RegisterCommandHandler _handler;

    public RegisterCommandHandlerTests()
    {
        _authServiceMock = new Mock<IAuthService>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _handler = new RegisterCommandHandler(_authServiceMock.Object, _userRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldRegisterUser()
    {
        // Arrange
        var command = new RegisterCommand
        {
            Username = "testuser",
            Email = "test@test.com",
            Password = "Password123",
            FirstName = "Test",
            LastName = "User"
        };

        var user = new User
        {
            Id = 1,
            Username = command.Username,
            Email = command.Email,
            FirstName = command.FirstName,
            LastName = command.LastName,
            CreatedAt = DateTime.UtcNow,
            UserRoles = new List<UserRole>
            {
                new UserRole
                {
                    Role = new Role { Name = "User" }
                }
            }
        };

        _userRepositoryMock.Setup(x => x.ExistsAsync(command.Email, command.Username))
            .ReturnsAsync(false);
        _authServiceMock.Setup(x => x.RegisterAsync(command.Username, command.Email, command.Password, 
            command.FirstName, command.LastName))
            .ReturnsAsync(user.Id);
        _userRepositoryMock.Setup(x => x.GetByIdAsync(user.Id)).ReturnsAsync(user);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Username.Should().Be(command.Username);
        result.Data.Email.Should().Be(command.Email);
    }

    [Fact]
    public async Task Handle_ExistingUser_ShouldReturnFailure()
    {
        // Arrange
        var command = new RegisterCommand
        {
            Username = "existinguser",
            Email = "existing@test.com",
            Password = "Password123"
        };

        _userRepositoryMock.Setup(x => x.ExistsAsync(command.Email, command.Username))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Contain("already exists");
        
        _authServiceMock.Verify(x => x.RegisterAsync(It.IsAny<string>(), It.IsAny<string>(), 
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }
}
