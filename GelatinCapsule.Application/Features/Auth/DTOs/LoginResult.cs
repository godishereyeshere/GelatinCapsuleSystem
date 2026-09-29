namespace GelatinCapsule.Application.Features.Auth.DTOs;

public class LoginResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }

    public int? UserId { get; set; }
    public string? Username { get; set; }
    public string? FullName { get; set; }
    public bool MustChangePassword { get; set; }

    public static LoginResult Ok(int userId, string username, string fullName, bool mustChangePassword)
        => new()
        {
            Success = true,
            UserId = userId,
            Username = username,
            FullName = fullName,
            MustChangePassword = mustChangePassword
        };

    public static LoginResult Fail(string message)
        => new() { Success = false, ErrorMessage = message };
}