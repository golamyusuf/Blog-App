using MediatR;
using BlogApplication.Application.DTOs.Category;
using BlogApplication.Application.Common;
using BlogApplication.Domain.Interfaces;
using BlogApplication.Application.Interfaces;

namespace BlogApplication.Application.Features.Categories.Commands;

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Result<CategoryDto>>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCategoryCommandHandler(
        ICategoryRepository categoryRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CategoryDto>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Check if category already exists
            if (await _categoryRepository.ExistsByNameAsync(request.Name))
            {
                return Result<CategoryDto>.Failure("A category with this name already exists");
            }

            var userId = _currentUserService.UserId;
            if (userId == 0 || !_currentUserService.IsAuthenticated)
            {
                return Result<CategoryDto>.Failure("User not authenticated");
            }

            // Generate slug from name
            var slug = GenerateSlug(request.Name);

            var category = new Domain.Entities.Category
            {
                Name = request.Name,
                Description = request.Description,
                Slug = slug,
                CreatedByUserId = userId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _categoryRepository.CreateAsync(category);
            await _unitOfWork.SaveChangesAsync();

            var categoryDto = new CategoryDto
            {
                Id = created.Id,
                Name = created.Name,
                Description = created.Description,
                Slug = created.Slug,
                CreatedByUserId = created.CreatedByUserId,
                IsActive = created.IsActive,
                CreatedAt = created.CreatedAt
            };

            return Result<CategoryDto>.Success(categoryDto);
        }
        catch (Exception ex)
        {
            return Result<CategoryDto>.Failure($"Failed to create category: {ex.Message}");
        }
    }

    private string GenerateSlug(string name)
    {
        return name.ToLower()
            .Replace(" ", "-")
            .Replace("&", "and")
            .Replace("'", "")
            .Replace("\"", "");
    }
}
