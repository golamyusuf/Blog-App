using MediatR;
using BlogApplication.Application.DTOs.Blog;
using BlogApplication.Application.Common;

namespace BlogApplication.Application.Features.Blogs.Queries;

public class GetBlogByIdQuery : IRequest<Result<BlogDto>>
{
    public string Id { get; set; } = string.Empty;
}
