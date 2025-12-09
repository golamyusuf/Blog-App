using Xunit;
using Moq;
using FluentAssertions;
using BlogApplication.Domain.Entities;
using BlogApplication.Domain.Interfaces;
using BlogApplication.Application.Features.Blogs.Commands;
using BlogApplication.Application.Interfaces;

namespace BlogApplication.Tests.Application;

public class CreateBlogCommandHandlerTests
{
    private readonly Mock<IBlogRepository> _blogRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly CreateBlogCommandHandler _handler;

    public CreateBlogCommandHandlerTests()
    {
        _blogRepositoryMock = new Mock<IBlogRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        
        _handler = new CreateBlogCommandHandler(
            _blogRepositoryMock.Object,
            _currentUserServiceMock.Object,
            _userRepositoryMock.Object
        );
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldCreateBlog()
    {
        // Arrange
        var userId = 1;
        var user = new User
        {
            Id = userId,
            Username = "testuser",
            Email = "test@test.com",
            PasswordHash = "hash",
            CreatedAt = DateTime.UtcNow
        };

        _currentUserServiceMock.Setup(x => x.UserId).Returns(userId);
        _userRepositoryMock.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(user);
        _blogRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<Blog>()))
            .ReturnsAsync((Blog b) => { b.Id = "123"; return b; });

        var command = new CreateBlogCommand
        {
            Title = "Test Blog",
            Content = "This is a test blog content with enough characters to pass validation.",
            Summary = "Test summary",
            Tags = new List<string> { "test", "blog" },
            IsPublished = true
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Title.Should().Be(command.Title);
        result.Data.Content.Should().Be(command.Content);
        result.Data.UserId.Should().Be(userId);
        result.Data.Username.Should().Be(user.Username);
        
        _blogRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<Blog>()), Times.Once);
    }

    [Fact]
    public async Task Handle_UserNotFound_ShouldReturnFailure()
    {
        // Arrange
        var userId = 1;
        _currentUserServiceMock.Setup(x => x.UserId).Returns(userId);
        _userRepositoryMock.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync((User?)null);

        var command = new CreateBlogCommand
        {
            Title = "Test Blog",
            Content = "This is a test blog content with enough characters to pass validation.",
            IsPublished = true
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Be("User not found");
        
        _blogRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<Blog>()), Times.Never);
    }
}
