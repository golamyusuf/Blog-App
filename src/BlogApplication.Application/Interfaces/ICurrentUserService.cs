namespace BlogApplication.Application.Interfaces;

public interface ICurrentUserService
{
    int UserId { get; }
    string Email { get; }
    List<string> Roles { get; }
    bool IsAuthenticated { get; }
    bool IsAdmin { get; }
}
