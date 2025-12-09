using Microsoft.AspNetCore.Mvc;
using MediatR;
using BlogApplication.Application.Features.Auth.Commands;
using BlogApplication.Application.DTOs.Auth;

namespace BlogApplication.API.Controllers;

[ApiController]
[Route("api/[controller]")]
/// <summary>
/// Controller for authentication operations including user registration and login.
/// </summary>
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Registers a new user account.
    /// </summary>
    /// <param name="request">The registration details including username, email, and password.</param>
    /// <returns>The created user information.</returns>
    /// <response code="200">User successfully registered.</response>
    /// <response code="400">Invalid input or user already exists.</response>
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

    /// <summary>
    /// Authenticates a user and returns a JWT token.
    /// </summary>
    /// <param name="request">The login credentials (email and password).</param>
    /// <returns>JWT token and user information on successful authentication.</returns>
    /// <response code="200">Login successful, returns token.</response>
    /// <response code="401">Invalid credentials.</response>
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
