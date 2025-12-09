using MediatR;
using BlogApplication.Application.DTOs.Auth;
using BlogApplication.Application.Common;
using BlogApplication.Application.Interfaces;
using BlogApplication.Domain.Interfaces;

namespace BlogApplication.Application.Features.Auth.Commands;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<UserDto>>
{
    private readonly IAuthService _authService;
    private readonly IUserRepository _userRepository;

    public RegisterCommandHandler(IAuthService authService, IUserRepository userRepository)
    {
        _authService = authService;
        _userRepository = userRepository;
    }

    public async Task<Result<UserDto>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Check if user already exists
            var existingUser = await _userRepository.ExistsAsync(request.Email, request.Username);
            if (existingUser)
                return Result<UserDto>.Failure("User with this email or username already exists");

            var userId = await _authService.RegisterAsync(
                request.Username,
                request.Email,
                request.Password,
                request.FirstName,
                request.LastName
            );

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return Result<UserDto>.Failure("Failed to create user");

            var userDto = new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                ProfileImageUrl = user.ProfileImageUrl,
                Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList(),
                CreatedAt = user.CreatedAt
            };

            return Result<UserDto>.Success(userDto);
        }
        catch (Exception ex)
        {
            return Result<UserDto>.Failure(ex.Message);
        }
    }
}
