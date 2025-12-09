using MediatR;
using BlogApplication.Application.DTOs.Blog;
using BlogApplication.Application.Common;

namespace BlogApplication.Application.Features.Blogs.Queries;

/// <summary>
/// Query for retrieving a single blog post by its unique identifier.
/// </summary>
public class GetBlogByIdQuery : IRequest<Result<BlogDto>>
{
    public string Id { get; set; } = string.Empty;
}
