using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using BlogApplication.Application.Interfaces;
using BlogApplication.Domain.Entities;
using BlogApplication.Domain.Interfaces;
using BlogApplication.Infrastructure.Configuration;

namespace BlogApplication.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly JwtSettings _jwtSettings;

    public AuthService(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IPasswordHasher passwordHasher,
        IOptions<JwtSettings> jwtSettings)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _passwordHasher = passwordHasher;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<(string token, int userId, List<string> roles)> AuthenticateAsync(string email, string password)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        
        if (user == null || !user.IsActive)
            throw new UnauthorizedAccessException("Invalid credentials");

        if (!_passwordHasher.VerifyPassword(password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid credentials");

        // Update last login
        user.LastLoginAt = DateTime.UtcNow;
        await _userRepository.UpdateAsync(user);

        var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();
        var token = GenerateJwtToken(user.Id, user.Email, roles);

        return (token, user.Id, roles);
    }

    public async Task<int> RegisterAsync(string username, string email, string password, string? firstName, string? lastName)
    {
        var passwordHash = _passwordHasher.HashPassword(password);

        var user = new User
        {
            Username = username,
            Email = email,
            PasswordHash = passwordHash,
            FirstName = firstName,
            LastName = lastName,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var createdUser = await _userRepository.CreateAsync(user);

        // Assign default "User" role
        var userRole = await _roleRepository.GetByNameAsync("User");
        if (userRole != null)
        {
            createdUser.UserRoles.Add(new UserRole
            {
                UserId = createdUser.Id,
                RoleId = userRole.Id,
                AssignedAt = DateTime.UtcNow
            });
            await _userRepository.UpdateAsync(createdUser);
        }

        return createdUser.Id;
    }

    public string GenerateJwtToken(int userId, string email, List<string> roles)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new(ClaimTypes.Email, email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
