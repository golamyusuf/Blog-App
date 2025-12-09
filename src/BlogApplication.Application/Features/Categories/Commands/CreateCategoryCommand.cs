using MediatR;
using BlogApplication.Application.DTOs.Category;
using BlogApplication.Application.Common;

namespace BlogApplication.Application.Features.Categories.Commands;

/// <summary>
/// Command for creating a new blog category.
/// </summary>
public class CreateCategoryCommand : IRequest<Result<CategoryDto>>
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}
