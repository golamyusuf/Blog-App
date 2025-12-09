using MediatR;
using BlogApplication.Application.Common;

namespace BlogApplication.Application.Features.Blogs.Commands;

public class DeleteBlogCommand : IRequest<Result<bool>>
{
    public string Id { get; set; } = string.Empty;
}
