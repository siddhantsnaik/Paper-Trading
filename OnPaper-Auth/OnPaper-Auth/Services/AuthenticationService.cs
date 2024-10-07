using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using OnPaper_Auth.Abstractions;
using Serilog;
using System.Net;
using System.Net.Http;
using System.Text.Json;


namespace OnPaper_Auth.Services;
public class AuthenticationService
{
    // ! Path to the service account key
    private readonly string _pathToServiceAccountKey = "./OnPaper-Auth_Config/onpaper-auth-firebase-adminsdk-58xcs-f23cdc41a4.json";

    private readonly FirebaseApp _firebaseApp;
    private readonly FirebaseAuth _firebaseAuth;
    private IdentityToolKitEndpointFactory factory = new();
    

public AuthenticationService(FirebaseApp firebaseApp, FirebaseAuth firebaseAuth)
    {
        _firebaseApp = firebaseApp;
        _firebaseAuth = firebaseAuth;

        ////TODO: Fix with dependency injection
        //var secretsServiceAccountKey = Environment.GetEnvironmentVariable("FIREBASE_SERVICE_ACCOUNT_JSON");
        var secretsWebAPIKey = Environment.GetEnvironmentVariable("FIREBASE_WEB_API_KEY");
        factory = new IdentityToolKitEndpointFactory(secretsWebAPIKey);
        //var credentialFromJson = default(GoogleCredential);
        //var credentialFromFile = default(GoogleCredential);
        //try
        //{
        //    credentialFromJson = GoogleCredential.FromJson(secretsServiceAccountKey);
        //    //credentialFromFile = GoogleCredential.FromFile(_pathToServiceAccountKey);
        //}
        //catch (ArgumentNullException)
        //{
        //    Console.WriteLine("Service account key not found in environment variables");
        //    Log.Error("Service account key not found in environment variables");
        //    throw;
        //}
        //catch (FileNotFoundException)
        //{
        //    Console.WriteLine("Service account key not found in the file");
        //    Log.Error("Service account key not found in the file");
        //    throw;
        //}
        //catch (NullReferenceException)
        //{
        //    Log.Error("Something to do with the Firebase Web API Key");
        //    throw;
        //}
        //catch (Exception)
        //{
        //    Log.Error("Something went wrong with the Firebase Admin SDK");
        //    Console.WriteLine("Service account key not found in environment variables or the file");
        //    throw;
        //}

        //if (FirebaseApp.DefaultInstance == null)
        //{
        //    FirebaseApp.Create(new AppOptions()
        //    {
        //        Credential = credentialFromJson ?? credentialFromFile
        //    });
        //}
    }

    public async Task<string> RegisterAsync(string userName, string fullName, string email, string password)
    {
        Random rnd = new Random();

        try
        {
            var response = await _firebaseAuth.CreateUserAsync(new UserRecordArgs()
            {
                DisplayName = fullName,
                Uid = userName,
                PhotoUrl = $"https://picsum.photos/id/{rnd.Next(1, 64)}/200/",
                Email = email,
                EmailVerified = false,
                Password = password
            });
        }
        catch (Exception e)
        {
            Log.Error(e.Message);
            return FormatErrorMessage(e.Message.Split('\n')[0]); // Fix: Return a valid HttpContent instance
        }
        return await AuthenticateAsync(email, password, true);
    }

    private string FormatErrorMessage(string errorMessage)
    {
        return $@"
            {{
                ""error"": {{
                    ""errors"": [
                        {{
                            ""domain"": ""global"",
                            ""reason"": ""invalid"",
                            ""message"": ""{errorMessage}""
                        }}
                    ],
                    ""code"": 400,
                    ""message"": ""{errorMessage}""
                }}
            }}";
    }

    public async Task<string> AuthenticateAsync(string email, string password, bool returnSecureToken)
    {
        return await factory.CreateEndpoint(IdentityToolKitEndpointsEnum.SignIn).SendRequestAsync(new { email, password, returnSecureToken });
    }
    
    public async Task<string> RefreshToken(string refreshToken)
    {
        var _endpoint = $"https://securetoken.googleapis.com/v1/token?key={Environment.GetEnvironmentVariable("FIREBASE_WEB_API_KEY")}";
        var _httpClient = new HttpClient();
        string grant_type = "refresh_token";
        var content = new StringContent(JsonSerializer.Serialize(new {grant_type, refreshToken }), System.Text.Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(_endpoint, content);
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> RevokeToken(string idToken)
    {
        return await factory.CreateEndpoint(IdentityToolKitEndpointsEnum.DeleteToken).SendRequestAsync(new { idToken });
    }

    public void Dispose()
    {
        _firebaseApp.Delete();
    }
}
