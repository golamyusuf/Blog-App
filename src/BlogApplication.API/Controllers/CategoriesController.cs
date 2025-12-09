using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using BlogApplication.Application.Features.Categories.Commands;
using BlogApplication.Application.Features.Categories.Queries;
using BlogApplication.Application.DTOs.Category;

namespace BlogApplication.API.Controllers;

[ApiController]
[Route("api/[controller]")]
/// <summary>
/// Controller for category management operations.
/// </summary>
public class CategoriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CategoriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Retrieves all categories, optionally filtered to active ones only.
    /// </summary>
    /// <param name="activeOnly">If true, returns only active categories (default: true).</param>
    /// <returns>A list of categories.</returns>
    [HttpGet]
    public async Task<IActionResult> GetCategories([FromQuery] bool activeOnly = true)
    {
        var query = new GetCategoriesQuery { ActiveOnly = activeOnly };
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
            return BadRequest(new { message = result.ErrorMessage });

        return Ok(result.Data);
    }

    /// <summary>
    /// Creates a new category. Requires authentication.
    /// </summary>
    /// <param name="request">The category details (name and description).</param>
    /// <returns>The created category with auto-generated slug.</returns>
    /// <response code="201">Category created successfully.</response>
    /// <response code="400">Invalid input or category already exists.</response>
    /// <response code="401">User not authenticated.</response>
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDto request)
    {
        var command = new CreateCategoryCommand
        {
            Name = request.Name,
            Description = request.Description
        };

        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(new { message = result.ErrorMessage });

        return CreatedAtAction(nameof(GetCategories), new { id = result.Data!.Id }, result.Data);
    }
}
