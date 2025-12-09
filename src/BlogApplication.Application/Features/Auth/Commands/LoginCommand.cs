using MediatR;
using BlogApplication.Application.DTOs.Auth;
using BlogApplication.Application.Common;

namespace BlogApplication.Application.Features.Auth.Commands;

public class LoginCommand : IRequest<Result<LoginResponseDto>>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
