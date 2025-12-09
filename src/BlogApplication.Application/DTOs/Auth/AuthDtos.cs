namespace BlogApplication.Application.DTOs.Auth;

/// <summary>
/// Data transfer object for user login requests.
/// </summary>
public class LoginRequestDto
{
    /// <summary>
    /// The user's email address.
    /// </summary>
    public string Email { get; set; } = string.Empty;
    
    /// <summary>
    /// The user's password.
    /// </summary>
    public string Password { get; set; } = string.Empty;
}

/// <summary>
/// Data transfer object for login response containing JWT token and user information.
/// </summary>
public class LoginResponseDto
{
    /// <summary>
    /// The JWT authentication token.
    /// </summary>
    public string Token { get; set; } = string.Empty;
    
    /// <summary>
    /// The authenticated user's information.
    /// </summary>
    public UserDto User { get; set; } = null!;
}

/// <summary>
/// Data transfer object for user registration requests.
/// </summary>
public class RegisterRequestDto
{
    /// <summary>
    /// The desired username (must be unique).
    /// </summary>
    public string Username { get; set; } = string.Empty;
    
    /// <summary>
    /// The user's email address (must be unique).
    /// </summary>
    public string Email { get; set; } = string.Empty;
    
    /// <summary>
    /// The user's password (will be hashed before storage).
    /// </summary>
    public string Password { get; set; } = string.Empty;
    
    /// <summary>
    /// The user's first name (optional).
    /// </summary>
    public string? FirstName { get; set; }
    
    /// <summary>
    /// The user's last name (optional).
    /// </summary>
    public string? LastName { get; set; }
}
