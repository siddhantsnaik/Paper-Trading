using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace OnPaper_Auth.Abstractions
{
    public enum IdentityToolKitEndpointsEnum
    {
        // Auth
        SignUp,
        SignIn,
        VerifyEmail,
        ResetPassword,
        ChangePassword,
        ChangeEmail,
        DeleteAccount,
        // User
        GetUser,
        UpdateUser,
        DeleteUser,
        // Admin
        GetUsers,
        UpdateUserByAdmin,
        DeleteUserByAdmin,
        // Token
        GetToken,
        VerifyToken,
        DeleteToken,
        // Email
        SendEmail,
        VerifyEmailByCode,
        VerifyEmailByLink,
        // Phone
        SendPhone,
        VerifyPhoneByCode,
        VerifyPhoneByLink,
        // Password
        SendPasswordReset,
        VerifyPasswordResetByCode,
        VerifyPasswordResetByLink,
        // Custom
        Custom
    }

    public class IdentityToolKitEndpoint
    {
        private readonly string _endpoint;
        private readonly string _apiKey;
        private readonly HttpClient _httpClient;

        public IdentityToolKitEndpoint(IdentityToolKitEndpointsEnum endpointEnum, string apiKey)
        {
            _endpoint = $"https://identitytoolkit.googleapis.com/v1/accounts:{GetEndpoint(endpointEnum)}?key={apiKey}";
            Console.WriteLine($"Initialized _endpoint: {_endpoint}"); // Log the _endpoint value when initialized
            _apiKey = apiKey;
            _httpClient = new HttpClient();
        }

        private string GetEndpoint(IdentityToolKitEndpointsEnum endpoint)
        {
            return endpoint switch
            {
                IdentityToolKitEndpointsEnum.SignUp => "signUp",
                IdentityToolKitEndpointsEnum.SignIn => "signInWithPassword",
                IdentityToolKitEndpointsEnum.VerifyEmail => "sendOobCode",
                IdentityToolKitEndpointsEnum.ResetPassword => "resetPassword",
                IdentityToolKitEndpointsEnum.ChangePassword => "update",
                IdentityToolKitEndpointsEnum.ChangeEmail => "update",
                IdentityToolKitEndpointsEnum.DeleteAccount => "delete",
                IdentityToolKitEndpointsEnum.GetUser => "lookup",
                IdentityToolKitEndpointsEnum.UpdateUser => "update",
                IdentityToolKitEndpointsEnum.DeleteUser => "delete",
                IdentityToolKitEndpointsEnum.GetUsers => "lookup",
                IdentityToolKitEndpointsEnum.UpdateUserByAdmin => "update",
                IdentityToolKitEndpointsEnum.DeleteUserByAdmin => "delete",
                IdentityToolKitEndpointsEnum.GetToken => "token",
                IdentityToolKitEndpointsEnum.VerifyToken => "lookup",
                IdentityToolKitEndpointsEnum.DeleteToken => "revokeToken",
                IdentityToolKitEndpointsEnum.SendEmail => "sendOobCode",
                IdentityToolKitEndpointsEnum.VerifyEmailByCode => "confirmEmailVerification",
                IdentityToolKitEndpointsEnum.VerifyEmailByLink => "confirmEmailVerification",
                IdentityToolKitEndpointsEnum.SendPhone => "sendVerificationCode",
                IdentityToolKitEndpointsEnum.VerifyPhoneByCode => "verifyPhoneNumber",
                IdentityToolKitEndpointsEnum.VerifyPhoneByLink => "verifyPhoneNumber",
                IdentityToolKitEndpointsEnum.SendPasswordReset => "sendOobCode",
                IdentityToolKitEndpointsEnum.VerifyPasswordResetByCode => "resetPassword",
                IdentityToolKitEndpointsEnum.VerifyPasswordResetByLink => "resetPassword",
                IdentityToolKitEndpointsEnum.Custom => "custom",
                _ => throw new ArgumentException("Invalid endpoint", nameof(endpoint))
            };
        }

        public async Task<string> SendRequestAsync(object payload)
        {
            var content = new StringContent(JsonSerializer.Serialize(payload), System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(_endpoint, content);
            return await response.Content.ReadAsStringAsync();
        }
    }

    public class IdentityToolKitEndpointFactory(string apiKey)
    {
        private readonly string _apiKey = apiKey;

        public IdentityToolKitEndpoint CreateEndpoint(IdentityToolKitEndpointsEnum endpointEnum)
        {
            return new IdentityToolKitEndpoint(endpointEnum, _apiKey);
        }
    }
}