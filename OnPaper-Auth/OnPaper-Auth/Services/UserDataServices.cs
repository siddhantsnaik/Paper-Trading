using FirebaseAdmin;
using FirebaseAdmin.Auth;
using OnPaper_Auth.Abstractions;
using Serilog;
using System.Text.Json;


namespace OnPaper_Auth.Services;

public class UserDataServices
{
    private readonly FirebaseApp _firebaseApp;
    private readonly FirebaseAuth _firebaseAuth;
    
    public UserDataServices(FirebaseApp firebaseApp, FirebaseAuth firebaseAuth)
    {
        _firebaseApp = firebaseApp;
        _firebaseAuth = firebaseAuth;
        var secretsWebAPIKey = Environment.GetEnvironmentVariable("FIREBASE_WEB_API_KEY");
        IdentityToolKitEndpointFactory factory = new IdentityToolKitEndpointFactory(secretsWebAPIKey);
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

    public async Task<string> UpdateUserProfile(string idToken, UserUpdateRequest UserRecord)
    {
        try
        {
            FirebaseToken decodedToken = await _firebaseAuth.VerifyIdTokenAsync(idToken);
            var user = await _firebaseAuth.GetUserAsync(decodedToken.Uid);

            var profileUpdate = new UserRecordArgs()
            {
                Uid = user.Uid,
                Email = UserRecord.Email,
                DisplayName = UserRecord.FullName,
                Password = UserRecord.Password,
                PhotoUrl = UserRecord.ProfilePicture,
                PhoneNumber = UserRecord.PhoneNumber
            };

            var updatedUser = await _firebaseAuth.UpdateUserAsync(profileUpdate);

            UserSession userSession = new UserSession()
            {
                localId = updatedUser.Uid,
                email = updatedUser.Email,
                idToken = idToken,
                expiresIn = "3600",
                kind = "OnPaperAuth#UpdateUserResponse",
                displayName = updatedUser.DisplayName,
                profilePicture = updatedUser.PhotoUrl,
                registered = true,
                phoneNumber = updatedUser.PhoneNumber
            };
            return JsonSerializer.Serialize(userSession);
        }
        catch (FirebaseAuthException e)
        {
            Log.Error("Error verifying ID token: {0}", e);
            return FormatErrorMessage(e.Message.Split('\n')[0]);
        }
    }
}
