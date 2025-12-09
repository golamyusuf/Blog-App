using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using BlogApplication.Application.Features.Blogs.Commands;

namespace BlogApplication.API.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpDelete("blogs/{id}")]
    public async Task<IActionResult> DeleteBlog(string id)
    {
        var command = new DeleteBlogCommand { Id = id };
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(new { message = result.ErrorMessage });

        return NoContent();
    }
}
