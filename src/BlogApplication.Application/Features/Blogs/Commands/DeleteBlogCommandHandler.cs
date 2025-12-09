using MediatR;
using BlogApplication.Application.Common;
using BlogApplication.Application.Interfaces;
using BlogApplication.Domain.Interfaces;

namespace BlogApplication.Application.Features.Blogs.Commands;

public class DeleteBlogCommandHandler : IRequestHandler<DeleteBlogCommand, Result<bool>>
{
    private readonly IBlogRepository _blogRepository;
    private readonly ICurrentUserService _currentUserService;

    public DeleteBlogCommandHandler(IBlogRepository blogRepository, ICurrentUserService currentUserService)
    {
        _blogRepository = blogRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result<bool>> Handle(DeleteBlogCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var blog = await _blogRepository.GetByIdAsync(request.Id);
            if (blog == null)
                return Result<bool>.Failure("Blog not found");

            // Only the owner or admin can delete the blog
            if (blog.UserId != _currentUserService.UserId && !_currentUserService.IsAdmin)
                return Result<bool>.Failure("You are not authorized to delete this blog");

            await _blogRepository.DeleteAsync(request.Id);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure(ex.Message);
        }
    }
}
