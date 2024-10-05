



using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.DependencyInjection;
using OnPaper_Auth.Abstractions;
using Serilog;

namespace OnPaper_Auth.DependencyInj;

public class FirebaseAppInj
{
    public static void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<FirebaseApp>(sp =>
        {
            var secretsServiceAccountKey = Environment.GetEnvironmentVariable("FIREBASE_SERVICE_ACCOUNT_JSON");
            var credentialFromJson = default(GoogleCredential);
            try
            {
                credentialFromJson = GoogleCredential.FromJson(secretsServiceAccountKey);
                
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

            return FirebaseApp.Create(new AppOptions()
            {
                Credential = credentialFromJson
            });
        });
        services.AddSingleton<FirebaseAuth>(sp =>
        {
            return FirebaseAuth.DefaultInstance;
        });
    }

}