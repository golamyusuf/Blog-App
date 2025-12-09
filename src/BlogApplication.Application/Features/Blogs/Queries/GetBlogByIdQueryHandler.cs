using MediatR;
using BlogApplication.Application.DTOs.Blog;
using BlogApplication.Application.Common;
using BlogApplication.Domain.Interfaces;

namespace BlogApplication.Application.Features.Blogs.Queries;

public class GetBlogByIdQueryHandler : IRequestHandler<GetBlogByIdQuery, Result<BlogDto>>
{
    private readonly IBlogRepository _blogRepository;

    public GetBlogByIdQueryHandler(IBlogRepository blogRepository)
    {
        _blogRepository = blogRepository;
    }

    public async Task<Result<BlogDto>> Handle(GetBlogByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var blog = await _blogRepository.GetByIdAsync(request.Id);
            if (blog == null)
                return Result<BlogDto>.Failure("Blog not found");

            // Increment view count
            await _blogRepository.IncrementViewCountAsync(request.Id);
            blog.ViewCount++;

            var blogDto = new BlogDto
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

            return Result<BlogDto>.Success(blogDto);
        }
        catch (Exception ex)
        {
            return Result<BlogDto>.Failure(ex.Message);
        }
    }
}
