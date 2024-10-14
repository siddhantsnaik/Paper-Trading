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
//TODO: Add UID update too?
public class UserUpdateRequest
{
    public required string IdToken { get; set; }
    public string? Email { get; set; }
    public string? FullName { get; set; }
    public string? Password { get; set; }
    public string? ProfilePicture { get; set; }
    public string? PhoneNumber { get; set; }
}

public class UserSession
{
    public string localId { get; set; }
    public string email { get; set; }
    public string idToken { get; set; }
    public string expiresIn { get; set; }
    public string kind { get; set; }
    public string displayName { get; set; }
    public string profilePicture { get; set; }
    public bool registered { get; set; }
    public string phoneNumber { get; set; }
}

//displayName
//email
//expiresIn
//idToken
//kind
//localId
//profilePicture
//refreshToken not included
//registered
