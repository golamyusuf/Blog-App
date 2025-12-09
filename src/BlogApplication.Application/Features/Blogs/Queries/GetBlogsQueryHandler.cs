using MediatR;
using BlogApplication.Application.DTOs.Blog;
using BlogApplication.Application.Common;
using BlogApplication.Domain.Interfaces;

namespace BlogApplication.Application.Features.Blogs.Queries;

public class GetBlogsQueryHandler : IRequestHandler<GetBlogsQuery, Result<BlogListDto>>
{
    private readonly IBlogRepository _blogRepository;

    public GetBlogsQueryHandler(IBlogRepository blogRepository)
    {
        _blogRepository = blogRepository;
    }

    public async Task<Result<BlogListDto>> Handle(GetBlogsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            IEnumerable<Domain.Entities.Blog> blogs;
            long totalCount;

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                blogs = await _blogRepository.SearchAsync(request.SearchTerm, request.PageNumber, request.PageSize);
                totalCount = blogs.Count(); // Approximation for search
            }
            else if (request.UserId.HasValue)
            {
                blogs = await _blogRepository.GetByUserIdAsync(request.UserId.Value, request.PageNumber, request.PageSize);
                totalCount = await _blogRepository.GetUserBlogsCountAsync(request.UserId.Value);
            }
            else if (request.PublishedOnly)
            {
                blogs = await _blogRepository.GetPublishedAsync(request.PageNumber, request.PageSize);
                totalCount = await _blogRepository.GetTotalCountAsync();
            }
            else
            {
                blogs = await _blogRepository.GetAllAsync(request.PageNumber, request.PageSize);
                totalCount = await _blogRepository.GetTotalCountAsync();
            }

            var blogDtos = blogs.Select(blog => new BlogDto
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
            }).ToList();

            var result = new BlogListDto
            {
                Blogs = blogDtos,
                TotalCount = (int)totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };

            return Result<BlogListDto>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<BlogListDto>.Failure(ex.Message);
        }
    }
}
