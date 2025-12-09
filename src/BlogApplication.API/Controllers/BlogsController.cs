using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using BlogApplication.Application.Features.Blogs.Commands;
using BlogApplication.Application.Features.Blogs.Queries;
using BlogApplication.Application.DTOs.Blog;

namespace BlogApplication.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BlogsController : ControllerBase
{
    private readonly IMediator _mediator;

    public BlogsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetBlogs([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, 
        [FromQuery] int? userId = null, [FromQuery] int? categoryId = null, [FromQuery] string? searchTerm = null)
    {
        var query = new GetBlogsQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            UserId = userId,
            CategoryId = categoryId,
            SearchTerm = searchTerm,
            PublishedOnly = true
        };

        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
            return BadRequest(new { message = result.ErrorMessage });

        return Ok(result.Data);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetBlog(string id)
    {
        var query = new GetBlogByIdQuery { Id = id };
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
            return NotFound(new { message = result.ErrorMessage });

        return Ok(result.Data);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateBlog([FromBody] CreateBlogDto request)
    {
        var command = new CreateBlogCommand
        {
            CategoryId = request.CategoryId,
            Title = request.Title,
            Content = request.Content,
            Summary = request.Summary,
            Tags = request.Tags,
            MediaItems = request.MediaItems,
            IsPublished = request.IsPublished
        };

        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(new { message = result.ErrorMessage });

        return CreatedAtAction(nameof(GetBlog), new { id = result.Data!.Id }, result.Data);
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBlog(string id, [FromBody] UpdateBlogDto request)
    {
        var command = new UpdateBlogCommand
        {
            Id = id,
            Title = request.Title,
            Content = request.Content,
            Summary = request.Summary,
            Tags = request.Tags,
            MediaItems = request.MediaItems,
            IsPublished = request.IsPublished
        };

        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(new { message = result.ErrorMessage });

        return Ok(result.Data);
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBlog(string id)
    {
        var command = new DeleteBlogCommand { Id = id };
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(new { message = result.ErrorMessage });

        return NoContent();
    }

    [Authorize]
    [HttpGet("my-blogs")]
    public async Task<IActionResult> GetMyBlogs([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
            return Unauthorized();

        var query = new GetBlogsQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            UserId = int.Parse(userIdClaim.Value),
            PublishedOnly = false
        };

        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
            return BadRequest(new { message = result.ErrorMessage });

        return Ok(result.Data);
    }
}
