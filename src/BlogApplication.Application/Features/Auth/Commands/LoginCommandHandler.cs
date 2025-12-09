using MediatR;
using BlogApplication.Application.DTOs.Auth;
using BlogApplication.Application.Common;
using BlogApplication.Application.Interfaces;
using BlogApplication.Domain.Interfaces;

namespace BlogApplication.Application.Features.Auth.Commands;

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<LoginResponseDto>>
{
    private readonly IAuthService _authService;
    private readonly IUserRepository _userRepository;

    public LoginCommandHandler(IAuthService authService, IUserRepository userRepository)
    {
        _authService = authService;
        _userRepository = userRepository;
    }

    public async Task<Result<LoginResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var (token, userId, roles) = await _authService.AuthenticateAsync(request.Email, request.Password);
            
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return Result<LoginResponseDto>.Failure("User not found");

            var userDto = new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                ProfileImageUrl = user.ProfileImageUrl,
                Roles = roles,
                CreatedAt = user.CreatedAt
            };

            var response = new LoginResponseDto
            {
                Token = token,
                User = userDto
            };

            return Result<LoginResponseDto>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<LoginResponseDto>.Failure(ex.Message);
        }
    }
}
