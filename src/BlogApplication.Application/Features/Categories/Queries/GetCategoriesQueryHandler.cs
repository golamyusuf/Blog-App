using MediatR;
using BlogApplication.Application.DTOs.Category;
using BlogApplication.Application.Common;
using BlogApplication.Domain.Interfaces;

namespace BlogApplication.Application.Features.Categories.Queries;

public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, Result<List<CategoryDto>>>
{
    private readonly ICategoryRepository _categoryRepository;

    public GetCategoriesQueryHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<Result<List<CategoryDto>>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var categories = request.ActiveOnly
                ? await _categoryRepository.GetActiveAsync()
                : await _categoryRepository.GetAllAsync();

            var categoryDtos = categories.Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                Slug = c.Slug,
                CreatedByUserId = c.CreatedByUserId,
                IsActive = c.IsActive,
                CreatedAt = c.CreatedAt
            }).ToList();

            return Result<List<CategoryDto>>.Success(categoryDtos);
        }
        catch (Exception ex)
        {
            return Result<List<CategoryDto>>.Failure($"Failed to retrieve categories: {ex.Message}");
        }
    }
}
