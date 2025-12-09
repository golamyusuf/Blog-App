using MediatR;
using BlogApplication.Application.DTOs.Category;
using BlogApplication.Application.Common;

namespace BlogApplication.Application.Features.Categories.Queries;

public class GetCategoriesQuery : IRequest<Result<List<CategoryDto>>>
{
    public bool ActiveOnly { get; set; } = true;
}
