using MediatR;
using BlogApplication.Application.DTOs.Blog;
using BlogApplication.Application.Common;

namespace BlogApplication.Application.Features.Blogs.Queries;

public class GetBlogsQuery : IRequest<Result<BlogListDto>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int? UserId { get; set; }
    public string? SearchTerm { get; set; }
    public bool PublishedOnly { get; set; } = true;
}
