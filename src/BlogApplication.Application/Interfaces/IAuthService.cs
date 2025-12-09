namespace BlogApplication.Application.Interfaces;

/// <summary>
/// Service interface for authentication and authorization operations.
/// </summary>
public interface IAuthService
{
    Task<(string token, int userId, List<string> roles)> AuthenticateAsync(string email, string password);
    Task<int> RegisterAsync(string username, string email, string password, string? firstName, string? lastName);
    string GenerateJwtToken(int userId, string email, List<string> roles);
}
