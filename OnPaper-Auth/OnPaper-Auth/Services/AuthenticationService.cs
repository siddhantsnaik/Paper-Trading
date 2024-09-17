using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using OnPaper_Auth.Abstractions;
using Serilog;


namespace OnPaper_Auth.Services;
public class AuthenticationService
{
    // ! Path to the service account key
    private readonly string _pathToServiceAccountKey = "./OnPaper-Auth_Config/onpaper-auth-firebase-adminsdk-58xcs-f23cdc41a4.json";

    private IdentityToolKitEndpointFactory factory = new();
    

public AuthenticationService()
    {
        
        //TODO: Fix with dependency injection
        var secretsServiceAccountKey = Environment.GetEnvironmentVariable("FIREBASE_SERVICE_ACCOUNT_JSON");
        var secretsWebAPIKey = Environment.GetEnvironmentVariable("FIREBASE_WEB_API_KEY");
        var credentialFromJson = default(GoogleCredential);
        var credentialFromFile = default(GoogleCredential);
        try
        {
            credentialFromJson = GoogleCredential.FromJson(secretsServiceAccountKey);
            credentialFromFile = GoogleCredential.FromFile(_pathToServiceAccountKey);
            factory = new IdentityToolKitEndpointFactory(secretsWebAPIKey);
        }
        catch (ArgumentNullException)
        {
            Console.WriteLine("Service account key not found in environment variables");
            Log.Error("Service account key not found in environment variables");
            throw;
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine("Service account key not found in the file");
            Log.Error("Service account key not found in the file");
            throw;
        }
        catch (NullReferenceException)
        {
            Log.Error("Something to do with the Firebase Web API Key");
            throw;
        }
        catch (Exception)
        {
            Log.Error("Something went wrong with the Firebase Admin SDK");
            Console.WriteLine("Service account key not found in environment variables or the file");
            throw;
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
        return (await FirebaseAuth.DefaultInstance.CreateUserAsync(new UserRecordArgs()
        {
            DisplayName = fullName,
            Uid = userName,
            PhotoUrl = $"https://picsum.photos/id/{rnd.Next(1, 64)}/200/",
            Email = email,
            EmailVerified = false,
            Password = password
        })).Uid;
    }

    public async Task<string> AuthenticateAsync(string email, string password, bool returnSecureToken)
    {
        return await factory.CreateEndpoint(IdentityToolKitEndpointsEnum.SignIn).SendRequestAsync(new { email, password, returnSecureToken });
    }

    public void Dispose()
    {
        FirebaseApp.DefaultInstance.Delete();
    }
}
