using MediatR;
using BlogApplication.Application.DTOs.Auth;
using BlogApplication.Application.Common;

namespace BlogApplication.Application.Features.Auth.Commands;

public class RegisterCommand : IRequest<Result<UserDto>>
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}
