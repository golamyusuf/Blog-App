namespace BlogApplication.Application.Interfaces;

/// <summary>
/// Service interface for accessing information about the currently authenticated user.
/// </summary>
public interface ICurrentUserService
{
    int UserId { get; }
    string Email { get; }
    List<string> Roles { get; }
    bool IsAuthenticated { get; }
    bool IsAdmin { get; }
}
