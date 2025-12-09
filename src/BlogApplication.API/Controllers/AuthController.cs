using Microsoft.AspNetCore.Mvc;
using MediatR;
using BlogApplication.Application.Features.Auth.Commands;
using BlogApplication.Application.DTOs.Auth;

namespace BlogApplication.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
    {
        var command = new RegisterCommand
        {
            Username = request.Username,
            Email = request.Email,
            Password = request.Password,
            FirstName = request.FirstName,
            LastName = request.LastName
        };

        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(new { message = result.ErrorMessage, errors = result.Errors });

        return Ok(result.Data);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        var command = new LoginCommand
        {
            Email = request.Email,
            Password = request.Password
        };

        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return Unauthorized(new { message = result.ErrorMessage, errors = result.Errors });

        return Ok(result.Data);
    }
}
