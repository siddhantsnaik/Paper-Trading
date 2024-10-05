using Google.Apis.Requests;

namespace OnPaper_Auth.Abstractions;
public class UserRegisterRequest
{
    public required string Username { get; set; }
    public required string Password { get; set; }
    public required string Email { get; set; }
    public required string FullName { get; set; }
   
}
public class UserLoginRequest
{
    public required string Email { get; set; }
    public required string Password { get; set; }

    
}

public class UserUpdateRequest
{
    public required string IdToken { get; set; }
    public string? Email { get; set; }
    public string? FullName { get; set; }
    public string? Password { get; set; }
    public string? ProfilePicture { get; set; }
    public string? PhoneNumber { get; set; }
}
