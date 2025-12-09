using MediatR;
using BlogApplication.Application.DTOs.Auth;
using BlogApplication.Application.Common;

namespace BlogApplication.Application.Features.Auth.Commands;

/// <summary>
/// Command for user authentication.
/// </summary>
public class LoginCommand : IRequest<Result<LoginResponse>>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
