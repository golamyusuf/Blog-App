using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using BlogApplication.Application.Features.Categories.Commands;
using BlogApplication.Application.Features.Categories.Queries;
using BlogApplication.Application.DTOs.Category;

namespace BlogApplication.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CategoriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetCategories([FromQuery] bool activeOnly = true)
    {
        var query = new GetCategoriesQuery { ActiveOnly = activeOnly };
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
            return BadRequest(new { message = result.ErrorMessage });

        return Ok(result.Data);
    }

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
