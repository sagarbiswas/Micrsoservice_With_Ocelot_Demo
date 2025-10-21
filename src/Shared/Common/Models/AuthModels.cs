namespace Common.Models;

public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public DateTime Expiration { get; set; }
}

public class UserClaims
{
    public const string UserId = "uid";
    public const string Username = "username";
    public const string Role = "role";
}