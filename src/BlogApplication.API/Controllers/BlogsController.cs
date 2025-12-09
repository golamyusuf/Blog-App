using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using BlogApplication.Application.Features.Blogs.Commands;
using BlogApplication.Application.Features.Blogs.Queries;
using BlogApplication.Application.DTOs.Blog;

namespace BlogApplication.API.Controllers;

[ApiController]
[Route("api/[controller]")]
/// <summary>
/// Controller for blog post operations including CRUD operations and searching.
/// </summary>
public class BlogsController : ControllerBase
{
    private readonly IMediator _mediator;

    public BlogsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Retrieves a paginated list of blog posts with optional filtering.
    /// </summary>
    /// <param name="pageNumber">The page number (default: 1).</param>
    /// <param name="pageSize">The number of items per page (default: 10).</param>
    /// <param name="userId">Optional filter by user ID.</param>
    /// <param name="categoryId">Optional filter by category ID.</param>
    /// <param name="searchTerm">Optional search term for title/content.</param>
    /// <returns>A paginated list of blog posts.</returns>
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

    /// <summary>
    /// Retrieves a single blog post by its ID and increments view count.
    /// </summary>
    /// <param name="id">The blog post ID.</param>
    /// <returns>The blog post details.</returns>
    /// <response code="200">Blog post found.</response>
    /// <response code="404">Blog post not found.</response>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetBlog(string id)
    {
        var query = new GetBlogByIdQuery { Id = id };
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
            return NotFound(new { message = result.ErrorMessage });

        return Ok(result.Data);
    }

    /// <summary>
    /// Creates a new blog post. Requires authentication.
    /// </summary>
    /// <param name="request">The blog post details.</param>
    /// <returns>The created blog post.</returns>
    /// <response code="201">Blog post created successfully.</response>
    /// <response code="400">Invalid input.</response>
    /// <response code="401">User not authenticated.</response>
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
