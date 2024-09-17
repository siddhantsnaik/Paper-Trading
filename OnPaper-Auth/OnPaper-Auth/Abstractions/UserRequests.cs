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

