using MediatR;
using BlogApplication.Application.DTOs.Blog;
using BlogApplication.Application.Common;

namespace BlogApplication.Application.Features.Blogs.Commands;

public class CreateBlogCommand : IRequest<Result<BlogDto>>
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? Summary { get; set; }
    public List<string> Tags { get; set; } = new();
    public List<MediaItemDto> MediaItems { get; set; } = new();
    public bool IsPublished { get; set; }
}
