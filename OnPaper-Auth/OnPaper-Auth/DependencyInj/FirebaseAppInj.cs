//using FirebaseAdmin;
//using FirebaseAdmin.Auth;
//using Google.Apis.Auth.OAuth2;
//using Microsoft.Extensions.DependencyInjection;

//namespace OnPaper_Auth.DependencyInj;

//public class FirebaseAppInj
//{
//    public static void ConfigureServices(IServiceCollection services)
//    {
//        services.AddSingleton<FirebaseApp>(sp =>
//        {
//            return FirebaseApp.Create(new AppOptions()
//            {
//                Credential = GoogleCredential.FromFile("/OnPaper-Auth/OnPaper-Auth_Config/onpaper-auth-firebase-adminsdk-58xcs-f23cdc41a4.json")
//            });
//        });
//        services.AddSingleton<FirebaseAuth>(sp =>
//        {
//            return FirebaseAuth.DefaultInstance;
//        });
//    }

//}