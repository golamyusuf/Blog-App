using MediatR;
using BlogApplication.Application.DTOs.Blog;
using BlogApplication.Application.Common;
using BlogApplication.Application.Interfaces;
using BlogApplication.Domain.Interfaces;
using BlogApplication.Domain.Entities;

namespace BlogApplication.Application.Features.Blogs.Commands;

public class UpdateBlogCommandHandler : IRequestHandler<UpdateBlogCommand, Result<BlogDto>>
{
    private readonly IBlogRepository _blogRepository;
    private readonly ICurrentUserService _currentUserService;

    public UpdateBlogCommandHandler(IBlogRepository blogRepository, ICurrentUserService currentUserService)
    {
        _blogRepository = blogRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result<BlogDto>> Handle(UpdateBlogCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var blog = await _blogRepository.GetByIdAsync(request.Id);
            if (blog == null)
                return Result<BlogDto>.Failure("Blog not found");

            // Only the owner can update the blog
            if (blog.UserId != _currentUserService.UserId)
                return Result<BlogDto>.Failure("You are not authorized to update this blog");

            blog.Title = request.Title;
            blog.Content = request.Content;
            blog.Summary = request.Summary;
            blog.Tags = request.Tags;
            blog.MediaItems = request.MediaItems.Select(m => new MediaItem
            {
                Url = m.Url,
                Type = m.Type,
                Caption = m.Caption,
                Order = m.Order
            }).ToList();
            
            var wasPublished = blog.IsPublished;
            blog.IsPublished = request.IsPublished;
            blog.UpdatedAt = DateTime.UtcNow;

            // Set PublishedAt if publishing for the first time
            if (!wasPublished && request.IsPublished)
                blog.PublishedAt = DateTime.UtcNow;

            await _blogRepository.UpdateAsync(blog);

            var blogDto = MapToBlogDto(blog);
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
