using MediatR;
using BlogApplication.Application.DTOs.Blog;
using BlogApplication.Application.Common;
using BlogApplication.Application.Interfaces;
using BlogApplication.Domain.Interfaces;
using BlogApplication.Domain.Entities;

namespace BlogApplication.Application.Features.Blogs.Commands;

public class CreateBlogCommandHandler : IRequestHandler<CreateBlogCommand, Result<BlogDto>>
{
    private readonly IBlogRepository _blogRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUserRepository _userRepository;
    private readonly ICategoryRepository _categoryRepository;

    public CreateBlogCommandHandler(
        IBlogRepository blogRepository,
        ICurrentUserService currentUserService,
        IUserRepository userRepository,
        ICategoryRepository categoryRepository)
    {
        _blogRepository = blogRepository;
        _currentUserService = currentUserService;
        _userRepository = userRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<Result<BlogDto>> Handle(CreateBlogCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(_currentUserService.UserId);
            if (user == null)
                return Result<BlogDto>.Failure("User not found");

            string? categoryName = null;
            if (request.CategoryId.HasValue)
            {
                var category = await _categoryRepository.GetByIdAsync(request.CategoryId.Value);
                if (category == null)
                    return Result<BlogDto>.Failure("Category not found");
                categoryName = category.Name;
            }

            var blog = new Blog
            {
                UserId = _currentUserService.UserId,
                Username = user.Username,
                CategoryId = request.CategoryId,
                CategoryName = categoryName,
                Title = request.Title,
                Content = request.Content,
                Summary = request.Summary,
                Tags = request.Tags,
                MediaItems = request.MediaItems.Select(m => new MediaItem
                {
                    Url = m.Url,
                    Type = m.Type,
                    Caption = m.Caption,
                    Order = m.Order
                }).ToList(),
                IsPublished = request.IsPublished,
                CreatedAt = DateTime.UtcNow,
                PublishedAt = request.IsPublished ? DateTime.UtcNow : null
            };

            var createdBlog = await _blogRepository.CreateAsync(blog);

            var blogDto = MapToBlogDto(createdBlog);
            return Result<BlogDto>.Success(blogDto);
        }
        catch (Exception ex)
        {
            return Result<BlogDto>.Failure(ex.Message);
        }
    }

    private static BlogDto MapToBlogDto(Blog blog)
    {
        return new BlogDto
        {
            Id = blog.Id,
            UserId = blog.UserId,
            Username = blog.Username,
            CategoryId = blog.CategoryId,
            CategoryName = blog.CategoryName,
            Title = blog.Title,
            Content = blog.Content,
            Summary = blog.Summary,
            Tags = blog.Tags,
            MediaItems = blog.MediaItems.Select(m => new MediaItemDto
            {
                Url = m.Url,
                Type = m.Type,
                Caption = m.Caption,
                Order = m.Order
            }).ToList(),
            ViewCount = blog.ViewCount,
            IsPublished = blog.IsPublished,
            CreatedAt = blog.CreatedAt,
            UpdatedAt = blog.UpdatedAt,
            PublishedAt = blog.PublishedAt
        };
    }
}
