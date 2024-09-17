using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using OnPaper_Auth.Abstractions;

namespace OnPaper_Auth.Services;
public class AuthenticationService
{
    // ! Path to the service account key
    private readonly string _pathToServiceAccountKey = "./OnPaper-Auth_Config/onpaper-auth-firebase-adminsdk-58xcs-f23cdc41a4.json";
    
    private readonly IdentityToolKitEndpointFactory factory = new("AIzaSyBalYTiFf5MRK_K14kWD0_RTAqZTpP9Q0c");
    

    public AuthenticationService()
    {
        //TODO: Fix with dependency injection
        var secretsServiceAccountKey = Environment.GetEnvironmentVariable("FIREBASE_SERVICE_ACCOUNT_JSON");
        var credentialFromJson = default(GoogleCredential);
        var credentialFromFile = default(GoogleCredential);
        try
        {
            credentialFromJson = GoogleCredential.FromJson(secretsServiceAccountKey);
            credentialFromFile = GoogleCredential.FromFile(_pathToServiceAccountKey);
        }
        catch (ArgumentNullException)
        {
            Console.WriteLine("Service account key not found in environment variables or The file");
            throw new NullReferenceException();
        }
        

        if (FirebaseApp.DefaultInstance == null)
        {
            FirebaseApp.Create(new AppOptions()
            {
                Credential = credentialFromFile ?? credentialFromJson
            });
        }
    }

    public async Task<string> RegisterAsync(string userName, string fullName, string email, string password)
    {
        Random rnd = new Random();
        return FirebaseAuth.DefaultInstance.CreateUserAsync(new UserRecordArgs()
        {
            DisplayName = fullName,
            Uid = userName,
            PhotoUrl = $"https://picsum.photos/id/{rnd.Next(1,64)}/200/",
            Email = email,
            EmailVerified = false,
            Password = password
        }).Result.Uid;
    }

    public async Task<string> AuthenticateAsync(string email, string password, bool returnSecureToken)
    {
        return await factory.CreateEndpoint(IdentityToolKitEndpointsEnum.SignIn).SendRequestAsync(new { email, password , returnSecureToken });
    }

    public void Dispose()
    {
        FirebaseApp.DefaultInstance.Delete();
    }
}
