using MediatR;
using BlogApplication.Application.Common;

namespace BlogApplication.Application.Features.Blogs.Commands;

/// <summary>
/// Command for deleting a blog post.
/// </summary>
public class DeleteBlogCommand : IRequest<Result<bool>>
{
    public string Id { get; set; } = string.Empty;
}
