namespace BlogApplication.Application.Interfaces;

/// <summary>
/// Service interface for password hashing and verification using BCrypt.
/// </summary>
public interface IPasswordHasher
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string passwordHash);
}
