using MediatR;
using BlogApplication.Application.DTOs.Category;
using BlogApplication.Application.Common;

namespace BlogApplication.Application.Features.Categories.Commands;

public class CreateCategoryCommand : IRequest<Result<CategoryDto>>
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}
