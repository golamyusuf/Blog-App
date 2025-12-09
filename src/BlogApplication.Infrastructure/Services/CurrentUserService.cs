using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using BlogApplication.Application.Interfaces;

namespace BlogApplication.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int UserId
    {
        get
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
        }
    }

    public string Email
    {
        get
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;
        }
    }

    public List<string> Roles
    {
        get
        {
            return _httpContextAccessor.HttpContext?.User?
                .FindAll(ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList() ?? new List<string>();
        }
    }

    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

    public bool IsAdmin => Roles.Contains("Admin");
}
